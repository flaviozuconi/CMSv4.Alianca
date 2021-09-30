using Framework.Utilities;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class AgrupadorAdminController : AdminBaseCRUDPortalController<MLCategoriaAgrupador, MLCategoriaAgrupador>
    {
        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Item(MLCategoriaAgrupador model)
        {
            TempData["Salvo"] = new BLCategoriaAgrupador().Salvar(model) > 0;
            return RedirectToAction("Index");
        }
    }
}
