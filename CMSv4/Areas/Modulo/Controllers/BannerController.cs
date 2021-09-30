using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class BannerController : ModuloBaseController<MLModuloBannerEdicao, MLModuloBannerHistorico, MLModuloBannerPublicado>
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

                // Visualizar Publicado
                MLModuloBanner model = CRUD.Obter<MLModuloBannerPublicado>(new MLModuloBannerPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

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
                var model = new MLModuloBanner();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloBannerEdicao>(new MLModuloBannerEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloBanner { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloBannerHistorico>(new MLModuloBannerHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloBannerPublicado>(new MLModuloBannerPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloBannerPublicado();

                if (!model.CodigoBanner.HasValue)
                {
                    var objMLBanner = new CRUD.Select<MLBanner>()
                                        .Equals(a => a.Ativo, true)
                                        .OrderByDesc(a => a.Codigo)
                                        .Top(1)
                                        .First(portal.ConnectionString);

                    model.CodigoBanner = objMLBanner.Codigo;
                    model.View = "Rotator";
                    model.ApresentarDescricao = true;
                    model.ApresentarSetas = true;
                    model.Autoplay = true;
                    model.ApresentarIndice = true;
                    model.Redimensionar = true;
                    model.Tempo = 5;

                    //Configuração Default
                    CRUD.Salvar
                        (
                            new MLModuloBannerEdicao() 
                            {
                                CodigoPagina = codigoPagina,
                                CodigoBanner = model.CodigoBanner, 
                                Repositorio = model.Repositorio, 
                                View = model.View,
                                DataRegistro = DateTime.Now,
                                ApresentarDescricao = model.ApresentarDescricao,
                                ApresentarSetas = model.ApresentarSetas,
                                Autoplay = model.Autoplay,
                                ApresentarIndice = model.ApresentarIndice,
                                Redimensionar = model.Redimensionar,
                                CodigoUsuario = BLUsuario.ObterLogado().Codigo,
                                Tempo = model.Tempo
                            }, 
                            BLPortal.Atual.ConnectionString
                        );
                }
               
                ViewData["controller"] = "banner";
                ViewData["action"] = "Index";

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

                var model = CRUD.Obter<MLModuloBannerEdicao>(new MLModuloBannerEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloBannerEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                var pagina = CRUD.Obter<MLPagina>(codigoPagina.GetValueOrDefault());

                ViewData["banners"] = CRUD.Listar(new MLBanner() { CodigoPortal = portal.Codigo, Ativo = true, CodigoIdioma = pagina?.CodigoIdioma }, portal.ConnectionString);
                ViewData["views"] = ListarViews();

                return View("Editar", model);
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
        public override ActionResult Editar(MLModuloBannerEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                CRUD.Salvar(model, PortalAtual.ConnectionString);
                MLBanner objMLBanner = CRUD.Obter<MLBanner>(new MLBanner { Codigo = model.CodigoBanner }, BLPortal.Atual.ConnectionString);

                return Json(new { success = true, bannerAutoPlay = model.Autoplay.ToString().ToLower(), bannerTempo = (objMLBanner.TempoPadrao ?? 5) * 1000, repositorio = model.Repositorio });
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

                CRUD.Excluir<MLModuloBannerEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

     
        #region ListarViews
        /// <summary>
        /// Lista as Views conforme a configuração do Módulo
        /// </summary>        
        private List<string> ListarViews()
        {
            var views = new List<string>();

            views.Add("Rotator");
            views.Add("Lateral");
            views.Add("Home");
            views.Add("RotatorHome");
            views.Add("RotatorHomeContador");
            views.Add("RotatorHomeSemTitulo");
            views.Add("HomeFull");
          
            return views;
        }
        #endregion

        #region Lista
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListarPublico(MLModuloBanner model)
        {
            try
            {
                if (model == null) model = new MLModuloBanner();

                List<MLBannerArquivoPublico> lstArquivos = BLModuloBanner.ListarPublico(new MLModuloBanner { CodigoBanner = model.CodigoBanner });
                model.Banner = CRUD.Obter<MLBanner>(new MLBanner { Codigo = model.CodigoBanner }, BLPortal.Atual.ConnectionString);

                if (model.Banner == null)
                    model.Banner = new MLBanner();

                if (lstArquivos.Count > 0)
                    lstArquivos[0].Class = "active";

                double minutos = Convert.ToDouble(model.Banner.TempoPadrao ?? 5);
                model.Tempo = (int)TimeSpan.FromSeconds(minutos).TotalMilliseconds;
                
                ViewData["lstArquivos"] = lstArquivos ?? new List<MLBannerArquivoPublico>();

                return View(model.View, model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion
    }
}
