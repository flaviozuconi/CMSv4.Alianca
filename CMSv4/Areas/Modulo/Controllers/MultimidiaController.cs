using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class MultimidiaController : ModuloBaseController<MLModuloMultimidiaEdicao, MLModuloMultimidiaHistorico, MLModuloMultimidiaPublicado>
    {
        #region Base

        #region Index
        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloMultimidiaPublicado>(new MLModuloMultimidiaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

                if (model == null || (!model.CodigoPagina.HasValue))
                {
                    model = new MLModuloMultimidiaPublicado()
                    {
                        CodigoPagina = codigoPagina,
                        Repositorio = repositorio
                    };
                }

                if(model.CodigoArquivo.HasValue)
                    ViewBag.Arquivo = new BLMultimidiaArquivo().ObterArquivo(model.CodigoArquivo.Value);

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
                var model = new MLModuloMultimidia();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloMultimidiaEdicao>(new MLModuloMultimidiaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloMultimidia { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloMultimidiaHistorico>(new MLModuloMultimidiaHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloMultimidiaPublicado>(new MLModuloMultimidiaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloMultimidia();

                ViewData["editavel"] = false;

                if (model.CodigoArquivo.HasValue)
                    ViewBag.Arquivo = new BLMultimidiaArquivo().ObterArquivo(model.CodigoArquivo.Value);

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
                var model = CRUD.Obter<MLModuloMultimidiaEdicao>(new MLModuloMultimidiaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloMultimidiaEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                ///PASTAS DISPONÍVEIS
                var lstPastas = CRUD.Listar(new MLMultimidiaCategoria { CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);

                if (model.CodigoArquivo.HasValue)
                {
                    var objArquivo = CRUD.Obter<MLMultimidiaArquivo>(model.CodigoArquivo.Value);
                    ViewBag.Arquivo = objArquivo;
                    ViewBag.Arquivos = CRUD.Listar<MLMultimidiaArquivo>(new MLMultimidiaArquivo() { CodigoCategoria = objArquivo.CodigoCategoria }, portal.ConnectionString);
                }

                ViewData["CategoriaArquivos"] = lstPastas;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #endregion

        #region ComboArquivo
        /// <summary>
        /// Popula os combos de views e categoria conforme a configuração da lista
        /// </summary>                
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, "/cms/{0}/pagina")]
        public ActionResult ComboArquivo(decimal id)
        {

            try
            {
                var categorias = CRUD.Listar(new MLMultimidiaArquivo { CodigoCategoria = id, Ativo = true }, PortalAtual.ConnectionString);

                var json = Json(new { success = true, categorias = categorias });
                return json;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = "Erro: " + ex.Message });
            }


        }
        #endregion

    }
}
