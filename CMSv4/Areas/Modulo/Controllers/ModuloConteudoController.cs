using Framework.Utilities;
using System;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.Model.Conteudo;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ModuloConteudoController : ModuloBaseController<MLModuloConteudoEdicao, MLModuloConteudoHistorico, MLModuloConteudoPublicado>
    {
        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloConteudoPublicado>(new MLModuloConteudoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                if (model == null) model = new MLModuloConteudoPublicado();

                var conteudo = string.Empty;

                if (!string.IsNullOrEmpty(model.Chave))
                {
                    var modelconteudo = CRUD.Obter<MLConteudoPublicado>(new MLConteudoPublicado { Chave = model.Chave }, portal.ConnectionString);
                    if (modelconteudo != null) conteudo = modelconteudo.Conteudo;
                }

                if (!string.IsNullOrEmpty(conteudo))
                    conteudo = Microsoft.JScript.GlobalObject.unescape(conteudo);

                ViewData["conteudo"] = conteudo;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Visualizar

        /// <summary>
        /// Área de Construção
        /// </summary>
        public override ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = BLPortal.Atual;
                var model = new MLModuloConteudo();
                var conteudo = string.Empty;
                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloConteudoEdicao>(new MLModuloConteudoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloConteudoHistorico>(new MLModuloConteudoHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloConteudoPublicado>(new MLModuloConteudoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloConteudo();

                if (!string.IsNullOrEmpty(model.Chave))
                {
                   var modelconteudo = CRUD.Obter<MLConteudoPublicado>(new MLConteudoPublicado { Chave = model.Chave }, portal.ConnectionString);
                   if (modelconteudo != null) conteudo = modelconteudo.Conteudo;
                }

                if(!string.IsNullOrEmpty(conteudo))
                    conteudo = Microsoft.JScript.GlobalObject.unescape(conteudo);

                ViewData["conteudo"] = conteudo;

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Edição

        /// <summary>
        /// Editar
        /// </summary>
        public override ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLModuloConteudoEdicao>(new MLModuloConteudoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloConteudoEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloConteudoEdicao model)
        {
            try
            {
                model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;

                CRUD.Salvar(model, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir
        /// </summary>
        public override ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                CRUD.Excluir<MLModuloConteudoEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Script

        /// <summary>
        /// Excluir
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloConteudo model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ListarConteudosPublicados

        /// <summary>
        /// Listar Conteudos Publicados
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarConteudosPublicados(string filtro)
        {
            try
            {
                var parametro = new MLConteudoPublicado() { Chave = filtro, CodigoIdioma = BLIdioma.Atual.Codigo };
                var lista = CRUD.Listar<MLConteudoPublicado>(parametro, null, "Chave", String.Empty, BLPortal.Atual.ConnectionString, true);
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion
    }
}
