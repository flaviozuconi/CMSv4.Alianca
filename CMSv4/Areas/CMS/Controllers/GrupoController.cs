using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    /// <summary>
    /// Grupos
    /// Agrupa as funcionalidades que os usuários 
    /// tem acesso no sisetma
    /// </summary>
    public class GrupoController : AdminBaseCRUDController<MLGrupo, MLGrupo>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            var model = BLGrupo.ObterCompleto(id.GetValueOrDefault(0));
            ViewBag.Funcionalidades = new BLFuncionalidade().Listar(new MLFuncionalidade { Ativo = true });

            return View(model);
        }

        /// <summary>
        /// Override do método do AdminBaseCRUDController para "Burlar" rota e utilizar com mais parametros
        /// </summary>
        [NonAction]
        public override ActionResult Item(MLGrupo model) { throw new NotImplementedException(); }

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public ActionResult Item(MLGrupo model, List<MLGrupoItemPermissaoForm> permissoes)
        {
            TempData["Salvo"] = new BLGrupo().Salvar(model, permissoes) > 0;
            return RedirectToAction("Index");
        }

        #endregion
    }
}
