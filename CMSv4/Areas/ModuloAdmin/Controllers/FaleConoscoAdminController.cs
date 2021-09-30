using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.Xml;
using System.Linq;
using System.Collections.Specialized;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class FaleConoscoAdminController : SecurePortalController
    {

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index() => View();

        #region Modelo Email

        #region Listar

        [CheckPermission(global::Permissao.Visualizar), DataTableHandleError]
        public ActionResult Listar(MLFaleConoscoModeloEmail criterios)
        {
            criterios.CodigoPortal = PortalAtual.Codigo;
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        #endregion

        #region ModeloEmail

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ModeloEmail() => View("ModeloEmail");

        #endregion

        #region ModeloEmailExcluir


        [HttpPost, JsonHandleError, CheckPermission(global::Permissao.Excluir)]
        public ActionResult ModeloEmailExcluir(List<string> ids)
        {
            new BLFaleConoscoModeloEmail().Excluir(ids, BLPortal.Atual.ConnectionString);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region ModeloItem

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult ModeloEmailItem(decimal? id)
        {        
            var model = new BLFaleConoscoModeloEmail().Obter(id.GetValueOrDefault(), BLPortal.Atual.ConnectionString);
            return View("ModeloEmailItem", model);
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "ModeloEmail")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModeloEmailItem(MLFaleConoscoModeloEmail model, string nomeAntigo)
        {
            TempData["Salvo"] = new BLFaleConoscoModeloEmail().Salvar(model, nomeAntigo, BLPortal.Atual.ConnectionString) > 0;
            return RedirectToAction("ModeloEmail");
        }

        #endregion

        #region ModeloEmailValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult ModeloEmailValidarNome(decimal? codigo, string nome)
        {
            var portal = BLPortal.Atual;
            var model = new BLFaleConoscoModeloEmail().Obter(new MLFaleConoscoModeloEmail() { Nome = nome, CodigoPortal = portal.Codigo }, portal.ConnectionString);

            if (model != null && model.Codigo.HasValue && ((codigo.HasValue && codigo.Value != model.Codigo) || !codigo.HasValue))
                return Json("Já existe um arquivo com esse nome.");

            return Json(true);
        }

        #endregion

        #endregion

        #region Formularios

        public ActionResult Formularios() => View("Formularios");

        #region FormulariosItem

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult FormulariosItem(decimal? id)
        {
            return View(new BLFaleConoscoFormulario().Obter(id.GetValueOrDefault(), BLPortal.Atual.ConnectionString));
        }


        [HttpPost]
        [ValidateInput(false)]
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "Formularios")]
        public ActionResult FormulariosItem(MLFaleConoscoFormulario model, string nomeAntigo)
        {
            TempData["Salvo"] = new BLFaleConoscoFormulario().Salvar(model, nomeAntigo, BLPortal.Atual.ConnectionString) > 0;
            return RedirectToAction("Formularios");
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar), DataTableHandleError]
        public ActionResult ListarFormularios(MLFaleConoscoFormulario criterios)
        {
            
            criterios.CodigoPortal = PortalAtual.Codigo;
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        #endregion

        #region FormulariosExcluir


        [HttpPost]
        [CheckPermission(global::Permissao.Excluir), JsonHandleError]
        public ActionResult FormulariosExcluir(List<string> ids)
        {
            new BLFaleConoscoFormulario().Excluir(ids, BLPortal.Atual.ConnectionString);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region FormularioslValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult FormularioslValidarNome(decimal? codigo, string nome)
        {
            var portal = BLPortal.Atual;
            var model = CRUD.Obter(new MLFaleConoscoFormulario() { Nome = nome, CodigoPortal = portal.Codigo }, portal.ConnectionString);

            if (model != null && model.Codigo.HasValue && ((codigo.HasValue && codigo.Value != model.Codigo) || !codigo.HasValue))
                return Json(T("Já existe um arquivo com esse nome."));

            return Json(true);
        }

        #endregion

        #endregion

        #region Mensagens

        #region Mensagens

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Mensagens() => View();

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar), JsonHandleError]
        public ActionResult ListarMensagens(MLFaleConoscoPadrao criterios)
        {
            TempData["QueryString"] = Request.QueryString;
            criterios.CodigoPortal = PortalAtual.Codigo;
            return DataTable.Listar(criterios, Request.QueryString, PortalAtual.ConnectionString);
        }

        #endregion

        #region Excluir

        [CheckPermission(global::Permissao.Excluir), JsonHandleError]
        public ActionResult MensagensExcluir(List<string> ids)
        {
            var faleConosco = new BLFaleConoscoPadrao();
            foreach (string id in ids)
                faleConosco.Excluir(Convert.ToDecimal(id), PortalAtual.ConnectionString);

            return Json(new { Sucesso = true });
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MensagensItem(string id)
        {
            return View("MensagensItem", new BLFaleConoscoPadrao().Obter(Convert.ToDecimal(id), PortalAtual.ConnectionString));
        }

        #endregion

        #endregion

        #region FormulariosItem

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult FormularioConfiguracaiEnvioEmail(decimal? id)
        {
            var model = new BLFaleConoscoConfiguracaoEnvioEmail().Obter(id.GetValueOrDefault(), BLPortal.Atual.ConnectionString) ?? new MLFaleConoscoConfiguracaoEnvioEmail();
            return View("ConfiguracaoEnvioEmail", model);
        }


        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FormularioConfiguracaiEnvioEmail(MLFaleConoscoConfiguracaoEnvioEmail model)
        {
            new BLFaleConoscoConfiguracaoEnvioEmail().Salvar(model, BLPortal.Atual.ConnectionString);
            return RedirectToAction("FormularioConfiguracaiEnvioEmail", new { id = 1 });
        }

        #endregion

        #region Exportar
        /// <summary>
        /// Exportar Mensagens
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public FileResult Exportar()
        {
            NameValueCollection request = (NameValueCollection)TempData["QueryString"];
            TempData["QueryString"] = request;

            var mensagens = CRUD.Listar(new MLFaleConoscoExportar(), request);
            var csvContent = BLUtilitarios.EPPlus.Exportar(mensagens.OrderByDescending(o=>o.DataCadastro).ToList());

            return File(csvContent.ToArray(), "application/vnd.ms-excel;", $"mensagens-{DateTime.Now.ToString()}.xlsx");
        }

        #endregion
    }
}
