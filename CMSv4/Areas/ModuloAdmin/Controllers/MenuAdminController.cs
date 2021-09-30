using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class MenuAdminController : AdminBaseCRUDPortalController<MLMenuModulo, MLMenuModulo>
    {
        #region Salvar

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "Index")]
        public ActionResult MenuSalvar(MLMenuCompletoModulo model)
        {
            TempData["Salvo"] = new BLMenuModulo().Salvar(model, PortalAtual.Obter.ConnectionString) > 0;
            return RedirectToAction("Index");
        }
        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            return View(new BLMenuModulo().ObterCompleto(id.GetValueOrDefault(), PortalAtual.ConnectionString, true) ?? new MLMenuCompletoModulo());
        }
        #endregion

        #region Item

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        [JsonHandleError]
        public ActionResult MenuItemSalvar(MLMenuItemModulo model)
        {
            CRUD.SalvarParcial(model, PortalAtual.ConnectionString);
            return new JsonResult() { Data = new { Sucesso = true, CodigoMenuItem = model.Codigo, Icon = model.Icone } };
        }
        #endregion

        #region IncluirArquivo

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult IncluirArquivo(decimal? id, string file)
        {
            var portal = PortalAtual.Obter;
            var model = new BLBanner().Obter(id.Value, portal.ConnectionString);

            if (model == null || !model.Codigo.HasValue) return null;

            var nome = Path.GetFileName(file);

            var diretorio = (BLConfiguracao.Pastas.ModuloBanner(portal.Diretorio) + "/" + id).Replace("//", "/");
            ViewData["diretorioGaleria"] = diretorio;
            ViewData["ajax"] = true;

            return PartialView("ArquivoItem");
        }

        #endregion

        #region Menu

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Menu(List<MLMenuItemModulo> itens, decimal? codigo)
        {
            return View(new MLMenuItemAdminModulo { CodigoAtual = codigo, Itens = itens });
        }
        #endregion

        #region MenuItem

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MenuItem(MLMenuItemModulo model) => View(model);

        #endregion

        #region CriarItem
      
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult CriarItem(decimal? codigopai, decimal codigoMenu)
        {
            MLMenuItemModulo novoItem = new BLMenuModulo().InserirMenuItem(codigopai, codigoMenu, PortalAtual.ConnectionString);

            var retorno = new JsonResult()
            {
                Data = new
                {
                    codigo = novoItem.Codigo,
                    ordem = novoItem.Ordem
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return retorno;
        }


        #endregion

        #region DeletarItem

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        [JsonHandleError]
        public JsonResult ExcluirItem(decimal id)
        {
            return Json(new { Sucesso = new BLMenuModulo().Excluir(id, PortalAtual.ConnectionString) > 0 });
        }

        #endregion

        #region ObterItem

        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ObterItem(decimal codigo)
        {
            return PartialView("MenuItem", new BLMenuModulo().ObterMenuItem(codigo, PortalAtual.ConnectionString));
        }

        #endregion

        #region OrdenarItem

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult OrdenarItem(decimal codigo, decimal? newParent, int posicao)
        {
            return Json(new { Sucesso = new BLMenuModulo().Ordenar(codigo, newParent.GetValueOrDefault(0), posicao, PortalAtual.ConnectionString) });
        }

        #endregion

        #region Renomear

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        [JsonHandleError]
        public JsonResult Renomear(decimal codigo, string novoNome)
        {
            bool retorno = new BLMenuModulo()
                                    .SalvarMenuItem(new MLMenuItemModulo()
                                    {
                                        Codigo = codigo,
                                        Nome = novoNome
                                    }, PortalAtual.ConnectionString) > 0;

            return Json(new { Sucesso = retorno });
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarMenu(decimal codigoMenu)
        {
            var model = new BLMenuModulo().ObterCompleto(codigoMenu, PortalAtual.ConnectionString, false);
            return Json(model.ItensMenu.Select(i => new MLJsTree() { id = i.Codigo.ToString(), parent = (i.CodigoPai.Value > 0) ? i.CodigoPai.ToString() : "#", text = i.Nome, icon = i.Icone }).ToList());
        }

        #endregion

        #region MenuView

        #region Excluir

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        [JsonHandleError]
        public ActionResult MenuViewExcluir(List<string> ids)
        {
            return Json(new { Sucesso = new BLMenuModulo().ExcluirMenuView(ids) > 0 });
        }

        #endregion

        #region Index

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MenuView() => View("MenuView/Index");

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult MenuViewItem(decimal? id)
        {
            var model = new BLMenuModulo().ObterMenuView(new MLMenuModuloView { Codigo = id }, PortalAtual.ConnectionString) ?? new MLMenuModuloView();
            model.View.Unescape();
            model.Script.Unescape();

            return View("MenuView/Item", model);
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "MenuView")]
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult MenuViewItem(MLMenuModuloView model, string nomeAntigo)
        {
            TempData["Salvo"] = new BLMenuModulo().SalvarMenuView(model, nomeAntigo);
            return RedirectToAction("MenuView");
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult ListarMenuView(MLMenuModuloView criterios)
        {
            criterios.CodigoPortal = PortalAtual.Codigo;
            return DataTable.Listar(criterios, Request.QueryString);
        }

        #endregion

        #region ValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult MenuViewValidarNome(decimal? codigo, string nome)
        {
            var portal = BLPortal.Atual;
            var model = new BLMenuModulo().ObterMenuView(new MLMenuModuloView() { Nome = nome, CodigoPortal = portal.Codigo }, portal.ConnectionString);

            if (model != null && model.Codigo.HasValue && ((codigo.HasValue && codigo.Value != model.Codigo) || !codigo.HasValue))
                return Json(T("Já existe um arquivo com esse nome."));

            return Json(true);
        }

        #endregion

        #endregion

        #region ValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarNome(decimal? Codigo, string Nome)
        {
            try
            {
                var model = new BLMenuModulo().Obter(new MLMenuModulo() { Codigo = Codigo, Nome = Nome, CodigoPortal = PortalAtual.Codigo }, PortalAtual.ConnectionString);

                if (model != null && model.Codigo.HasValue && model.Codigo.Value > 0 && Codigo != model.Codigo)
                    return Json(false);
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Não foi possível validar a URL, entre em contato com o administrador do sistema."));
            }
        }

        #endregion
    }
}
