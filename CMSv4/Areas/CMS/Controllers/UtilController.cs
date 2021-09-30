using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class UtilController : SecureController
    {
        //
        // GET: /CMS/Util/

        public ActionResult Index()
        {
            return View();
        }

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]    
        public ActionResult Criptografar(string valor, bool criptografar)
        {
            try
            {

                if (criptografar)
                    valor = BLEncriptacao.EncriptarAes(valor);
                else
                    valor = BLEncriptacao.DesencriptarAes(valor);

                return Json(new { success = true, resultado = valor });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarIconPack(string search, int page, int pageSize = 64, int size = 16)
        {
            try
            {
                var model = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MLIconPack>(new IconPack.Icon().Listar(search, page, pageSize, size));

                ViewBag.Size = size;

                return PartialView("Icon/_IconPicker", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [Compress]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarIconPackT(string search, int page, int pageSize = 64, int size = 16)
        {
            try
            {
                var model = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MLIconPack>(new IconPack.Icon().Listar(search, page, pageSize, size));

                ViewBag.Size = 16;

                return PartialView("Icon/_IconPicker", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
    }
}
