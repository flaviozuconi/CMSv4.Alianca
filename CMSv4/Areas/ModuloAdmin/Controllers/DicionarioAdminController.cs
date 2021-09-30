using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class DicionarioAdminController : SecurePortalController
    {

        #region Dicionario

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Dicionario() => View();

        [DataTableHandleError]
        public ActionResult DicionarioListar(MLDicionarios criterios)
        {
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DicionarioItem(decimal? id)
        {
            var portal = PortalAtual.Obter;
            var model = new BLDicionario().Obter(new MLDicionarios { Codigo = id.GetValueOrDefault(0), CodigoPortal = portal.Codigo }) ?? new MLDicionarios();

            ViewBag.Grupos = new BLDicionarioGrupo().Listar(new MLDicionarioGrupo() { CodigoPortal = portal.Codigo, CodigoIdioma = BLIdioma.CodigoAtual });
            
            return View(model);            
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public ActionResult DicionarioItem(MLDicionarios model)
        {
            TempData["Salvo"] = new BLDicionario().Salvar(model) > 0;
            return RedirectToAction("Dicionario");
        }

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult DicionarioExcluir(List<string> ids)
        {
            return Json(new { Sucesso = new BLDicionario().Excluir(ids) > 0 });
        }

        #endregion

        #region DicionarioGrupo

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DicionarioGrupo() => View();


        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult DicionarioGrupoListar(MLDicionarioGrupo criterios)
        {
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DicionarioGrupoItem(decimal? id)
        {
            var model = new BLDicionarioGrupo().Obter(id.GetValueOrDefault(0));
            ViewBag.Paises = new BLDicionarioGrupo().Listar(new MLDicionarioGrupo() { CodigoPortal = PortalAtual.Obter.Codigo });

            return View(model);
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public ActionResult DicionarioGrupoItem(MLDicionarioGrupo model)
        {
            TempData["Salvo"] = new BLDicionarioGrupo().Salvar(model) > 0;
            return RedirectToAction("DicionarioGrupo");
        }

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public JsonResult DicionarioGrupoExcluir(List<string> ids)
        {
            new BLDicionarioGrupo().Excluir(ids);
            return Json(new { Sucesso = new BLDicionarioGrupo().Excluir(ids) > 0 });
        }

        #endregion

    }
}
