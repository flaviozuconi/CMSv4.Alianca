using Framework.Utilities;
using System;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class RedeSocialController : VM2.Areas.CMS.Helpers.ModuloBaseController<MLModuloRedeSocialEdicao, MLModuloRedeSocialHistorico, MLModuloRedeSocialPublicado>
    {
        #region Override Actions CMS

        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                var portal = BLPortal.Atual;
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                MLModuloRedeSocial model = CRUD.Obter<MLModuloRedeSocialPublicado>(new MLModuloRedeSocialPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Edição

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloRedeSocialEdicao model)
        {
            try
            {
                if (!model.IsFacebook.HasValue) model.IsFacebook = false;
                if (!model.IsTwitter.HasValue) model.IsTwitter = false;
                if (!model.IsLinkedin.HasValue) model.IsLinkedin = false;
                    
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                //Limpar Cache
                BLCachePortal.RemoveAll("redes_sociais_html_lista_item");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #endregion

        #region Feeds

        [OutputCache(Duration = 1800, VaryByParam = "*")]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult Feeds(decimal deCodigoPagina, int decRepositorio, bool? bEdicao, string view)
        {
            var portal = BLPortal.Atual;
            var model = new MLModuloRedeSocial();            
                
            try
            {
                if (bEdicao.HasValue && bEdicao.Value)
                    model = CRUD.Obter(new MLModuloRedeSocialEdicao { CodigoPagina = deCodigoPagina, Repositorio = decRepositorio }, portal.ConnectionString);
                else
                    model = CRUD.Obter(new MLModuloRedeSocialPublicado { CodigoPagina = deCodigoPagina, Repositorio = decRepositorio }, portal.ConnectionString);

                //Listar os feeds das redes sociais selecionadas na propriedade do módulo e retornar no json
                return Json(new { success = true, html = BLRedeSocial.ListarFeedsHtml(model, this, view) });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }

        #endregion

        #region Lista

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Lista(MLModuloRedeSocial model, bool? bEdicao)
        {
            ViewBag.Edicao = bEdicao;
            return View(model);
        }

        #endregion

        #region Destaque

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Destaque(MLModuloRedeSocial model, bool? bEdicao)
        {
            ViewBag.Edicao = bEdicao;
            return View(model);
        }

        #endregion

        #region ScriptLista

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptLista(MLModuloRedeSocial model, bool? bEdicao)
        {
            ViewBag.Edicao = bEdicao;
            return View(model);
        }

        #endregion

        #region ScriptDestaque

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptDestaque(MLModuloRedeSocial model, bool? bEdicao)
        {
            ViewBag.Edicao = bEdicao;
            return View(model);
        }

        #endregion
    }


  


}
