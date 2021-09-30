using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
namespace CMSApp.Areas.Modulo.Controllers
{
    public class AgrupadorController : ModuloBaseController<MLModuloAgrupadorEdicao, MLModuloAgrupadorHistorico, MLModuloAgrupadorPublicado>
    {
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
                var model = CRUD.Obter(new MLModuloAgrupadorEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloAgrupadorEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                var agrupadores = CRUD.Listar(new MLAgrupador { CodigoPortal = portal.Codigo }, portal.ConnectionString);

                ViewData["categorias-todas"] = CRUD.Listar(new MLCategoriaAgrupador(), portal.ConnectionString);

                if (!string.IsNullOrEmpty(model.Categorias))
                    ViewData["categorias-selecionadas"] = model.Categorias.Split(',').ToList();

                if (!string.IsNullOrEmpty(model.Modulos))
                    ViewData["modulos-selecionados"] = model.Modulos.Split(',').ToList();

                MLAgrupador agrupador = null;
                if (agrupadores.Count > 0)
                {
                    if (model.CodigoAgrupador.HasValue)
                        agrupador = agrupadores.Find(o => o.Codigo == model.CodigoAgrupador);
                    else
                        agrupador = agrupadores[0];
                }
                if (agrupador == null)
                    agrupador = new MLAgrupador { Config = string.Empty };


                ViewData["agrupadores"] = agrupadores;
                ViewData["views"] = ListarViews(agrupador.Config);

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
        public override ActionResult Editar(MLModuloAgrupadorEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;

                if (Request["Categorias"] != null)
                    model.Categorias = Request["Categorias"].Replace("multiselect-all", "").TrimStart(',');
                else
                    model.Categorias = string.Empty;

                if (Request["Modulos"] != null)
                    model.Modulos = Request["Modulos"].Replace("multiselect-all", "").TrimStart(',');
                else
                    model.Modulos = string.Empty;

                if (Request["CodigoCategoriaAgrupador"] != null && !string.IsNullOrWhiteSpace(Request["CodigoCategoriaAgrupador"]))
                    model.CodigoCategoriaAgrupador = Convert.ToDecimal(Request["CodigoCategoriaAgrupador"].Replace("multiselect-all", "").TrimStart(','));
                else
                    model.CodigoCategoriaAgrupador = null;

                model.Destaques = Convert.ToString(Request.Form["Destaques"]) == "on";

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

        #region Combos
        /// <summary>
        /// Popula os combos de views e categoria conforme a configuração da lista
        /// </summary>                
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar, "/cms/{0}/pagina")]
        public ActionResult Combos(decimal idagrupador)
        {

            try
            {
                var config = CRUD.Obter<MLAgrupador>(idagrupador, PortalAtual.ConnectionString);
                var views = ListarViews(config.Config);

                var categorias = CRUD.Listar(new MLCategoriaAgrupador(), PortalAtual.ConnectionString);

                var json = Json(new { success = true, categorias = categorias, views = views });
                return json;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = "Erro: " + ex.Message });
            }


        }
        #endregion

        #region ListarViews
        /// <summary>
        /// Lista as Views conforme a configuração do Módulo
        /// </summary>        
        private List<string> ListarViews(string config)
        {
            var jsonConfig = System.Web.Helpers.Json.Decode(config);
            var views = new List<string>();
            if (jsonConfig != null && jsonConfig.views != null)
            {
                for (var i = 0; i < jsonConfig.views.Length; i++)
                {
                    views.Add(jsonConfig.views[i].nome);
                }
            }
            if (views.Count == 0)
            {
                views.Add("lista");
                views.Add("destaque");
                views.Add("detalhe");
            }

            return views;
        }
        #endregion

        #region BuscarController
        /// <summary>
        /// Lista as Views conforme a configuração do Módulo
        /// </summary>        
        private string[] BuscarActionController(decimal idlista, string nomeview, MLPortal portal)
        {
            var ret = new string[] { "lista", "lista" };
            try
            {
                var config = CRUD.Obter<MLListaConfig>(idlista, portal.ConnectionString);

                if (config != null && !string.IsNullOrEmpty(config.Configuracao))
                {
                    var jsonConfig = System.Web.Helpers.Json.Decode(config.Configuracao);

                    if (jsonConfig.views != null)
                    {
                        for (var i = 0; i < jsonConfig.views.Length; i++)
                        {
                            if (nomeview.ToLower() == Convert.ToString(jsonConfig.views[i].nome).ToLower())
                            {
                                if (jsonConfig.views[i].action != null)
                                    ret[0] = jsonConfig.views[i].action;
                                else
                                    ret[0] = nomeview;

                                if (jsonConfig.views[i].controller != null)
                                    ret[1] = jsonConfig.views[i].controller;

                                return ret;
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            return ret;


        }
        #endregion

        //Publicos
        #region Lista
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Lista(MLModuloAgrupador model)
        {
            try
            {
                var retorno = new MLAgrupadorPublico();
                var portal = BLPortal.Atual;
                var idioma = BLIdioma.Atual.Codigo.GetValueOrDefault();
                if (model == null) model = new MLModuloAgrupador();
                if (!model.Quantidade.HasValue) model.Quantidade = 5;

                var config = CRUD.Obter<MLAgrupador>(model.CodigoAgrupador ?? 0, BLPortal.Atual.ConnectionString);
                var listaslinks = ListarListasLink(config.Config ?? "");

                var anos = BLAgrupador.ListarPublicoAnos(model, idioma);

                if (anos.Count == 0)
                    anos.Add(new MLAgrupadorAnos { Ano = DateTime.Today.Year });

                int anoatual;
                int.TryParse(Convert.ToString(RouteData.Values["extra1"]), out anoatual);

                anoatual = anoatual > 0 ? anoatual : anos[0].Ano;

                retorno = BLAgrupador.ListarPublico(model, anoatual, idioma);

                ViewData["anos"] = anos;
                ViewData["anoatual"] = anoatual;
                ViewData["listaslinks"] = listaslinks;
                ViewData["urlatual"] = Convert.ToString(RouteData.Values["url"]);
                
                ViewData["modulo"] = model;

                return View(model.NomeView ?? "lista", retorno);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region ListarListasLink
        /// <summary>
        /// Lista as Views conforme a configuração do Módulo
        /// </summary>        
        private List<decimal> ListarListasLink(string config)
        {
            var jsonConfig = System.Web.Helpers.Json.Decode(config);
            var links = new List<decimal>();
            if (jsonConfig != null && jsonConfig.links != null)
            {
                for (var i = 0; i < jsonConfig.links.Length; i++)
                {
                    links.Add(jsonConfig.links[i]);
                }
            }

            return links;
        }
        #endregion
    }
}
