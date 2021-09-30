using Framework.Utilities;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.CMS.Controllers
{
    public class GrupoClienteController : AdminBaseCRUDPortalController<MLGrupoCliente, MLGrupoCliente>
    {
        #region Item

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]        
        public override ActionResult Item(MLGrupoCliente model)
        {
            TempData["Salvo"] = new BLGrupoCliente().Salvar(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion
    }
}
