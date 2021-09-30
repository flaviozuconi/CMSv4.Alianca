using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using System.Web;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ArquivoAdminController : AdminBaseCRUDPortalController<MLArquivoCategoria, MLArquivoCategoria>
    {
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
        public override JsonResult Listar(MLArquivoCategoria criterios)
        {
            var lista = new BLArquivoCategoria().Listar(criterios);

            return new DataTableResult()
            {
                Data = lista,
                TotalRows = lista.Count
            };
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
        public override ActionResult Item(decimal? id)
        {
            var model = new BLArquivoCategoria().Obter(id.GetValueOrDefault(0));
            ViewData["categorias"] = CRUD.Listar(new MLCategoriaAgrupador() { CodigoPortal = PortalAtual.Obter.Codigo }, PortalAtual.Obter.ConnectionString);

            return View(model);
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public override ActionResult Item(MLArquivoCategoria model)
        {
            TempData["Salvo"] = new BLArquivoCategoria().Salvar(model) > 0;
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
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public override ActionResult Excluir(List<string> ids)
        {
            new BLArquivoCategoria().Excluir(ids, PortalAtual.Obter.ConnectionString);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region ValidarNome

        /// <summary>
        /// Validar Nome do Registro
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarNome(string nome, decimal? id)
        {
            try
            {
                var codigo = BLModuloArquivo.Obter(nome, id);

                if ((codigo.HasValue && codigo.Value > 0))
                    return Json(T("Já existe uma categoria de arquivo cadastrada com esse Nome"));
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Já existe uma categoria de arquivo cadastrada com esse Nome"));
            }
        }

        #endregion
        
        //////////////////ARQUIVOS
        #region Arquivo
        /// <summary>
        /// Arquivo
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Arquivo(decimal CodigoCategoria)
        {
            ViewBag.CodigoCategoria = CodigoCategoria;

            return View();
        }
        #endregion

        #region ArquivoItem

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ArquivoItem(decimal? Codigo, decimal CodigoCategoria)
        {
            var model = new BLArquivos().Obter(Codigo.GetValueOrDefault(0), CodigoCategoria);

            return View(model);
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ArquivoItem(MLArquivo model, HttpPostedFileBase imgThumb, bool? RemoverImg)
        {
            if (ModelState.IsValid)
                new BLArquivos().Salvar(model, imgThumb, RemoverImg);
         
            return Redirect("/cms/" + PortalAtual.Diretorio + "/arquivoadmin/arquivo?codigocategoria=" + model.CodigoCategoria);
        }
        #endregion

        #region ListarArquivos
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
        public ActionResult ListarArquivos(MLArquivo criterios)
        {
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }
        #endregion

        #region ExcluirArquivos
        /// <summary>
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult ExcluirArquivos(List<string> ids)
        {
            new BLArquivos().Excluir(ids, PortalAtual.Obter.ConnectionString);
            return Json(new { Sucesso = true });
        }
        #endregion

        #region UploadFile
        /// <summary>
        /// Upload
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult UploadFile(MLArquivo model)
        {
            return Json(new { Sucesso = new BLArquivos().UploadFile(model.CodigoCategoria.GetValueOrDefault(0)) });
        }
        #endregion
    }
}