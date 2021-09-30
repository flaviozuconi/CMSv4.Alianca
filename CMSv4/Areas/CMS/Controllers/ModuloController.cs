using System;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ModuloController : SecureController
    {
        #region ListaConfig

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListaConfig()
        {
            return View();
        }

        #endregion

        #region ListaConfigItem

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)] 
        public ActionResult ListaConfigItem(decimal? id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = new MLListaConfig();

                if (id.HasValue)
                {
                    model = CRUD.Obter(new MLListaConfig { Codigo = id }, portal.ConnectionString);
                    ViewBag.Aprovadores = CRUD.Listar(new MLGrupo(), portal.ConnectionString);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ListaConfigItem(MLListaConfig model)
        {
            var portal = PortalAtual.Obter;

            try
            {
                if (ModelState.IsValid)
                {
                    model.CodigoPortal = portal.Codigo;

                    if (!model.Codigo.HasValue)
                    {
                        var lista = CRUD.Listar<MLListaConfig>(new MLListaConfig(), portal.ConnectionString);

                        model.Codigo = lista.Count + 1;
                    }

                    var codigo = CRUD.Salvar<MLListaConfig>(model, portal.ConnectionString);

                    TempData["Salvo"] = codigo > 0;
                }
                else
                {
                    return View(model);
                }

                return RedirectToAction("ListaConfig");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ListarListaConfig

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarListaConfig(MLListaConfig criterios)
        {
            var portal = PortalAtual.Obter;

            try
            {
                criterios.CodigoPortal = portal.Codigo;

                var retorno = CRUD.ListarJson(criterios, Request.QueryString, portal.ConnectionString);
                return retorno;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion
    }
}
