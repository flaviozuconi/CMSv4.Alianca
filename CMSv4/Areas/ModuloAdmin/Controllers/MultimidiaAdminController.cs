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
    public class MultimidiaAdminController : AdminBaseCRUDPortalController<MLMultimidiaCategoria, MLMultimidiaCategoria>
    {
        #region Listar

        [CheckPermission(global::Permissao.Visualizar), DataTableHandleError]
        public override JsonResult Listar(MLMultimidiaCategoria criterios)
        {
            var lista = new BLMultimidiaCategoria().ListGrid(criterios);

            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = lista.Count,
                    recordsFiltered = lista.Count,
                    data = lista
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {

            List<MLGrupo> PermissaoEscrita = new List<MLGrupo>();
            List<MLGrupoCliente> PermissaoLeitura = new List<MLGrupoCliente>();

            var model = new BLMultimidiaCategoria().ObterDadosParaEdicao(id, ref PermissaoEscrita, ref PermissaoLeitura);

            ViewBag.GruposLeitura = PermissaoLeitura.Select(c => new SelectListItem
            {
                Value = c.Codigo.ToString(),
                Text = c.Nome
            });

            ViewBag.GruposEscrita = PermissaoEscrita.Select(c => new SelectListItem
            {
                Value = c.Codigo.ToString(),
                Text = c.Nome
            });

            ViewData["categorias"] = new BLCategoriaAgrupador().Listar(new MLCategoriaAgrupador(), PortalAtual.ConnectionString);

            return View(model);

        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "Item")]
        [HttpPost]
        public override ActionResult Item(MLMultimidiaCategoria model)
        {
            if (new BLMultimidiaCategoria().Salvar(model) > 0)
            {
                TempData["Salvo"] = true;
                return RedirectToAction("Index");
            }

            return Redirect("/cms/" + PortalAtual.Diretorio + "/multimidiaadmin");
        }


        #endregion

        #region Excluir

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        [JsonHandleError]
        public override ActionResult Excluir(List<string> ids)
        {
            return Json(new { Sucesso = new BLMultimidiaCategoria().Excluir(ids, PortalAtual.ConnectionString) > 0 });
        }

        #endregion

        #region ValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarNome(string nome, decimal? id)
        {
            try
            {
                var codigo = new BLMultimidiaArquivo().Obter(nome, id);

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

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Arquivo() => View();

        #endregion

        #region ArquivoItem

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ArquivoItem(MLMultimidiaArquivo model)
        {
            var portal = PortalAtual.Obter;

            model = new BLMultimidiaArquivo().Obter(model, portal.ConnectionString) ?? new MLMultimidiaArquivo() { CodigoCategoria = model.CodigoCategoria };
            model.Descricao.Unescape();
            model.CodigoIdioma = model.CodigoIdioma.GetValueOrDefault(PortalAtual.Obter.CodigoIdioma.Value);

            ViewBag.Tipos = new BLMultimidiaTipo().Listar(new MLMultimidiaTipo(), portal.ConnectionString);

            return View(model);
        }

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ArquivoItem(MLMultimidiaArquivo model, bool? IsRemoverCapa)
        {         
            if (ModelState.IsValid)
                new BLMultimidiaArquivo().Salvar(ref model, PortalAtual.ConnectionString);

            return Redirect("/cms/" + PortalAtual.Diretorio + "/multimidiaadmin/arquivo?codigocategoria=" + model.CodigoCategoria);
        }

        #endregion

        #region ListarArquivos

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult ListarArquivos(MLMultimidiaArquivo criterios)
        {
            criterios.CodigoPortal = PortalAtual.Codigo;
            return DataTable.Listar(criterios, Request.QueryString);
        }
        #endregion

        #region ExcluirArquivos

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        [JsonHandleError]
        public ActionResult ExcluirArquivos(List<string> ids)
        {
            new BLMultimidiaArquivo().Excluir(ids, PortalAtual.ConnectionString);
            return Json(new { Sucesso = true });
        }
        #endregion

        #region UploadFile

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult UploadFile(MLMultimidiaArquivo model, HttpPostedFileBase file)
        {
            string caminhoArquivo = string.Empty;
            caminhoArquivo = new BLMultimidiaArquivo().UploadFile(model);
            return Json(new { Sucesso = true, name = Path.GetFileName(caminhoArquivo) });
        }
        #endregion

        #region DownloadFile

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public FileResult DownloadFile()
        {
            var file = Server.MapPath("~/Portal/alternativo/arquivos/787678.jpg.resource");
            return File(file, "application/octet-stream", "1.jpg");

        }
        #endregion
    }
}
