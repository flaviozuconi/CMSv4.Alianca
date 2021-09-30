using Framework.Utilities;
using System.Web.Mvc;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class IdiomaController : Controller
    {
        //
        // GET: /Modulo/Idioma/

        public ActionResult Index(string view)
        {
            var pagina = BLPagina.Atual;
            var dict = BLPagina.ListarPaginasRelacionadas(pagina.Codigo, (string)RouteData.Values["extra1"]);

            ViewBag.IdiomaAtual = BLIdioma.Atual.Sigla.ToUpper();
            
            return View(view ?? "Index", dict);
        }
    }
}
