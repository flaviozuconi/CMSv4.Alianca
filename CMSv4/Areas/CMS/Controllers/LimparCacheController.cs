using System.Web.Mvc;
using CMSv4.BusinessLayer;
using Framework.Utilities;

namespace CMSApp.Areas.CMS.Controllers
{
    public class LimparCacheController : SecurePortalController
    {
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Index()
        {
            return View();
        }

        #region Limpar

        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult Limpar(string key)
        {
            BLLimparCache.Remove(key);
            return Json(new { success = true });
        }

        #endregion
    }
}
