using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ClienteAdmController : AdminBaseCRUDController<MLCliente, MLCliente>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            var model = new BLClienteAdm().ObterViewModel(id.GetValueOrDefault(0));

            return View(model);
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public override ActionResult Item(MLCliente model)
        {
            var IsNovoRegistro = model.Codigo.HasValue;

            model.Codigo = new BLClienteAdm().Salvar(model, Convert.ToBoolean(Request.Form["RemoverCapa"]));

            ViewData["Novo"] = IsNovoRegistro;
            ViewData["Portais"] = BLUsuario.ObterLogado().Portais;
            TempData["Salvo"] = true;

            if (IsNovoRegistro)
                return View(model);

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
        public ActionResult Senha(decimal id)
        {
            var Cliente = new MLClienteAlterarSenha { Codigo = id };

            return View(Cliente);
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult EnviarNovaSenha(decimal id)
        {
            return Json(new { success = BLCliente.EnviarNovaSenha(id, TAdm("Nova Senha")) });
        }

        #endregion

        #region Excluir

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public override ActionResult Excluir(List<string> ids)
        {
            new BLClienteAdm().Excluir(ids);
            return Json(new { Sucesso = true });
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
        [JsonHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult GruposPortal(decimal portal, decimal cliente)
        {
            var modelportal = CRUD.Obter<MLPortal>(portal);

            ViewData["Grupos"] = CRUD.Listar(new MLGrupoCliente { CodigoPortal = portal }, modelportal.ConnectionString);
            ViewData["CodigoPortal"] = portal;
            ViewData["GruposCliente"] = CRUD.Listar(new MLClienteItemGrupo { CodigoCliente = cliente }, modelportal.ConnectionString);

            return View(modelportal);
        }

        /// <summary>
        /// Altear Senha do Cliente
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult GruposPortal(decimal portal, decimal cliente, List<decimal> associados, List<decimal> nassociados)
        {
            new BLClienteAdm().SalvarGruposPortal(portal, cliente, associados, nassociados);

            return Json(new { success = true });
        }

        #endregion
    }
}