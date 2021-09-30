using System.Web.Mvc;
using Framework.Utilities;

namespace CMSApp.Areas.CMS.Controllers
{
    public class RoxyFilemanController : SecureController
    {
        //
        // GET: /CMS/RoxyFileman/
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Index(string type, string integration)
        {
            return View("~/Content/js/plugins/ckeditor/plugins/fileman/Index.cshtml");
        }
    }
}
