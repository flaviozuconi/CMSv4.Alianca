using CMSv4.BusinessLayer;
using CMSv4.BusinessLayer.Base.GestaoInformacoesImportacao;
using CMSv4.Model.Base.GestaoInformacoesImportacao;
using Framework.Utilities;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class GestaoInformacoesImportacaoController : AdminBaseCRUDPortalController<MLGestaoInformacoesImportacao, MLGestaoInformacoesImportacao>
    {

        #region Item
        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public override ActionResult Item(MLGestaoInformacoesImportacao model)
        {
            model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
            model.DataAtualizacao = DateTime.Now;

            SetCodigoPortal(model);
            TempData["Salvo"] = CRUD.Salvar<MLGestaoInformacoesImportacao>(model) > 0;

            return RedirectToAction("Index");
        }
        #endregion

        #region Importar
        /// <summary>
        /// Importa os dados da planilha de Gestao de Informações e Importação
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

            var importacao = new MLGestaoInformacoesImportacaoHistorico
            {
                DataImportacao = DateTime.Now,
                Usuario = BLUsuario.ObterLogado()?.Login,
                Sucesso = false,
                Finalizado = false,
                Arquivo = ""
            };

            importacao.Codigo = new BLCRUD<MLGestaoInformacoesImportacaoHistorico>().Salvar(importacao);

            if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/GestaoInformacoesImportacao")))
                Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/GestaoInformacoesImportacao"));

            var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
            if (file != null)
                file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesImportacao"), nomeArquivo));

            importacao.Arquivo = nomeArquivo;

            var erros = BLGestaoInformacoesImportacao.Importacao(file, excluir, importacao);
            TempData["SalvoImportacao"] = erros.Length <= 0; 
            return Json(new { success = true });
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
                return DataTable.Listar(new MLGestaoInformacoesImportacaoHistorico(), Request.QueryString);
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
            return File("~/Templates/gestao-informacoes-importacao-template.xlsx", "application/ms-excel", $"gestao-informacoes-importacao-template.xlsx");
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
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesImportacao/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/GestaoInformacoesImportacao/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}