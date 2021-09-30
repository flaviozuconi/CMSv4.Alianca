using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Framework.Utilities
{
    public class BaseController : Controller
    {
        public static string T(string texto)
        {
            var T = new BLTraducao(PortalAtual.Obter);
            return T.Obter(texto);
        }

        public static string TAdm(string texto)
        {
            var T = new BLTraducao();
            return T.ObterAdm(texto);
        }
    }
}