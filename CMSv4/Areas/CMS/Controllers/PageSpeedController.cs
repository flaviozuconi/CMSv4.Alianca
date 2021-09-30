using CMSv4.BusinessLayer;
using Framework.Utilities;
using System.Web.Mvc;
using VM2.PageSpeed;

namespace CMSApp.Areas.CMS.Controllers
{
    public class PageSpeedController : SecureController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Item(decimal id)
        {
            var model = BLPagina.ObterPageSpeed(id);
            return View(model);
        }

        [Compress]
        [JsonHandleError]
        public JsonResult RunAnalysis(decimal CodigoPagina)
        {
            var result = new BLCmsPageSpeed(CodigoPagina).RunAnalysis();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Compress]
        [JsonHandleError]
        public JsonResult ScoreToHexaColor(double performanceScore, double pwaScore, double accessibilityScore, double bestPracticesScore, double seoScore)
        {
            var pageSpeedUtil = new BLPageSpeedUtil();

            return Json(new
            {
                performanceColor = pageSpeedUtil.ScoreToColor(performanceScore),
                pwaColor = pageSpeedUtil.ScoreToColor(pwaScore),
                accessibilityColor = pageSpeedUtil.ScoreToColor(accessibilityScore),
                bestPracticesColor = pageSpeedUtil.ScoreToColor(bestPracticesScore),
                seoColor = pageSpeedUtil.ScoreToColor(seoScore)
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
