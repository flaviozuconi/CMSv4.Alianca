using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Framework.Utilities;
using System.Transactions;
using CMSv4.BusinessLayer;
using CMSv4.Model;

namespace CMSApp.Areas.CMS.Controllers
{
    public class SecaoController : SecurePortalController
    {
        #region Index

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Index
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Listar

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
        public ActionResult Listar(MLSecao criterios)
        {
            try
            {
                criterios.CodigoPortal = PortalAtual.Codigo;
                return CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Item

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = new MLSecao();

                if (id.HasValue) model = BLSecao.ObterCompleto(id.Value, portal.ConnectionString);

                if (model.Codigo.HasValue && model.CodigoPortal != portal.Codigo) return null;

                ViewData["Grupos"] = CRUD.Listar(new MLGrupo { Ativo = true });
                ViewData["GruposCliente"] = CRUD.Listar(new MLGrupoCliente { Ativo = true,CodigoPortal = portal.Codigo }, portal.ConnectionString);

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
        public ActionResult Item(MLSecao model)
        {
            var portal = PortalAtual.Obter;
            try
            {

                string listaCodigoGrupo = Request.Form["listaCodigoGrupo"];
                string listaCodigoGrupoCliente = Request.Form["listaCodigoGrupoCliente"];

                if (ModelState.IsValid)
                {
                    model.CodigoPortal = portal.Codigo;

                    if (!model.Restrito.HasValue || !model.Restrito.Value) listaCodigoGrupoCliente = string.Empty;

                    using (var scope = new TransactionScope(portal.ConnectionString))
                    {
                        var codigo = CRUD.Salvar<MLSecao>(model);
                        model.Codigo = codigo;
                        if (listaCodigoGrupo == null) listaCodigoGrupo = string.Empty;
                        CRUD.Excluir<MLSecaoItemGrupo>("CodigoSecao", model.Codigo.Value);
                        if (!string.IsNullOrEmpty(listaCodigoGrupo))
                        {
                            foreach (var item in listaCodigoGrupo.Split(','))
                            {
                                var novoItem = new MLSecaoItemGrupo
                                {
                                    CodigoSecao = codigo,
                                    CodigoGrupo = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar<MLSecaoItemGrupo>(novoItem);
                            }
                        }

                        
                        CRUD.Excluir<MLSecaoPermissao>("CodigoSecao", model.Codigo.Value);
                        if (!string.IsNullOrEmpty(listaCodigoGrupoCliente))
                        {
                            foreach (var item in listaCodigoGrupoCliente.Split(','))
                            {
                                var novoItem = new MLSecaoPermissao
                                {
                                    CodigoSecao = codigo,
                                    CodigoGrupoCliente = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar<MLSecaoPermissao>(novoItem);
                            }
                        }
                        scope.Complete();

                        TempData["Salvo"] = codigo > 0;    
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult Excluir(List<string> ids)
        {
            try
            {
                foreach (var item in ids)
                {
                    CRUD.Excluir<MLSecao>(Convert.ToDecimal(item),PortalAtual.ConnectionString);
                }

                return Json(new { success = true });
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
