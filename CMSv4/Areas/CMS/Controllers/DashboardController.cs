using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.BusinessLayer;
using System;
using VM2.Google.BusinessLayer;
using System.Web;

namespace CMSApp.Areas.CMS.Controllers
{
    public class DashboardController : SecureController
    {
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Cropper()
        {
            return View();
        }

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Cropper(HttpPostedFileBase file)
        {
            file.SaveAs(@"D:\foto.jpg");

            return View();
        }

        /// <summary>
        /// Página padrão da área administrativa
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Index()
        {
            new BLDashboard().DefinirIdioma(Request.QueryString["l"]);

            return View();
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Analise()
        {
            if(BLAnalytics.HasRequiredInfoToGetData())
                return View();

            return View("AnalyticsNaoConfigurado");
        }

        #region UserRealTime

        [JsonHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult GetAmountOfUserOnlineInRealTime()
        {
            var analytics = BLAnalytics.GetAmountOfUserOnlineInRealTime();

            return Json(new { Sucesso = true, data = analytics[0].realTime }, JsonRequestBehavior.AllowGet);
            
        }

        #endregion 

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult GetAnalyticsData(DateTime start, DateTime end)
        {
            var analyticsResult = BLAnalytics.GetInfo(start, end);

            return PartialView("Analytics/_AnalysisResult", analyticsResult);
        }
    }
}
