using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class FaqAdminController : AdminBaseCRUDPortalController<MLFaq, MLFaq>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id) => View(new BLFaq().Obter(id));

        [HttpPost, JsonHandleError, ValidateAntiForgeryToken]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true), ValidateInput(false)]
        public override ActionResult Item(MLFaq model)
        {
            model.CodigoPortal = model.CodigoPortal.GetValueOrDefault(PortalAtual.Codigo.Value);
            TempData["Salvo"] = new BLFaq().Salvar(model) > 0;

            return RedirectToAction("Index");
        }

        #endregion

        #region Categoria

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Categoria() => View();

        #region Listar

        [CheckPermission(global::Permissao.Visualizar), DataTableHandleError]
        public ActionResult CategoriaListar(MLFaqCategoria criterios)
        {
            criterios.CodigoPortal = PortalAtual.Codigo;
            return CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult CategoriaItem(decimal? id)
        {
            return View(new BLFaq().ObterCategoria(id));
        }

        [HttpPost, JsonHandleError(View = "Categoria"), ValidateInput(false), CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public ActionResult CategoriaItem(MLFaqCategoria model)
        {
            TempData["Salvo"] = new BLFaq().SalvarCategoria(model) > 0;
            return RedirectToAction("Categoria");
        }

        #endregion

        #region Excluir

        [HttpPost, CheckPermission(global::Permissao.Excluir)]
        public ActionResult CategoriaExcluir(List<string> ids)
        {
            try
            {
                new BLFaq().ExcluirCategoria(ids);

                return Json(new { Sucesso = true });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("FK_MOD_FAQ_FAQ_MOD_FCA_FAQ_CATEGORIA"))
                {
                    ModelState.AddModelError("Codigo", T("O registro está sendo referenciado por algum registro no cadastro de FAQs"));
                    return Json(new { Sucesso = false, msg = "O registro está sendo referenciado por algum registro no cadastro de FAQs" });
                }
                else
                    return Json(new { Sucesso = false, msg = ex.Message });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { Sucesso = false, msg = ex.Message });
            }
        }

        #endregion

        #endregion
    }
}