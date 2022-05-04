using CMSv4.BusinessLayer;
using CMSv4.BusinessLayer.Base.GestaoInformacoesExportacao;
using CMSv4.Model.Base.GestaoInformacoesExportacao;
using Framework.Utilities;
using System;
using System.IO;
using System.Net;
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

        #region Validacao
        /// <summary>
        /// Valida se já existe o registro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proposta"></param>
        /// <param name="booking"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValid(decimal? id, string proposta, string booking)
        {
            var validacao = BLGestaoInformacoesExportacao.IsValid(id, proposta, booking);

            return Json(validacao);
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

            var erros = BLGestaoInformacoesExportacao.Importacao(file, excluir, importacao);
            TempData["SalvoImportacao"] = erros.Length <= 0;

            return Json(new { success = erros != null && erros.Length > 0 ? false : true, msg = erros});
            
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