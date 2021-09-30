using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

public abstract class AdminBaseCRUDController<TModelLista, TModelGravar> : SecureController
    where TModelLista : class
    where TModelGravar : class
{
    #region Index
    /// <summary>
    /// Index
    /// </summary>
    /// <returns></returns>
    [Compress]
    [CheckPermission(global::Permissao.Visualizar)]
    public virtual ActionResult Index()
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

    /// <summary>
    /// Visualizar ou Editar o registro conforme permissão do usuário
    /// </summary>
    /// <param name="id">Código do registro</param>
    /// <remarks>
    /// GET:
    ///     /Area/Controller/Item/id
    /// </remarks>
    [Compress]
    [CheckPermission(global::Permissao.Visualizar)]
    public virtual ActionResult Item(decimal? id)
    {
        return View(CRUD.Obter<TModelGravar>(id.GetValueOrDefault(0)));
    }

    /// <summary>
    /// Salvar registro
    /// </summary>
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
    /// <summary>
    /// Excluir registro
    /// </summary>
    /// <remarks>
    /// GET:
    ///     /Area/Controller/Excluir/id
    /// </remarks>
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

    private void SetCodigoPortal<TipoModel>(TipoModel model)
    {
        var prop = model.GetType().GetProperty("CodigoPortal");

        if (prop != null)
            prop.SetValue(model, PortalAtual.Codigo, null);
    }
}