using CMSv4.BusinessLayer;
using CMSv4.Model.Base;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMSv4.BusinessLayer.Base;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ProgramacaoPropostaController : AdminBaseCRUDPortalController<MLProgramacaoProposta, MLProgramacaoProposta>
    {
        #region Item
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Compress]
        //[CheckPermission(global::Permissao.Visualizar)]
        //public override ActionResult Item(decimal? id)
        //{
        //    return View(CRUD.Obter<MLProgramacaoProposta>(id.GetValueOrDefault(0)));
        //}

        ///// <summary>
        ///// Visualizar ou Editar o registro conforme permissão do usuário
        ///// </summary>
        ///// <param name="id">Código do registro</param>
        ///// <remarks>
        ///// GET:
        /////     /Area/Controller/Item/id
        ///// </remarks>

        //[CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public override ActionResult Item(MLProgramacaoProposta model)
        {
            model.NomeUsuario = BLUsuario.ObterLogado().Login;
            model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
            model.DataImportacao = DateTime.Now;

            SetCodigoPortal(model);
            TempData["Salvo"] = CRUD.Salvar<MLProgramacaoProposta>(model) > 0;

            return RedirectToAction("Index");

        }
        #endregion

        #region Listar Propostas
        /// <summary>
        /// Listagem programação de propostas
        /// </summary>
        /// <returns></returns>
        public JsonResult ListarPropostas()
        {
            string filtro = Request.QueryString["search[value]"];

            int orderBy;
            int.TryParse(Request.QueryString["order[0][column]"], out orderBy);

            bool isAsc = Request.QueryString["order[0][dir]"].Equals("asc");
            
            int start;
            int length;
            int.TryParse(Request.QueryString["start"], out start);
            int.TryParse(Request.QueryString["length"], out length);

            var lista = new BLProgramacaoProposta().Listar(filtro, start, start + length, orderBy, isAsc);

            // Retorna os resultados
            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = lista.Count > 0 ? lista[0]?.Total : 0,
                    recordsFiltered = lista.Count > 0 ? lista[0]?.Total : 0,
                    data = lista
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 50000000
            };
        }
        #endregion

        #region Importar
        /// <summary>
        /// Importa os dados oriundos da planilha de programação navio
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <returns></returns>
        public ActionResult Importar(HttpPostedFileBase file, bool? excluir)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

                if (file == null)
                {
                    return Json(new { success = false, msg = TAdm("Planilha não informada para a importação.") });
                }

                var lstModel = BLUtilitarios.EPPlus.LerExcel<MLProgramacaoProposta>(file);

                var importacao = new MLProgramacaoPropostaHistorico
                {
                    DataImportacao = DateTime.Now,
                    CodigoUsuario = BLUsuario.ObterLogado()?.Codigo,
                    Sucesso = false,
                    Finalizado = false,
                    Arquivo = ""
                };

                importacao.Codigo = new BLCRUD<MLProgramacaoPropostaHistorico>().Salvar(importacao);

                if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaoproposta")))
                    Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaoproposta"));

                var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
                if (file != null)
                    file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaoproposta"), nomeArquivo));

                importacao.Arquivo = nomeArquivo;

                string erros = ValidaPlanilha(lstModel);

                if (!string.IsNullOrEmpty(erros))
                {
                    importacao.Sucesso = false;
                    importacao.Finalizado = true;
                    new BLCRUD<MLProgramacaoPropostaHistorico>().SalvarParcial(importacao);

                    ApplicationLog.Log($"Erro integração \"Programação Propostas\": {erros}");

                    return Json(new { success = false, msg = erros });
                }

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    InserirPlanilha(lstModel, importacao, excluir);
                }));

                thread.Start();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.Log($"Erro geral: {ex.Message}");

                return Json(new { success = false });
            }
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de ProgramacaoProposta
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLProgramacaoProposta> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> erroProposta= new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (item.NumeroProposta == null || !item.NumeroProposta.HasValue)
                    erroProposta.Add(linha);

                linha++;
            }

            if (erroProposta.Count > 0)
                erros.AppendLine("O campo \"Nº Proposta\" é obrigatório na(s) linha(s): " + string.Join(",", erroProposta) + "<br>");

            return erros.ToString();
        }
        #endregion

        #region Inserir Planilha
        /// <summary>
        /// Insere os registros da planilha no banco
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="importacao"></param>
        /// <param name="excluir"></param>
        public void InserirPlanilha(List<MLProgramacaoProposta> lista, MLProgramacaoPropostaHistorico importacao, bool? excluir)
        {
            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_PROPOSTA"));
                }

                foreach (var item in lista)
                {
                    item.CodigoUsuario = importacao.CodigoUsuario;
                    item.DataImportacao = importacao.DataImportacao;
                    item.ValidadeDDR = Add3Horas(item.ValidadeDDR);
                    item.Validade = Add3Horas(item.Validade);
                    item.Fechamento = Add3Horas(item.Fechamento);
                    item.Inclusao = Add3Horas(item.Inclusao);
                    item.ValidadeCotacao = Add3Horas(item.ValidadeCotacao);
                    item.CotacaoAceita = Add3Horas(item.CotacaoAceita);
                    item.CotacaoNegada = Add3Horas(item.CotacaoNegada);
                    item.CotacaoRejeitada = Add3Horas(item.CotacaoRejeitada);

                    new BLCRUD<MLProgramacaoProposta>().Salvar(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoPropostaHistorico>().SalvarParcial(importacao);
            }
            catch (Exception ex)
            {
                ApplicationLog.Log($"Erro interno: {ex.Message}");
                ApplicationLog.ErrorLog(ex);
                importacao.Finalizado = true;
                importacao.Sucesso = false;
                new BLCRUD<MLProgramacaoPropostaHistorico>().SalvarParcial(importacao);
            }
        }

        private DateTime? Add3Horas(DateTime? Data)
        {
            if (Data.HasValue)
            {
                return Data.Value.AddHours(3);
            }

            return null;
        }
        #endregion

        #region Historico Importacao
        /// <summary>
        /// Listar Historico Importacao
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoricoImportacao()
        {
            try
            {
                return DataTable.Listar(new MLProgramacaoPropostaHistorico(), Request.QueryString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }
        #endregion

        #region Download Modelo
        /// <summary>
        /// Download modelos
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadModelo()
        {
            return File("~/Templates/programacao-proposta-template.xlsx", "application/ms-excel", "programacao-proposta-template.xlsx");
        }
        #endregion

        #region Download Importacao
        /// <summary>
        /// Download arquivo de importação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadImportacao(decimal? id)
        {
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaoproposta/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaoproposta/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
