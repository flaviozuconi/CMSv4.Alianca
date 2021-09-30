using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Framework.Utilities;

namespace CMSApp.Areas.CMS.Controllers
{
    public class AjudaController : SecureController
    {
        //
        // GET: /CMS/Ajuda/
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
