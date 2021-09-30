using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSv4.BusinessLayer;
using CMSv4.Model;

namespace CMSApp.Areas.CMS.Controllers
{
    /// <summary>
    /// Funcionalidade
    /// Determina quais os recursos podem ser acessados pelo usuário
    /// </summary>
    public class FuncionalidadeController : AdminBaseCRUDController<MLFuncionalidade, MLFuncionalidade>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            try
            {
                var model = new MLFuncionalidade();
                if (id.HasValue) model = BLFuncionalidade.ObterCompleto(id.Value);

                ViewData["Grupos"] = CRUD.Listar(new MLGrupo { Ativo = true });
                
                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Override do método do AdminBaseCRUDController para "Burlar" rota e utilizar com mais parametros
        /// </summary>
        [NonAction]
        public override ActionResult Item(MLFuncionalidade model) { throw new NotImplementedException(); }

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public ActionResult Item(MLFuncionalidade model, List<MLFuncionalidadeItemPermissaoForm> permissoes)
        {
            TempData["Salvo"] = new BLFuncionalidade().Salvar(model, permissoes) > 0;
                  
            return RedirectToAction("Index");
        }

        #endregion

        #region Validar Url

        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValidUrl(decimal? id, string url)
        {
            return Json(new BLFuncionalidade().ValidarExistente(id, "Url", "Codigo", url));
        }

        #endregion
    }
}
