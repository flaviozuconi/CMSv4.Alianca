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

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ProgramacaoNavioAdminController : AdminBaseCRUDPortalController<MLProgramacaoNavio, MLProgramacaoNavio>
    {
        #region Item

        ///// <summary>
        ///// Visualizar ou Editar o registro conforme permissão do usuário
        ///// </summary>
        ///// <param name="id">Código do registro</param>
        ///// <remarks>
        ///// GET:
        /////     /Area/Controller/Item/id
        ///// </remarks>

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Item(MLProgramacaoNavio model)
        {

            model.DataImportacao = DateTime.Now;

            SetCodigoPortal(model);
            TempData["Salvo"] = CRUD.Salvar<MLProgramacaoNavio>(model) > 0;

            return RedirectToAction("Index");

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
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            if (file == null)
            {
                return Json(new { success = false, msg = TAdm("Planilha não informada para a importação.") });
            }

            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLProgramacaoNavio>(file);

            var importacao = new MLProgramacaoNavioHistorico
            {
                DataImportacao = DateTime.Now,
                Usuario = BLUsuario.ObterLogado()?.Login,
                Sucesso = false,
                Finalizado = false,
                Arquivo = ""
            };

            importacao.Codigo = new BLCRUD<MLProgramacaoNavioHistorico>().Salvar(importacao);

            if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaonavio")))
                Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaonavio"));

            var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
            if (file != null)
                file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio"), nomeArquivo));

            importacao.Arquivo = nomeArquivo;

            string erros = ValidaPlanilha(lstModel);

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);

                return Json(new { success = false, msg = erros });
            }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                InserirPlanilha(lstModel, importacao, excluir);
            }));

            thread.Start();

            return Json(new { success = true });
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de ProgramacaoNavio
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLProgramacaoNavio> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errOrigem = new List<int>();
            List<int> errNavio = new List<int>();
            List<int> errDeadline = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.Origem))
                    errOrigem.Add(linha);

                if (string.IsNullOrEmpty(item.NavioViagem))
                    errNavio.Add(linha);

                if (item.Deadline == null)
                    errDeadline.Add(linha);

                linha++;
            }

            if (errOrigem.Count > 0)
                erros.AppendLine("O campo \"Origem\" é obrigatório na(s) linha(s): " + string.Join(",", errOrigem) + "<br>");

            if (errNavio.Count > 0)
                erros.AppendLine("O campo \"Navio/Viagem\" é obrigatório na(s) linha(s): " + string.Join(",", errNavio) + "<br>");

            if (errDeadline.Count > 0)
                erros.AppendLine("O campo \"Deadline\" é obrigatório na(s) linha(s): " + string.Join(",", errDeadline) + "<br>");

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
        public void InserirPlanilha(List<MLProgramacaoNavio> lista, MLProgramacaoNavioHistorico importacao, bool? excluir)
        {

            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("DELETE FROM MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS"));
                }

                foreach (var item in lista)
                {
                    item.Usuario = importacao.Usuario;
                    item.DataImportacao = importacao.DataImportacao;
                    item.SaidaPrevisto = Add3Horas(item.SaidaPrevisto);
                    item.SaidaRealizado = Add3Horas(item.SaidaRealizado);
                    item.ChegadaPrevisto = Add3Horas(item.ChegadaPrevisto);
                    item.ChegadaRealizado = Add3Horas(item.ChegadaRealizado);
                    item.Deadline = Add3Horas(item.Deadline);
                    item.ChegadaTransbordoPrevisto1 = Add3Horas(item.ChegadaTransbordoPrevisto1);
                    item.ChegadaTransbordoRealizado1 = Add3Horas(item.ChegadaTransbordoRealizado1);
                    item.ChegadaTransbordoPrevisto2 = Add3Horas(item.ChegadaTransbordoPrevisto2);
                    item.ChegadaTransbordoRealizado2 = Add3Horas(item.ChegadaTransbordoRealizado2);
                    item.ChegadaTransbordoPrevisto3 = Add3Horas(item.ChegadaTransbordoPrevisto3);
                    item.ChegadaTransbordoRealizado3 = Add3Horas(item.ChegadaTransbordoRealizado3);
                    item.ChegadaTransbordoPrevisto4 = Add3Horas(item.ChegadaTransbordoPrevisto4);
                    item.ChegadaTransbordoRealizado4 = Add3Horas(item.ChegadaTransbordoRealizado4);

                    new BLCRUD<MLProgramacaoNavio>().Salvar(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);
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
                return DataTable.Listar(new MLProgramacaoNavioHistorico(), Request.QueryString);
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
            return File("~/Templates/programacao-navio-template.xlsx", "application/ms-excel", $"programacao-navio-template.xlsx");
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
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
