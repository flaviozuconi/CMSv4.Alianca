using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ConfiguracaoController : AdminBaseCRUDController<MLConfiguracao, MLConfiguracao>
    {
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValid(decimal? id, string value)
        {
            return Json(new BLCmsConfiguracao().ValidarExistente(id, "Chave", "Codigo", value));
        }

    }
}
