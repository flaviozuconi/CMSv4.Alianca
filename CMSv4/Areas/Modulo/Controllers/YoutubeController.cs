using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class YoutubeController : ModuloBaseController<MLModuloYoutubeEdicao, MLModuloYoutubeHistorico, MLModuloYoutubePublicado>
    {
        

        

        #region Edição

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloYoutubeEdicao model)
        {
            try
            {
                model.Link = model.Link.Replace("/watch?v=", "/embed/");
                CRUD.Salvar(model, PortalAtual.Obter.ConnectionString);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion
    }
}
