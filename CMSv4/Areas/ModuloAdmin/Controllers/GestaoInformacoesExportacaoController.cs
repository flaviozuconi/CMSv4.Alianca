using CMSv4.BusinessLayer;
using CMSv4.Model.Base.GestaoInformacoesExportacao;
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
    public class GestaoInformacoesExportacaoController : AdminBaseCRUDPortalController<MLGestaoInformacoesExportacao, MLGestaoInformacoesExportacao>
    {
        #region Item
        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public override ActionResult Item(MLGestaoInformacoesExportacao model)
        {
            model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
            model.DataAtualizacao = DateTime.Now;

            TempData["Salvo"] = CRUD.Salvar<MLGestaoInformacoesExportacao>(model) > 0;

            return RedirectToAction("Index");

        }
        #endregion

        #region Importar
        /// <summary>
        /// Importa os dados da planilha de Gestao de Informações e Exportação
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <returns></returns>
        public ActionResult Importacao(HttpPostedFileBase file, bool? excluir)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            if (file == null)
            {
                return Json(new { success = false, msg = TAdm("Planilha não informada para a importação.") });
            }

            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLGestaoInformacoesExportacao>(file);

            var importacao = new MLGestaoInformacoesExportacaoHistorico
            {
                DataImportacao = DateTime.Now,
                Usuario = BLUsuario.ObterLogado()?.Login,
                Sucesso = false,
                Finalizado = false,
                Arquivo = ""
            };

            importacao.Codigo = new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().Salvar(importacao);

            if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/GestaoInformacoesExportacao")))
                Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/GestaoInformacoesExportacao"));

            var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
            if (file != null)
                file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesExportacao"), nomeArquivo));

            importacao.Arquivo = nomeArquivo;

            string erros = ValidaPlanilha(lstModel);

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);

                return Json(new { success = false, msg = erros });
            }

            var codigoUsuario = BLUsuario.ObterLogado()?.Codigo;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                InserirPlanilha(lstModel, importacao, excluir, codigoUsuario);

            }));

            thread.Start();

            TempData["SalvoImportacao"] = erros.Length <= 0;

            return Json(new { success = true });
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de Gestao de Informações e Exportação
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLGestaoInformacoesExportacao> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errPropostaComercial = new List<int>();
            List<int> errNumeroBooking = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.PropostaComercial))
                    errPropostaComercial.Add(linha);

                if (string.IsNullOrEmpty(item.NumeroBooking))
                    errNumeroBooking.Add(linha);

                linha++;
            }

            if (errPropostaComercial.Count > 0)
                erros.AppendLine("O campo \"Proposta Comercial\" é obrigatório na(s) linha(s): " + string.Join(",", errPropostaComercial) + "<br>");

            if (errNumeroBooking.Count > 0)
                erros.AppendLine("O campo \"Numero do Booking\" é obrigatório na(s) linha(s): " + string.Join(",", errNumeroBooking) + "<br>");

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
        /// <param name="codigoUsuario"></param>
        public void InserirPlanilha(List<MLGestaoInformacoesExportacao> lista, 
            MLGestaoInformacoesExportacaoHistorico importacao, bool? excluir, decimal? codigoUsuario)
        {

            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_GIE_GESTAO_INFORMACOES_EXPORTACAO"));
                }

                foreach (var item in lista)
                {
                    item.CodigoUsuario = codigoUsuario;
                    item.DataAtualizacao = DateTime.Now;

                    new BLCRUD<MLGestaoInformacoesExportacao>().Salvar(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;

                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);
            }
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
                return DataTable.Listar(new MLGestaoInformacoesExportacaoHistorico(), Request.QueryString);
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
        /// Download do modelo
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadModelo()
        {
            return File("~/Templates/gestao-informacoes-exportacao-template.xlsx", "application/ms-excel", $"gestao-informacoes-exportacao-template.xlsx");
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
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesExportacao/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesExportacao/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}