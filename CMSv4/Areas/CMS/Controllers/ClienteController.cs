using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using CMSv4.BusinessLayer.DataTableFilter;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ClienteController : AdminBaseCRUDPortalController<MLCliente, MLCliente>
    {
        #region Listar

        [Compress]
        [CheckPermission(global::Permissao.Visualizar)]
        public override JsonResult Listar(MLCliente criterios)
        {
            try
            {
                var filter = new DataTableFilter(Request.QueryString).Get();

                var lista = BLCliente.ListarAdmin
                (
                    filter.SearchedValue,
                    BLPortal.Atual.Codigo,
                    filter.OrderBy,
                    filter.Sort,
                    filter.Start,
                    filter.Length
                );

                return new DataTableResult()
                {
                    Data = lista,
                    TotalRows = (lista.Count > 0 ? lista[0].TotalRows : 0) ?? 0
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            var model = new MLClienteCompleto();

            if (id.HasValue)
                model = BLCliente.ObterCompleto(id.Value);

            ViewData["Estados"] = CRUD.ListarCache(new MLEstado(), 480);
            ViewData["Grupos"] = CRUD.Listar(new MLGrupoCliente { CodigoPortal = PortalAtual.Codigo }, PortalAtual.ConnectionString);

            return View(model);

        }

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public override ActionResult Item(MLCliente model)
        {          
            TempData["Salvo"] = new BLCliente().Salvar(model, Request.Form["listaCodigoGrupo"]) > 0;
            return RedirectToAction("Index");   
        }


        /// <summary>
        /// Verifica se o login não ficará duplicado ao salvar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValid(decimal? id, string login)
        {
            return Json(new BLClienteAdm().ValidarExistente(id, "Login", "Codigo", login));
        }

        /// <summary>
        /// Verifica se o email não ficará duplicado ao salvar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValidEmail(decimal? id, string email)
        {
            return Json(new BLClienteAdm().ValidarExistente(id, "Email", "Codigo", email));
        }

        #endregion

        #region Senha

        /// <summary>
        /// Altear Senha do Cliente
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        [HttpPost]
        public ActionResult EnviarNovaSenha(decimal id)
        {
            return Json(new { success = BLCliente.EnviarNovaSenha(id, TAdm("Nova Senha")) });
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
        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public override ActionResult Excluir(List<string> ids)
        {
            new BLCliente().Excluir(ids);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region ListarEstados

        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult ListarEstados(decimal CodigoPais)
        {
            return Json(CRUD.Listar(new MLEstado() { CodigoPais = CodigoPais }, PortalAtual.ConnectionString));
        }

        #endregion
    }
}
