using Framework.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMSApp
{
    /// <summary>
    /// Registra onde o sistema pode encontrar algumas views e partial views
    /// utilizadas pelos modulos do CMS
    /// </summary>
    public class MyAreasViewEngine : RazorViewEngine
    {
        public MyAreasViewEngine()
        {
            var viewLocations = new List<string>();

            foreach (var portal in CRUD.Listar(new MLPortal { Ativo = true }))
            {
                viewLocations.Add("~/portal/" + portal.Diretorio + "/modulo/{1}/{0}.cshtml");
            }

            viewLocations.Add("~/areas/modulo/views/{1}/{0}.cshtml");

            viewLocations.Add("~/areas/moduloadmin/views/{1}/{0}.cshtml");

            viewLocations.Add("~/areas/cms/Views/{1}/{0}.cshtml");

            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(viewLocations.ToArray()).ToArray();
            base.ViewLocationFormats = base.ViewLocationFormats.Union(viewLocations.ToArray()).ToArray();
        }
    }
}
