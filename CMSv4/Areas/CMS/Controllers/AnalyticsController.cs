using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Framework.Utilities;
//using Google.GData.Analytics;
//using Google.GData.Client;

namespace CMSApp.Areas.CMS.Controllers
{
    public class AnalyticsController : SecurePortalController
    {
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Erro", ex);
            }
        }
    }
}
