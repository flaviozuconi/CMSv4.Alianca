using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class BuscaController : ModuloBaseController<MLModuloBuscaEdicao, MLModuloBuscaHistorico, MLModuloBuscaPublicado>
    {
        #region Busca

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Busca(MLModuloBusca model)
        {
            return View(model);
        }

        #endregion

        #region BuscaAutoComplete

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult BuscaAutoComplete(string strTermo, string strExibirImagem)
        {
            if (strTermo.Length >= 3)
            {
                const int qtde = 5;
                int pagina = 1;
                bool IsExibirImagem = false;

                try
                {
                    StringBuilder sbResultado = new StringBuilder();
                    List<MLBuscaResultado> lstML = BLModuloBusca.ListarPublico(qtde, pagina, strTermo);
                    Boolean.TryParse(strExibirImagem, out IsExibirImagem);

                    if (lstML.Count > 0)
                    {
                        sbResultado.Append("<ul class='autocomplete'>");

                        foreach (MLBuscaResultado item in lstML)
                        {
                            var strItem =
                                "<li>";

                            if(item.Chamada.Length > 200)
                                item.Chamada = item.Chamada.Substring(0,200) + "...";

                            if (IsExibirImagem && !String.IsNullOrEmpty(item.Imagem))
                                strItem += "<a href='" + item.Url + "' target='_top'><img alt='' src='" + item.Imagem + "' /></a>";

                            strItem += "<a href='" + item.Url + "' target='_top'><span>" + item.Titulo + "</span></a>" +
                                       "<a href='" + item.Url + "' target='_top'><span>" + item.Chamada + "</span></a>" +
                                 "</li>";

                            sbResultado.Append(strItem);
                        }

                        sbResultado.Append("</ul>");
                    }

                    return Json(new { success = true, html = sbResultado.ToString() });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, html = "" });
                }
            }

            return Json(new { success = false, html = "" });
        }

        [HttpGet, CheckPermission(global::Permissao.Publico)]
        public JsonResult AutoComplete(string termo)
        {
            if (!string.IsNullOrEmpty(termo) && termo.Length >= 1)
            {
                try
                {
                    var urlPesquisa = string.Concat("/", BLPortal.Atual.Diretorio, BLModuloBusca.ObterUrlResultadoBusca());

                    StringBuilder sbResultado = new StringBuilder();
                    var lstML = BLModuloBusca.ListarPublico(termo);

                    for (int i = 0; i < lstML.Count; i++)
                    {
                        sbResultado.Append(string.Format("<li><a href='{0}?q={1}' target='_top'><span>{1}</span></a></li>", urlPesquisa, lstML[i]));
                    }

                    return Json(new { success = true, html = sbResultado.ToString() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, html = "" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = true, html = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult BuscaAutoCompleteView(string strTermo, string view, int? quantidade)
        {
            List<MLBuscaResultado> resultadoProduto = new List<MLBuscaResultado>();

            if (strTermo.Length >= 3)
            {
                quantidade = quantidade.HasValue ? quantidade : 20;
                int pagina = 1;

                try
                {
                    StringBuilder sbResultado = new StringBuilder();
                    List<MLBuscaResultado> lista = BLModuloBusca.ListarPublico(quantidade.Value, pagina, strTermo);

                    return Json(new { success = true, html = BLConteudoHelper.RenderViewToString(this, view, String.Empty, lista) }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, html = "" });
                }
            }

            return Json(new { success = false, html = "" });
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

                var model = CRUD.Obter<MLModuloBuscaEdicao>(new MLModuloBuscaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloBuscaEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

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
        public override ActionResult Editar(MLModuloBuscaEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                return Json(new { success = true, });
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

                CRUD.Excluir<MLModuloBuscaEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

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
                MLModuloBusca model = CRUD.Obter<MLModuloBuscaPublicado>(new MLModuloBuscaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
                return View("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region ListarPublico
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListarPublico(MLModuloBusca model)
        {
            try
            {
                if (model == null || String.IsNullOrEmpty(model.View))
                {
                    if (model == null)
                    {
                        model = new MLModuloBusca();
                    }

                    model.View = "Busca";
                }
                //else
                //{
                //    if (model.View == "Resultado" && Request.QueryString["busca"] == null && BLPortal.Atual.Codigo != 2 /* RI NÃO TEM PRODUTO */) /*Abrir primeiramente a página de resumo da pesquisa*/
                //    {
                //        model.View = "ResultadoResumo";
                //    }
                //    else
                //    {
                //        model.View = "Resultado";
                //    }
                //}

                if (model.View != "Busca")
                {
                    ViewData["lstResultados"] = ResultadoBusca(model.QtdePorPagina ?? 10, resumo: model.View == "ResultadoResumo");
                }

                return View(model.View, model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region ListarPublicoAjax
        /// <summary>
        /// Action da lista
        /// </summary>        
        [HttpGet, CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarPublicoAjax(string q, int? t, int? p)
        {
            try
            {
                //q = query
                //p = paginacao
                //t = total por pagina
                var lista = ResultadoBusca(t ?? 10, q, p);

                return Json(new { success = true, html = BLConteudoHelper.RenderViewToString(this, "ResultadoAjax", String.Empty, lista) }, JsonRequestBehavior.AllowGet);
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

            views.Add("Busca");
            views.Add("Resultado");
            views.Add("ResultadoResumo");

            return views;
        }
        #endregion

        #region ResponseResult

        public ActionResult ResponseResult(string url)
        {
            Response.Redirect(url, true);
            return null;
        }

        #endregion

        #region ResultadoBusca

        [CheckPermission(global::Permissao.Publico)]
        private List<MLBuscaResultado> ResultadoBusca(int intQtdePorPagina, string termo = "", int? paginacalAtual = null, bool resumo = false)
        {
            int pagina = 1;
            List<MLBuscaResultado> lstML = null;

            if (!string.IsNullOrEmpty(Request.QueryString.ToString()) && !string.IsNullOrEmpty(Request.QueryString.ToString().Split('=')[1]))
                termo = Request.QueryString.ToString().Split('=')[1].HtmlUnescapeDecode();
            else
                termo = termo.HtmlUnescapeDecode();

            if (termo.Length < 3)
            {
                ViewBag.Mensagem = T("Informar acima de 3 caracteres para pesquisa");

                return new List<MLBuscaResultado>();
            }

            termo = termo.Replace("+", " ");
            
            ViewBag.Query = Request.Url.Query;

            if (resumo)
            {
                lstML = BLModuloBusca.ListarResumo(intQtdePorPagina, termo);

                var lstResultadosSite = lstML.FindAll(a => !a.Tipo.Equals("PRO", StringComparison.InvariantCultureIgnoreCase));
                var lstResultadosProduto = lstML.FindAll(a => a.Tipo.Equals("PRO", StringComparison.InvariantCultureIgnoreCase));

                ViewBag.ResultadosSite = lstResultadosSite;
                ViewBag.ResultadosProduto = lstResultadosProduto;

                ViewBag.UrlProdutos = BLModuloBusca.ObterUrl(28);
                ViewBag.TotalSite = lstResultadosSite.Count > 0 ? lstResultadosSite[0].TotalRows : 0;
                ViewBag.TotalProdutos = lstResultadosProduto.Count > 0 ? lstResultadosProduto[0].TotalRows : 0;
            }
            else
            {
                if (!paginacalAtual.HasValue && RouteData.Values["extra1"] != null)
                {
                    int.TryParse(RouteData.Values["extra1"].ToString(), out pagina);
                }

                lstML = BLModuloBusca.ListarPublico(intQtdePorPagina, paginacalAtual ?? pagina, termo);

                double? totalRows = lstML.Count > 0 ? lstML[0].TotalRows : null;

                ViewBag.TotalRows = totalRows.GetValueOrDefault(0);
                ViewBag.TotalPaginas = Math.Ceiling((double)(totalRows.GetValueOrDefault(1)) / (intQtdePorPagina));
                ViewBag.PaginaAtual = pagina;
            }
            ViewBag.Url = string.Concat(Portal.Url(), Request.Url.AbsolutePath.Replace(pagina.ToString(), "")).TrimEnd('/');

            return lstML;
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloBusca model)
        {
            try
            {
                if (model.IsAutoComplete.GetValueOrDefault())
                    return View("ScriptAutoComplete", model);
                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptResultadoBusca(MLModuloBusca model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
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
                var model = new MLModuloBusca();
                MLModuloBusca objMLBusca = new MLModuloBusca();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloBuscaEdicao>(new MLModuloBuscaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloBusca { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloBuscaHistorico>(new MLModuloBuscaHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloBuscaPublicado>(new MLModuloBuscaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloBuscaPublicado();

                if (String.IsNullOrEmpty(model.View))
                {
                    MLModuloBuscaEdicao objModel = new MLModuloBuscaEdicao();
                    objModel.View = "Busca";
                    objModel.Repositorio = repositorio;
                    objModel.CodigoPagina = codigoPagina;
                    objModel.TextoPlaceHolder = "Busca...";
                    objModel.QtdePorPagina = 15;
                    objModel.ExibirImagem = true;
                    objModel.IsAutoComplete = true;
                    objModel.DataRegistro = DateTime.Now;
                    objModel.CodigoUsuario = BLUsuario.ObterLogado().Codigo;

                    //Configuração Default
                    CRUD.Salvar(objModel, BLPortal.Atual.ConnectionString);
                }

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region FormularioPesquisa
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult FormularioPesquisa(bool? adicionarAutoComplete = false)
        {
            return View(adicionarAutoComplete);
        }
        #endregion
    }
}
