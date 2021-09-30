using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using Newtonsoft.Json;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ListaController : ModuloBaseController<MLModuloListaEdicao, MLModuloListaHistorico, MLModuloListaPublicado>
    {
        #region Avaliar

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult Avaliar(int nota, decimal id, string CodigoLista)
        {
            MLListaConteudoAvaliacao objML = new MLListaConteudoAvaliacao();

            try
            {
                var cliente = BLCliente.ObterLogado();
                if (cliente != null && cliente.Codigo.HasValue) objML.CodigoCliente = cliente.Codigo;

                objML.CodigoConteudo = id;
                objML.DataCadastro = DateTime.Now;
                objML.Nota = nota;
                objML.IP = Request.UserHostAddress;

                MLAvaliacaoRetorno model = BLLista.InserirAvaliacao(objML);

                //Limpar o cache após avaliar, para que no próximo carregando a informações esteja atualizada
                var cacheKey = string.Format("url_{0}_codigolista_{1}", Request.Url.ToString(), CodigoLista);
                BLCachePortal.Remove(cacheKey);

                return Json(new { success = true, QtdeAvaliacoes = model.QtdeAvaliacoes, Media = model.Media });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
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
                var portal = BLPortal.Atual;
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                MLModuloLista model = CRUD.Obter<MLModuloListaPublicado>(new MLModuloListaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                {
                    model = new MLModuloLista();
                    var modulopagina = CRUD.Obter<MLPaginaModuloPublicado>(new MLPaginaModuloPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    decimal? codigoLista = null;
                    if (modulopagina != null && modulopagina.CodigoModulo.HasValue)
                        codigoLista = BLModulo.Obter(modulopagina.CodigoModulo, portal.ConnectionString).CodigoLista;
                    model.CodigoLista = codigoLista;
                }

                var nomeview = string.IsNullOrEmpty(model.NomeView) ? "lista" : model.NomeView;
                var actioncontroller = model.CodigoLista.HasValue ? BuscarActionController(model.CodigoLista.Value, nomeview, BLPortal.Atual) : new string[] { nomeview, "lista" };

                ViewData["action"] = actioncontroller[0];
                ViewData["controller"] = actioncontroller[1];

                //model.UrlDetalhe = string.Empty;

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
                var model = new MLModuloLista();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter(new MLModuloListaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null)
                    {
                        var modulopagina = CRUD.Obter(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                        decimal? codigoLista = null;

                        if (modulopagina != null && modulopagina.CodigoModulo.HasValue)
                            codigoLista = BLModulo.Obter(modulopagina.CodigoModulo, portal.ConnectionString).CodigoLista;

                        model = new MLModuloLista { CodigoPagina = codigoPagina, Repositorio = repositorio, CodigoLista = codigoLista };
                    }
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter(new MLModuloListaHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter(new MLModuloListaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloListaPublicado();

                ViewData["editavel"] = false;
                var nomeview = string.IsNullOrEmpty(model.NomeView) ? "lista" : model.NomeView;
                var actioncontroller = model.CodigoLista.HasValue ? BuscarActionController(model.CodigoLista.Value, nomeview, PortalAtual.Obter) : new string[] { nomeview, "lista" };

                ViewData["action"] = actioncontroller[0];
                ViewData["controller"] = actioncontroller[1];

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

                var model = CRUD.Obter<MLModuloListaEdicao>(new MLModuloListaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloListaEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;


                var modulopagina = CRUD.Obter<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                decimal? codigoLista = null;
                if (modulopagina != null && modulopagina.CodigoModulo.HasValue)
                    codigoLista = BLModulo.Obter(modulopagina.CodigoModulo, portal.ConnectionString).CodigoLista;

                if (codigoLista.HasValue)
                {
                    model.CodigoLista = codigoLista;
                    ViewData["listaconfigurado"] = true;
                }


                ViewData["listas"] = CRUD.Listar(new MLListaConfig { CodigoPortal = portal.Codigo }, portal.ConnectionString);
                var stringconfig = string.Empty;
                if (model.CodigoLista.HasValue)
                {
                    var config = CRUD.Obter<MLListaConfig>(model.CodigoLista.Value, PortalAtual.ConnectionString);
                    stringconfig = config.Configuracao;
                    ViewData["categorias-todas"] = CRUD.Listar(new MLListaConfigCategoria { CodigoLista = model.CodigoLista }, PortalAtual.ConnectionString);
                    if (!string.IsNullOrEmpty(model.Categorias))
                        ViewData["categorias-selecionadas"] = model.Categorias.Split(',').ToList();

                }

                ViewData["views"] = ListarViews(stringconfig);

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
        public override ActionResult Editar(MLModuloListaEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                if (Request["Categorias"] != null)
                    model.Categorias = Request["Categorias"].Replace("multiselect-all", "").TrimStart(',');
                else
                    model.Categorias = string.Empty;


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

                CRUD.Excluir<MLModuloListaEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

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
        public ActionResult Combos(decimal idlista)
        {

            try
            {
                var config = CRUD.Obter<MLListaConfig>(idlista, PortalAtual.ConnectionString);
                var views = ListarViews(config.Configuracao);

                var categorias = CRUD.Listar(new MLListaConfigCategoria { CodigoLista = idlista }, PortalAtual.ConnectionString);

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
                            string jsonNome = jsonConfig.views[i].nome;

                            if (jsonNome.ToLower() == nomeview.ToLower())
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
        public ActionResult Lista(MLModuloLista model, string nomeView = "")
        {
            try
            {

                if (model == null) model = new MLModuloLista();
                if (!model.Quantidade.HasValue) model.Quantidade = 5;

                int paginaAtual = 1;

                if (Request.QueryString["rep"] != null && !String.IsNullOrEmpty(Request.QueryString["rep"]))
                {
                    int repositorio = Convert.ToInt16(Request.QueryString["rep"]);

                    if (model.Repositorio == repositorio)
                    {
                        int.TryParse(Convert.ToString(RouteData.Values["extra1"]), out paginaAtual);
                    }
                }

                if (paginaAtual == 0) paginaAtual = 1;

                var lista = BLLista.ListarPublico(model, paginaAtual, codigoIdioma: BLIdioma.CodigoAtual);

                if (lista.Count > 0)
                {
                    var totalPaginas = Math.Ceiling((decimal)lista[0].TotalRows / (decimal)model.Quantidade);
                    ViewData["paginas"] = totalPaginas;

                    if (paginaAtual > totalPaginas) paginaAtual = (int)totalPaginas;
                }

                ViewData["modulo"] = model;
                ViewData["paginaAtual"] = paginaAtual;

                if (!string.IsNullOrEmpty(nomeView))
                {
                    return View(nomeView, lista);
                }
                else
                {
                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region Lista PreReleases
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaPreReleases(MLModuloLista model, string nomeView = "")
        {
            try
            {

                if (model == null) model = new MLModuloLista();
                if (!model.Quantidade.HasValue) model.Quantidade = 5;

                int paginaAtual = 1;

                if (Request.QueryString["rep"] != null && !String.IsNullOrEmpty(Request.QueryString["rep"]))
                {
                    int repositorio = Convert.ToInt16(Request.QueryString["rep"]);

                    if (model.Repositorio == repositorio)
                    {
                        int.TryParse(Convert.ToString(RouteData.Values["extra1"]), out paginaAtual);
                    }
                }

                if (paginaAtual == 0) paginaAtual = 1;

                var lista = BLLista.ListarPublicoLista(model, paginaAtual, codigoIdioma: BLIdioma.CodigoAtual);

                if (lista.Count > 0)
                {
                    var totalPaginas = Math.Ceiling((decimal)lista[0].TotalRows / (decimal)model.Quantidade);
                    ViewData["paginas"] = totalPaginas;

                    if (paginaAtual > totalPaginas) paginaAtual = (int)totalPaginas;
                }

                ViewData["modulo"] = model;
                ViewData["paginaAtual"] = paginaAtual;

                if (!string.IsNullOrEmpty(nomeView))
                {
                    return View(nomeView, lista);
                }
                else
                {
                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region ListaAno
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaAno(MLModuloLista model)
        {
            ViewBag.Anos = BLLista.ListarAnos();

            return View(model);
        }
        #endregion

        #region ListaAnoAjaxRequest
        [HttpGet, CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListaAnoAjaxRequest(decimal codigoLista, string categorias, int ano)
        {
            var lista = BLLista.ListarPorAno(new MLModuloLista
            {
                CodigoLista = codigoLista,
                Categorias = categorias
            }, ano);

            return Json(new { view = BLConteudoHelper.RenderViewToString(this, "ListaAnoAjax", String.Empty, lista) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Destaque
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Destaque(MLModuloLista model, string nomeView = "")
        {
            try
            {

                if (model == null) model = new MLModuloLista();
                if (!model.Quantidade.HasValue) model.Quantidade = 3;

                int paginaAtual = 1;

                if (Request.QueryString["rep"] != null && !String.IsNullOrEmpty(Request.QueryString["rep"]))
                {
                    int repositorio = Convert.ToInt16(Request.QueryString["rep"]);

                    if (model.Repositorio == repositorio)
                    {
                        int.TryParse(Convert.ToString(RouteData.Values["extra1"]), out paginaAtual);
                    }
                }


                var lista = BLLista.ListarPublico(model, paginaAtual, destaque: true, codigoIdioma: BLIdioma.Atual.Codigo);

                ViewData["modulo"] = model;

                if (!string.IsNullOrEmpty(nomeView))
                {
                    return View(nomeView, lista);
                }
                else
                {
                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloEventos model)
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

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptDetalhe(MLModuloEventos model, decimal codigo)
        {
            try
            {
                ViewBag.Codigo = codigo;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptAno(MLModuloLista model)
        {
            return PartialView(model);
        }

        #endregion

        #region EnviarEmailNoticia

        private MLCompartilharEmail BindEmail
            (
                string Nome, string Email,
                string NomeAmigo,
                string EmailAmigo,
                string comentario,
                string url,
                decimal codigo
            )
        {
            var cliente = BLCliente.ObterLogado();

            MLCompartilharEmail objML = new MLCompartilharEmail();

            if (cliente != null)
                objML.CodigoCliente = cliente.Codigo ?? null;

            objML.DataCadastro = DateTime.Now;
            objML.Email = Email;
            objML.Nome = Nome;
            objML.NomeAmigo = NomeAmigo;
            objML.EmailAmigo = EmailAmigo;
            objML.UrlCompartilhada = Server.UrlDecode(url);
            objML.CodigoConteudo = codigo;
            objML.Comentario = comentario;
            objML.UrlSite = Request.Url.Scheme + "://" + Request.Url.Authority;

            return objML;
        }

        #region EnviarEmailNoticia

        /// <summary>
        /// Enviar e-mail
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult EnviarEmailNoticia
            (
                string Nome,
                string Email,
                string NomeAmigo,
                string EmailAmigo,
                string comentario,
                string url,
                decimal codigo
            )
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || !BLUtilitarios.ValidarEmail(Email))
                {
                    return Json(new { success = false, msg = "Email de remetente não preenchido ou inválido" });
                }

                if (string.IsNullOrEmpty(EmailAmigo) || !BLUtilitarios.ValidarEmail(EmailAmigo))
                {
                    return Json(new { success = false, msg = "Email de destinatário não preenchido ou inválido" });
                }

                if (string.IsNullOrEmpty(Nome))
                {
                    return Json(new { success = false, msg = "Informar nome" });
                }

                if (string.IsNullOrEmpty(NomeAmigo))
                {
                    return Json(new { success = false, msg = "Informar nome do amigo" });
                }

                var objML = BindEmail(Nome, Email, NomeAmigo, EmailAmigo, comentario, url, codigo);
                objML.CodigoTipo = (int)MLCompartilharEmail.Tipos.Noticia;

                CRUD.Salvar<MLCompartilharEmail>(objML, BLPortal.Atual.ConnectionString);

                //Montar src da imagem da notícia.
                var not = CRUD.Obter<MLListaConteudoPublicado>(codigo);

                string src = String.Empty;

                if (not == null)
                {
                    not = new MLListaConteudoPublicado();
                }

                var portal = BLPortal.Atual;

                if (!String.IsNullOrWhiteSpace(not.Imagem) &&
                    System.IO.File.Exists(Server.MapPath(string.Format("~/portal/{0}/arquivos/listas/{1}/capa.png", portal.Diretorio, not.Codigo))))
                    src = String.Format("{0}/thumb/{1}/listas/{2}/213/0/capa{3}/", objML.UrlSite, portal.Diretorio, not.Codigo, not.Imagem);
                else
                    src = String.Format("{0}/content/img/img-share-default-small.jpg", objML.UrlSite);

                BLEmail.Enviar
                    (
                        String.Format("Braskem | {0} compartilhou uma notícia com você!", Nome),
                        EmailAmigo,
                        BLEmail.ObterModelo(BLEmail.ModelosPadrao.EmailCompartilhar)
                            .Replace("[[nome_amigo]]", NomeAmigo)
                            .Replace("[[nome]]", Nome)
                            .Replace("[[email]]", Email)
                            .Replace("[[url]]", objML.UrlCompartilhada)
                            .Replace("[[SRC_IMAGEM]]", src)
                            .Replace("[[URL_SITE]]", objML.UrlSite)
                            .Replace("[[COMENTARIO]]", !String.IsNullOrEmpty(comentario) ? "Comentário:<br />" + comentario : "")
                            .Replace("[[DESCRICAO]]", not.Chamada)
                            .Replace("[[TITULO]]", not.Titulo)
                            .Replace("[[ANO]]", DateTime.Now.Year.ToString())
                    );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #endregion

        private MLListaConteudo CarregarDetalhe(MLModuloLista model)
        {
            if (model == null) model = new MLModuloLista();

            var portal = BLPortal.Atual;
            var conteudo = CRUD.Obter(new MLListaConteudo
            {
                Url = Convert.ToString(RouteData.Values["extra1"]),
                CodigoLista = model.CodigoLista,
                Ativo = true
            }, portal.ConnectionString);

            if (conteudo == null)
            {
                return null;
            }

            if (conteudo.listaRegistroRelacionadosIdiomas != null && conteudo.listaRegistroRelacionadosIdiomas.Count > 0)
                BLPagina.Atual.ListaDetalhesRelacionados = conteudo.listaRegistroRelacionadosIdiomas;

            var seo = CRUD.Obter(new MLListaConteudoSEO { Codigo = conteudo.Codigo }, portal.ConnectionString);

            if (!string.IsNullOrEmpty(conteudo.Conteudo))
                conteudo.Conteudo = conteudo.Conteudo.Unescape();

            //#region Imagens

            //if (conteudo.Conteudo.Contains("[galeria]"))
            //{
            //    if (conteudo.Imagens.Count > 0)
            //    {
            //        ViewData["GUID"] = conteudo.GUID;
            //        ViewData["diretorioGaleria"] = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + conteudo.GUID.Value.ToString().Replace("-", "") + "/galeria").Replace("//", "/");

            //        conteudo.Conteudo = conteudo.Conteudo.Replace("[galeria]",
            //            BLConteudoHelper.RenderViewToString(this, "ImagemItem", String.Empty, conteudo.Imagens));

            //    }
            //    else
            //    {
            //        conteudo.Conteudo = conteudo.Conteudo.Replace("[galeria]", "Galeria indisponível");
            //    }
            //}

            //#endregion

            //#region Videos

            //if (conteudo.Conteudo.Contains("[video]"))
            //{
            //    // Obter arquivos em disco (normalizando para apenas enviar os NOMES sem o diretório)
            //    var diretorioVideo = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + conteudo.GUID.Value.ToString().Replace("-", "") + "/video").Replace("//", "/");
            //    ViewData["diretorioVideo"] = diretorioVideo;

            //    while (conteudo.Conteudo.Contains("[video]"))
            //    {
            //        MLListaConteudoVideo video = new MLListaConteudoVideo();

            //        //Determina a posição inicial e final da tag para encontrar o código do video
            //        int indexInicio = conteudo.Conteudo.IndexOf("[video]");
            //        int indexFim = conteudo.Conteudo.IndexOf("[/video]");
            //        string strGuidVideo = conteudo.Conteudo.Substring((indexInicio + 7), (indexFim - (indexInicio + 7)));

            //        //Verifica se o código é valido
            //        Guid guidVideo;
            //        bool IsSucesso = true;

            //        if (Guid.TryParse(strGuidVideo, out guidVideo))
            //        {
            //            //Encontrar o video especificado na tag
            //            video = conteudo.Videos.Find(v => v.Guid == guidVideo);

            //            IsSucesso = (video != null && video.Codigo.HasValue && video.Codigo.Value > 0);
            //        }
            //        else
            //        {
            //            IsSucesso = false;
            //        }

            //        if (IsSucesso)
            //            conteudo.Conteudo = conteudo.Conteudo.Replace("[video]" + strGuidVideo + "[/video]",
            //                BLConteudoHelper.RenderViewToString(this, "VideoItem", String.Empty, video));
            //        else
            //            conteudo.Conteudo = conteudo.Conteudo.Replace("[video]" + strGuidVideo + "[/video]", "Vídeo não disponível");
            //    }
            //}

            //#endregion

            //#region Audio

            //if (conteudo.Conteudo.Contains("[audio]"))
            //{
            //    // Obter arquivos em disco (normalizando para apenas enviar os NOMES sem o diretório)
            //    var diretorioaudio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + conteudo.GUID.Value.ToString().Replace("-", "") + "/audio").Replace("//", "/");
            //    ViewData["diretorioaudio"] = diretorioaudio;

            //    while (conteudo.Conteudo.Contains("[audio]"))
            //    {
            //        MLListaConteudoAudio audio = new MLListaConteudoAudio();

            //        //Determina a posição inicial e final da tag para encontrar o código do audio
            //        int indexInicio = conteudo.Conteudo.IndexOf("[audio]");
            //        int indexFim = conteudo.Conteudo.IndexOf("[/audio]");
            //        string strGuidaudio = conteudo.Conteudo.Substring((indexInicio + 7), (indexFim - (indexInicio + 7)));

            //        //Verifica se o código é valido
            //        Guid guidaudio;
            //        bool IsSucesso = true;

            //        if (Guid.TryParse(strGuidaudio, out guidaudio))
            //        {
            //            //Encontrar o audio especificado na tag
            //            audio = conteudo.Audios.Find(v => v.Guid == guidaudio);

            //            IsSucesso = (audio != null && audio.Codigo.HasValue && audio.Codigo.Value > 0);
            //        }
            //        else
            //        {
            //            IsSucesso = false;
            //        }

            //        if (IsSucesso)
            //            conteudo.Conteudo = conteudo.Conteudo.Replace("[audio]" + strGuidaudio + "[/audio]",
            //                BLConteudoHelper.RenderViewToString(this, "audioItem", String.Empty, audio));
            //        else
            //            conteudo.Conteudo = conteudo.Conteudo.Replace("[audio]" + strGuidaudio + "[/audio]", "Vídeo não disponível");
            //    }
            //}

            //#endregion

            if (seo == null)
                seo = new MLListaConteudoSEO();

            #region Valores Seo

            //Define a imagem que será utilizada, a prioridade é a imagem da aba SEO
            if (String.IsNullOrWhiteSpace(seo.Ogimage))
            {
                string nomeArquivo = String.Empty;

                if (!string.IsNullOrWhiteSpace(conteudo.Imagem))
                    nomeArquivo = String.Format("/portal/{0}/arquivos/listas/{1}/capa{2}", portal.Diretorio, conteudo.Codigo, conteudo.Imagem);
                else
                    nomeArquivo = "/content/img/img-share-default.jpg";

                seo.Image = Request.Url.Scheme + "://" + Request.Url.Authority + nomeArquivo.Replace(" ", "%20");
                seo.Ogimage = seo.Image;
            }
            else
            {
                seo.Image = seo.Ogimage;
            }

            if (String.IsNullOrWhiteSpace(seo.Titulo))
                seo.Titulo = conteudo.Titulo;

            if (String.IsNullOrWhiteSpace(seo.Url))
                seo.Url = Request.Url.ToString();

            if (String.IsNullOrWhiteSpace(seo.Description))
                seo.Description = conteudo.Chamada ?? conteudo.Titulo;

            if (String.IsNullOrWhiteSpace(seo.Tags))
                seo.Tags = conteudo.Tags;

            seo.SetPadroes();

            #endregion

            BLConteudo.AdicionarSeo(seo);

            if (conteudo.CodigoIdioma.HasValue && model.CodigoLista.HasValue && !String.IsNullOrWhiteSpace(conteudo.Tags))
                conteudo.Relacionados =
                    BLLista.ListarRelacionados
                    (
                        model.CodigoLista.Value,
                        conteudo.Codigo.Value,
                        portal.Codigo.Value,
                        conteudo.CodigoIdioma.Value,
                        model.Quantidade,
                        conteudo.Tags
                    );

            return conteudo;
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult DetalheNovo(MLModuloLista model)
        {
            bool redirectUrl = false;

            try
            {
                var conteudo = CarregarDetalhe(model);

                redirectUrl = conteudo == null;

                if (!redirectUrl)
                {
                    ViewData["modulo"] = model;
                    return View(conteudo);
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }

            var url = Convert.ToString(RouteData.Values["extra1"]);
            if (!string.IsNullOrWhiteSpace(url))
            {
                return NotFound(url);
            }
            return View(new MLListaConteudo());
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult DetalheCampanha(MLModuloLista model)
        {
            bool redirectUrl = false;
            try
            {
                var conteudo = CarregarDetalhe(model);

                redirectUrl = conteudo == null;

                if (!redirectUrl)
                {
                    ViewData["modulo"] = model;

                    return View(conteudo);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }

            var url = Convert.ToString(RouteData.Values["extra1"]);
            if (!string.IsNullOrWhiteSpace(url))
            {
                return NotFound(url);
            }

            return View(new MLListaConteudo());
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Detalhe(MLModuloLista model, string nomeView = "")
        {
            bool redirectUrl = false;
            try
            {
                var conteudo = CarregarDetalhe(model);

                redirectUrl = conteudo == null;

                if (!redirectUrl)
                {
                    ViewData["modulo"] = model;

                    if (!string.IsNullOrEmpty(nomeView))
                    {
                        return View(nomeView, conteudo);
                    }
                    else
                    {
                        return View("Detalhe", conteudo);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }

            var url = Convert.ToString(RouteData.Values["extra1"]);
            if (!string.IsNullOrWhiteSpace(url))
            {
                return NotFound(url);
            }

            return View(new MLListaConteudo());
        }

        #region Lista Com Filtro

        #region ScriptListaFiltro
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptListaFiltro(MLModuloLista model, decimal totalPaginas)
        {
            ViewBag.TotalPaginas = totalPaginas;
            return View(model);
        }
        #endregion

        #region ListaFiltro
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaFiltro(MLModuloLista model)
        {
            var lstAnos = BLLista.ListarComunicacaoAno(model, 1);
            var lstTemas = BLLista.ListarComunicacaoTemas(model);

            if (!model.Quantidade.HasValue)
                model.Quantidade = 6;

            ViewBag.Anos = lstAnos;
            ViewBag.Temas = lstTemas;

            var lista = BLLista.ListaFiltroBuscar(model.CodigoLista.Value, model.Categorias, 1, model.Quantidade.Value, BLIdioma.Atual.Codigo, null, null, String.Empty);
            ViewBag.Conteudo = lista;

            if (lista.Count > 0)
            {
                var totalPaginas = Math.Ceiling((decimal)lista[0].TotalRows / (decimal)model.Quantidade);
                ViewBag.TotalPaginas = totalPaginas;
            }

            return View(model);
        }
        #endregion

        #region ListaFiltroMeses
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListaFiltroMeses(decimal decCodigoLista, string strCategorias, int intAno)
        {
            try
            {
                var lstMeses = BLLista.ListarComunicacaoMes(decCodigoLista, strCategorias, BLIdioma.Atual.Codigo, intAno);
                return Json(new { success = true, lista = lstMeses });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        #endregion

        #region ListaFiltroBuscar
        /// <summary>
        /// Action da lista
        /// </summary>        
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListaFiltroBuscar(decimal decCodigoLista, string strCategorias, int intPagina, int intQuantidade, int? intAno, int? intMes, string strBusca, string urlDetalhe, string nomeViewItem)
        {
            try
            {
                ViewBag.UrlDetalhe = urlDetalhe;
                var lista = BLLista.ListaFiltroBuscar(decCodigoLista, strCategorias, intPagina, intQuantidade, BLIdioma.Atual.Codigo, intAno, intMes, strBusca);

                var strHtmlOutput = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "Lista", nomeViewItem, lista);
                decimal totalPaginas = 0;

                if (lista.Count > 0)
                    totalPaginas = Math.Ceiling((decimal)lista[0].TotalRows / (decimal)intQuantidade);
                else
                    strHtmlOutput = "<div class='alert alert-warning'>" + T("Nenhum registro encontrado") + "</div>";

                return Json(new { success = true, html = strHtmlOutput, total = totalPaginas });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }

        }
        #endregion

        #region ListaFiltroItem
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaFiltroItem(List<MLListaConteudoPublicoListagem> Model, string urlDetalhe)
        {
            ViewBag.UrlDetalhe = urlDetalhe;
            return View(Model);
        }
        #endregion

        #endregion

        #region Campanhas

        #region ScriptListaCampanha
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptListaCampanha(MLModuloLista model)
        {
            return View(model);
        }
        #endregion

        #region ListaCampanha
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaCampanha(MLModuloLista model)
        {
            if (!model.Quantidade.HasValue)
                model.Quantidade = 6;

            return View(model);
        }
        #endregion

        #region ListaCampanhaItem
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListaCampanhaItem(List<MLListaConteudoPublicoListagem> Model, string urlDetalhe)
        {
            return View(Model);
        }
        #endregion

        #endregion

        #region Comentarios
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Comentarios(MLModuloLista model)
        {
            ViewBag.Comentarios = CRUD.Listar(new MLListaConteudo { CodigoLista = model.CodigoLista, Publicado = true }, PortalAtual.ConnectionString).OrderBy(x => x.Data).ToList();
            return View(model.NomeView ?? "labs/comentarios");
        }
        #endregion

        #region Eventos Calendario Landing

        [HttpGet]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult GetEventos(decimal codigoLista, string categoria, string rota, string porto, string sentido)
        {
            MLListaConteudoAvaliacao objML = new MLListaConteudoAvaliacao();

            try
            {
                var model = new MLModuloLista { CodigoLista = codigoLista, Quantidade = 99999 };
                var lista = BLLista.ListarPublico(model, 1, codigoIdioma: BLIdioma.CodigoAtual);                
                lista = lista.FindAll(o =>
                    (string.IsNullOrEmpty(rota) || (!string.IsNullOrEmpty(o.Extra1) && o.Extra1.Equals(rota, StringComparison.InvariantCultureIgnoreCase))) &&
                    (string.IsNullOrEmpty(porto) || (!string.IsNullOrEmpty(o.Extra2) && o.Extra2.Equals(porto, StringComparison.InvariantCultureIgnoreCase))) &&
                    (string.IsNullOrEmpty(sentido) || (!string.IsNullOrEmpty(o.Extra3) && o.Extra3.Equals(sentido, StringComparison.InvariantCultureIgnoreCase)))
                );
                var eventos = new List<MLListaEventosCalendario>();
                foreach (var item in lista)
                {
                    eventos.Add(new MLListaEventosCalendario
                    {
                        id = item.Codigo.ToString(),
                        title = item.Titulo,
                        start = item.Data.Value.ToString("yyyy-MM-dd"),
                        color = !string.IsNullOrEmpty(item.Categoria) && item.Categoria.Split('|').Length > 1 ? item.Categoria.Split('|')[1] : "#021859",
                        display = !string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(item.Categoria) && !item.Categoria.Split('|')[0].Equals(categoria) ? "none" : "auto"
                    }
                    );
                }

                //Limpar o cache após avaliar, para que no próximo carregando a informações esteja atualizada
                //var cacheKey = string.Format("url_{0}_codigolista_{1}", Request.Url.ToString(), CodigoLista);
                //BLCachePortal.Remove(cacheKey);

                return Json(eventos.FindAll(o => !o.display.Equals("none")), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }

        [HttpGet]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult GetFiltros(decimal codigoLista)
        {
            MLListaConteudoAvaliacao objML = new MLListaConteudoAvaliacao();

            try
            {
                var model = new MLModuloLista { CodigoLista = codigoLista, Quantidade = 99999 };
                var lista = BLLista.ListarPublico(model, 1, codigoIdioma: BLIdioma.CodigoAtual);
              
                var _portos = new List<MLListaPortosCalendarios>();
                var _servicos = new List<string>();
                foreach (var item in lista)
                {                    
                    if (!string.IsNullOrEmpty(item.Extra1) && !_servicos.Contains(item.Extra1))
                    {
                        _servicos.Add(item.Extra1);
                    }

                    if (!string.IsNullOrEmpty(item.Extra2) &&
                        _portos.Find(o=>
                        o.porto.Equals(item.Extra2,StringComparison.InvariantCultureIgnoreCase) &&
                        o.servico.Equals(item.Extra1, StringComparison.InvariantCultureIgnoreCase)
                        ) ==null)
                    {
                        _portos.Add(new MLListaPortosCalendarios
                        {
                            servico = item.Extra1,
                            porto = item.Extra2
                        });
                    }
                }
               
                //Limpar o cache após avaliar, para que no próximo carregando a informações esteja atualizada
                //var cacheKey = string.Format("url_{0}_codigolista_{1}", Request.Url.ToString(), CodigoLista);
                //BLCachePortal.Remove(cacheKey);

                return Json(new {servicos = _servicos.OrderBy(o=>o).ToList(), portos = _portos.OrderBy(o => o.porto).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }


        #endregion
    }


}