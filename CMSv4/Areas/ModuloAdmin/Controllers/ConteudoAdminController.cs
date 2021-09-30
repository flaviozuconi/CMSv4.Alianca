using Framework.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSv4.BusinessLayer;
using CMSv4.Model.Conteudo;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ConteudoAdminController : AdminBaseCRUDController<MLConteudoEdicao, MLConteudoEdicao>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            return View(new BLConteudo().Obter(id.GetValueOrDefault(0)));
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Item(MLConteudoEdicao model)
        {
            TempData["Salvo"] = new BLConteudo().Salvar(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion

        #region Excluir

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public override ActionResult Excluir(List<string> ids)
        {;
            return Json(new { Sucesso = new BLConteudo().Excluir(ids) > 0 });
        }

        #endregion

        #region Histórico

        #region Historico

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult RecuperarHistorico(decimal id)
        {
            new BLConteudoHistorico().Recuperar(id);
            return Json(new { Sucesso = true });
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult VisualizarHistorico(decimal id)
        {
            var modelHistorico = new BLConteudoHistorico().Obter(id);
            return Json(new { Sucesso = true, modelHistorico.Chave, modelHistorico.Conteudo, modelHistorico.Codigo, DataCadastro = modelHistorico.DataCadastro.HasValue ? modelHistorico.DataCadastro.Value.ToString("dd/MM/yyyy HH:mm") : "" });   
        }

        #endregion

        #region ListarHistorico

        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult ListarHistorico(decimal id)
        {
            return DataTable.Listar(new MLConteudoHistorico() { CodigoConteudo = id }, Request.QueryString, PortalAtual.ConnectionString);
        }

        #endregion

        #endregion
    }
}