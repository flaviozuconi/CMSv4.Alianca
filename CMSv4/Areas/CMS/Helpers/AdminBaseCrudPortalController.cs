using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


public abstract class AdminBaseCRUDPortalController<TModelLista, TModelGravar> : SecurePortalController
    where TModelLista : class
    where TModelGravar : class
{
        #region Index

        [Compress]
        [CheckPermission(global::Permissao.Visualizar)]
        public virtual ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Listar

        [Compress]
        [DataTableHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public virtual JsonResult Listar(TModelLista criterios)
        {
            SetCodigoPortal(criterios);
            return DataTable.Listar(criterios, Request.QueryString);
        }
        #endregion

        #region Item

        [Compress]
        [CheckPermission(global::Permissao.Visualizar)]
        public virtual ActionResult Item(decimal? id)
        {
            return View(CRUD.Obter<TModelGravar>(id.GetValueOrDefault(0)));
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        //[ValidateInput(false)]
        public virtual ActionResult Item(TModelGravar model)
        {
            SetCodigoPortal(model);
            TempData["Salvo"] = CRUD.Salvar<TModelGravar>(model) > 0;

            return RedirectToAction("Index");
        }

        #endregion

        #region Excluir
        
        [Compress]
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public virtual ActionResult Excluir(List<string> ids)
        {
            foreach (var item in ids)
                CRUD.Excluir<TModelGravar>(Convert.ToDecimal(item));

            return Json(new { Sucesso = true });
        }
        #endregion

        protected void SetCodigoPortal<TipoModel>(TipoModel model)
        {
            var prop = model.GetType().GetProperty("CodigoPortal");

            if (prop != null)
                prop.SetValue(model, PortalAtual.Codigo, null);
        }
    }
