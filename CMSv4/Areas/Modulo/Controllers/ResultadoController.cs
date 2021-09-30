using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ResultadoController : ModuloBaseController<MLModuloResultadoEdicao, MLModuloResultadoHistorico, MLModuloResultadoPublicado>
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

                // Visualizar Publicado
                MLModuloResultado model = CRUD.Obter<MLModuloResultadoPublicado>(new MLModuloResultadoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
               
                if (model == null)
                {
                    model = new MLModuloResultado();
                    var modulopagina = CRUD.Obter<MLPaginaModuloPublicado>(new MLPaginaModuloPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    model = new MLModuloResultado { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }

                //setar idioma
                var idioma = BLIdioma.Atual.Codigo.GetValueOrDefault();

                ViewData["listaAnos"] = BLResultado.ListarAnos(true, idioma);

                var nomeview = string.IsNullOrEmpty(model.View) ? "lista" : model.View;
                var actioncontroller = "Resultado";
                ViewData["action"] = nomeview;
                ViewData["controller"] = actioncontroller;
                ViewData["ano_selecionado"] = BLCachePortal.Get<int>("resultado_ano");

                return PartialView("Index", model);
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
                var model = new MLModuloResultado();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloResultadoEdicao>(new MLModuloResultadoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null)
                    {
                        var modulopagina = CRUD.Obter<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                        decimal? codigoLista = null;
                        if (modulopagina != null && modulopagina.CodigoModulo.HasValue)
                            codigoLista = CRUD.Obter<MLModulo>(modulopagina.CodigoModulo.Value, portal.ConnectionString).CodigoLista;

                        model = new MLModuloResultado { CodigoPagina = codigoPagina, Repositorio = repositorio };

                    }
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloResultadoHistorico>(new MLModuloResultadoHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloResultadoPublicado>(new MLModuloResultadoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloResultadoPublicado();

                var CodigoIdioma = BLIdioma.Atual.Codigo.GetValueOrDefault();

                var nomeview = string.IsNullOrEmpty(model.View) ? "lista" : model.View;
                ViewData["listaAnos"] = BLResultado.ListarAnos(true,CodigoIdioma);
        
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

                var model = CRUD.Obter<MLModuloResultadoEdicao>(new MLModuloResultadoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloResultadoEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                var modulopagina = CRUD.Obter<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);


                ViewData["views"] = new List<string>() { "Lista", "Detalhe" };

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
        public override ActionResult Editar(MLModuloResultadoEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                
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

        //Publicos

        #region Lista
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Lista(MLModuloResultado model)
        {
            try
            {
                var connString = BLPortal.Atual.ConnectionString;
                
                var Ano = DateTime.Now.Year;
                decimal CodigoIdioma;
                try
                {
                    CodigoIdioma = Convert.ToInt32(Request.QueryString["CodigoIdioma"].ToString());
                    BLIdioma.Atual = CRUD.Obter<MLIdioma>(CodigoIdioma, connString);
                }
                catch
                {
                    CodigoIdioma = BLIdioma.Atual.Codigo.GetValueOrDefault();
                }
                               

                try
                {
                    Ano = Convert.ToInt32(Request.QueryString["ano"].ToString());                    
                }
                catch
                {
                    if (BLCachePortal.Get<int>("resultado_ano") != 0)
                    {
                        Ano = BLCachePortal.Get<int>("resultado_ano");
                    }
                    else
                    {
                        var listaAnos = BLResultado.ListarAnos(true, CodigoIdioma).OrderByDescending(x => x.Ano).ToList();
                        if (listaAnos != null && listaAnos.Count > 0)
                            Ano = (int)listaAnos[0].Ano.Value;
                    }
                }

                
                    
                
                if (model == null) model = new MLModuloResultado();

              

                var lista = CRUD.Listar<MLResultado>(new MLResultado() { Ano = Ano, Idioma = CodigoIdioma }, "Trimestre", "ASC", connString);
                

                ViewData["modulo"] = model;

                BLCachePortal.Remove("resultado_ano");
                BLCachePortal.Add("resultado_ano", Ano);  
                
                return View(lista);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Detalhe(decimal id)
        {
            try
            {
                var portal = BLPortal.Atual;
                var conteudo = CRUD.Obter<MLResultado>(id, portal.ConnectionString);

                //Tratar erro de referência de resultado, pesquisa no banco por ano, trimestre e idioma
                if (conteudo.Idioma != BLIdioma.CodigoAtual)
                {
                    var conteudoIdioma = CRUD.Obter<MLResultado>(new MLResultado
                    {
                        Trimestre = conteudo.Trimestre,
                        Ano = conteudo.Ano,
                        Idioma = BLIdioma.CodigoAtual
                    }, portal.ConnectionString);

                    if (conteudoIdioma != null)
                        return View(conteudoIdioma);
                }

                return View(conteudo);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
    }


}
