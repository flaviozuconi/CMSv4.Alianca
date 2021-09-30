using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class GaleriaAdminController : AdminBaseCRUDPortalController<MLGaleriaMultimidia, MLGaleriaMultimidia>
    {
        #region Galeria

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public override ActionResult Item(MLGaleriaMultimidia model)
        {
            TempData["Salvo"] = new BLGaleriaMultimidia().Salvar(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion

        #region Arquivo

        #region Arquivo
        /// <summary>
        /// Arquivo
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Arquivo() => View();

        #endregion

        #region ArquivoItem

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ArquivoItem(decimal? id)
        {
            ListarTipos();
            return View(new BLGaleriaMultimidiaArquivo().Obter(id, Request.QueryString["CodigoGaleria"], Request.QueryString["CodigoIdioma"]));
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult ArquivoItem(MLGaleriaMultimidiaArquivo model, HttpPostedFileBase imgThumb, bool? RemoverImg)
        {
            ListarTipos();
            if(ModelState.IsValid)
                model = new BLGaleriaMultimidiaArquivo().Salvar(model, imgThumb, RemoverImg);

            return RedirectToAction("arquivo", new { codigogaleria = model.CodigoGaleria });
        }
        #endregion

        #region ListarArquivos

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult ListarArquivos(MLGaleriaMultimidiaArquivo criterios)
        {
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }
        #endregion

        #region ExcluirArquivos

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult ExcluirArquivo(List<string> ids)
        {
            return Json(new { Sucesso = new BLGaleriaMultimidiaArquivo().Excluir(ids, PortalAtual.ConnectionString) > 0 });
        }
        #endregion

        #region UploadFile

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult UploadFile(MLGaleriaMultimidiaArquivo model, HttpPostedFileBase file)
        {
            return Json(new
            {
                Sucesso = true,
                name = Path.GetFileName(new BLGaleriaMultimidiaArquivo().UploadFile(model, file, Request.Files)?.FileName)
            });

        }
        #endregion

        private void ListarTipos()
        {
            var portal = BLPortal.Atual;

            ViewBag.Tipos = new BLGaleriaMultimidiaTipo().Listar(new MLGaleriaMultimidiaTipo
            {
                CodigoPortal = portal.Codigo,
                Ativo = true
            }, portal.ConnectionString);
        }

        #endregion
    }
}