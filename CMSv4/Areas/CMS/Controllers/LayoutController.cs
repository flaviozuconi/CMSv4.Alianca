using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class LayoutController : SecurePortalController
    {
        //
        // GET: /CMS/Layout
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            ViewBag.ListaLayouts = BLLayout.ListarArquivos(PortalAtual.Obter);
            return View();
        }

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
        public ActionResult Item(string id)
        {
            var model = BLLayout.Obter(id);           
            return View(model);
        }

        
        [HttpPost]
        [ValidateInput(false)]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Item(MLLayout model, HttpPostedFileBase Imagem, string NomeAnterior)
        {
            TempData["Salvo"] = BLLayout.Salvar(model,Imagem, NomeAnterior) > 0;
            ViewBag.ListaLayouts = BLLayout.ListarArquivos(PortalAtual.Obter);
            
            return RedirectToAction("Index");
        }

        #endregion

        #region Excluir

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Excluir(List<string> ids)
        {
            BLLayout.Excluir(ids);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region Historico

        /// <summary>
        /// Copiar o conteúdo do histórico selecionado para a versão atual
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult RecuperarHistorico(decimal id)
        {
            TempData["salvo"] = BLLayoutHistorico.RecuperarHistorico(id);

            return Json(new { Sucesso = true });
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult VisualizarHistorico(decimal id)
        {
            var modelHistorico = new BLLayoutHistorico().Obter(id);

            return Json(new {
                Sucesso = true,
                modelHistorico.Conteudo,
                modelHistorico.Codigo,
                DataCadastro = modelHistorico.DataCadastro.HasValue ? modelHistorico.DataCadastro.Value.ToString("dd/MM/yyyy HH:mm") : "" }
            );
        }

        #endregion

        #region ListarHistorico

        [DataTableHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult ListarHistorico(string NomeLayout)
        {
            double outTotal;
            var lista = new BLLayoutHistorico().Listar(new MLLayout() { Nome = NomeLayout, CodigoPortal = PortalAtual.Codigo }, Request.QueryString, out outTotal, PortalAtual.ConnectionString);

            return new DataTableResult()
            {
                Data = lista,
                TotalRows = (int)outTotal
            };
        }

        #endregion
    }
}
