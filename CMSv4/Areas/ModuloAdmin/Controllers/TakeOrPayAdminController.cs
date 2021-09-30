using Framework.Utilities;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using System;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class TakeOrPayAdminController : AdminBaseCRUDPortalController<MLTakeOrPayEmbarqueCerto, MLTakeOrPayEmbarqueCerto>
    {
        #region Item
        /// <summary>
        /// Obter Take or Pay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            return View(BLTakeOrPay.ObterCompleto(id.GetValueOrDefault(0)));
        }

        #endregion

        #region Item

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost]
        public JsonResult ItemSalvar(MLTakeOrPayEmbarqueCertoCompleto model, string lstNumeroProposta)
        {
            TempData["Salvo"] = BLTakeOrPay.Salvar(model, lstNumeroProposta);

            BLTakeOrPay.EnviarEmailAdmin(model.Codigo);

            return new JsonResult() { Data = new { Sucess = true } };
        }

        #endregion

        #region ScriptCadastro
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloTakeOrPay model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion
    }
}
