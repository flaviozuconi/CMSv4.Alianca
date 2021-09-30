using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    /// <summary>
    /// Módulo HTML
    /// </summary>
    public class HtmlController : ModuloBaseController<MLModuloHtmlEdicao, MLModuloHtmlHistorico, MLModuloHtmlPublicado>
    {

        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>        
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter(new MLModuloHtmlPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
                if (model == null) model = new MLModuloHtmlPublicado();

                if (!string.IsNullOrEmpty(model.Conteudo))
                    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);

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

                var portal = PortalAtual.Obter;
                var model = new MLModuloHtml();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloHtmlEdicao>(new MLModuloHtmlEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloHtml {CodigoPagina = codigoPagina,Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloHtmlHistorico>(new MLModuloHtmlHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloHtmlPublicado>(new MLModuloHtmlPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloHtml();
                
                if (!string.IsNullOrEmpty(model.Conteudo))
                    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);

                ViewData["editavel"] = false;

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
                var model = CRUD.Obter<MLModuloHtmlEdicao>(new MLModuloHtmlEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloHtmlEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;
                if (!string.IsNullOrEmpty(model.Conteudo))
                    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);
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
        public override ActionResult Editar(MLModuloHtmlEdicao model)
        {
            try
            {
                model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;
                model.Titulo = string.Concat("Página ", model.CodigoPagina);
                model.ConteudoBusca = model.Conteudo.HtmlDecode().RemoveHtmlTags();

                if (!string.IsNullOrEmpty(model.Conteudo))                
                    model.Conteudo = Microsoft.JScript.GlobalObject.escape(model.Conteudo);
                
                CRUD.Salvar(model, PortalAtual.ConnectionString);

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

                CRUD.Excluir<MLModuloHtmlEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

    }
}
