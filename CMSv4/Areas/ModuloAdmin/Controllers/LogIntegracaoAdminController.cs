using Framework.Utilities;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class AssuntosController : AdminBaseCRUDPortalController<MLAssuntos, MLAssuntos>
    {
        #region Item

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public override ActionResult Item(MLAssuntos model)
        {
            TempData["Salvo"] = new BLAssunto().Salvar(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion
    }
}
