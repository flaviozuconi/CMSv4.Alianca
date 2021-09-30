using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Caching;
using System.Web.Mvc;
using System.Configuration;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Controllers
{
    public class PublicoController : Controller
    {
        //url padrão de página de erro 404 caso não encontre a url na base de dados
        public const string _defaultPaginaNaoEncontrada = "pagina-nao-encontrada";

        //url padrão de página de erro 500 caso não encontre a url na base de dados
        public const string _defaultErroInterno = "erro-interno";

        #region Index

        [Compress]
        public ActionResult Index(string url, string extra1, string extra2, decimal? codigoPortal)
        {
            if (codigoPortal.HasValue)
            {
                var portal = BLPortal.Portais.Find(o => o.Codigo == codigoPortal);
                return PaginaPortal(portal, url, extra1, extra2);
            }
            else
            {
                var atual = BLPortal.Atual;
                return PaginaPortal(atual, url, extra1, extra2);
            }
        }
        #endregion

        #region Script

        public ActionResult Script(MLModuloLogin model)
        {
            return View("~/Areas/Modulo/Views/ModuloLogin/Index.cshtml", model);
        }

        #endregion

        #region PaginaPortal
        /// <summary>
        /// Apresenta a página do CMS na área pública
        /// </summary>
        public ActionResult PaginaPortal(MLPortal portal, string url, string extra1, string extra2)
        {
            try
            {
                if (portal == null || !portal.Codigo.HasValue)
                {
                    var urlRedirect = ObterUrlErroInterno();

                    return Redirect(string.Concat(urlRedirect, "?url=", Url.Encode(url)));
                }

                if (portal.Manutencao.GetValueOrDefault())
                {
                    return Redirect("~/manutencao.html");
                }

                if (Request.QueryString["cache"] == "0") BLCachePortal.ResetarCachePortal(portal.Codigo.Value);

                // HOME
                if (string.IsNullOrEmpty(url))
                {
                    url = ObterUrlHome(portal);
                }

                // Obter Pagina
                var pagina = ObterPagina(url, portal);

                if (pagina == null || !pagina.Codigo.HasValue)
                {
                    var urlRedirect = ObterUrlPaginaNaoEncontrada(portal);

                    return Redirect(string.Concat(urlRedirect, "?url=", Url.Encode(url)));
                }

                // Validar Requisição HTTPS
                if (pagina.Https.GetValueOrDefault()) //Página só pode ser exibida sob protocolo ssl
                {
                    var urlRequisitada = Request.Url.ToString();

                    if (Request.Url.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase) && /*não possuir https*/
                        urlRequisitada.IndexOf("http://localhost", StringComparison.InvariantCultureIgnoreCase) == -1 && /*não é localhost*/
                        urlRequisitada.IndexOf("homolog.co", StringComparison.InvariantCultureIgnoreCase) == -1) /*não é homolog.co*/
                    {
                        if (Request.Url.Host.IndexOf("www", StringComparison.InvariantCultureIgnoreCase) == -1) //incluir www. caso não possua
                        {
                            urlRequisitada = urlRequisitada.Replace(Request.Url.Host, string.Concat("www.", Request.Url.Host));
                        }

                        var urlssl = Convert.ToString(ConfigurationManager.AppSettings["CMS.Configuracoes.Url.Https"]);

                        if (!string.IsNullOrWhiteSpace(urlssl) && Request.Url.ToString().ToLower().Contains(urlssl))
                        {
                            Response.Redirect(urlRequisitada.Replace(Request.Url.Scheme, "https")); //redireciona para página com ssl
                        }
                        else
                        {
                            Response.Redirect(string.Format("{0}/{1}/{2}", urlssl, portal.Diretorio, url));
                        }
                    }
                }

                // Validar Permissoes
                PermissaoPublica permissao = ValidarPermissoes(pagina.Codigo.Value, portal);

                //TODO tratar página de login / acesso negado. Se possível, a página deverá estar na tabela cms_pag_pagina
                if (permissao == PermissaoPublica.NaoLogado)
                {
                    if (!String.IsNullOrEmpty(pagina.UrlLogin))
                    {
                        TempData["PortalUrl"] = portal.Codigo;
                        return Redirect(string.Concat(pagina.UrlLogin, "?key=", BLEncriptacao.EncriptarQueryString(Request.Url.ToString())));
                    }
                    else
                    {
                        return View("~/Areas/Modulo/Views/ModuloLogin/Default.cshtml", new MLModuloLogin() { Repositorio = 0, CodigoPagina = pagina.Codigo, CodigoUsuario = 0, UrlRetorno = Request.Url.ToString() });
                    }
                }
                else if (permissao == PermissaoPublica.AcessoNegado)
                {
                    return View("~/Areas/Modulo/Views/ModuloLogin/AcessoNegado.cshtml", new MLModuloLogin() { Repositorio = 0, CodigoPagina = pagina.Codigo, CodigoUsuario = 0, UrlRetorno = Request.Url.ToString(), UrlPaginaLogin = pagina.UrlLogin });
                }

                #region Definir idioma
                //define no contexto informações da página atual e de idioma
                BLPagina.Atual = pagina;

                BLIdioma.Atual = new MLIdioma
                {
                    Codigo = pagina.IdiomaCodigo,
                    Nome = pagina.IdiomaNome,
                    Sigla = pagina.IdiomaSigla,
                    Ativo = pagina.IdiomaAtivo
                };

                //Define cultura seguindo sigla de idioma da página
                //if (!string.IsNullOrEmpty(pagina.IdiomaSigla))
                Thread.CurrentThread.CurrentCulture = new CultureInfo(pagina.IdiomaSigla);
                #endregion

                string caminhoView = "";
                string caminhoLayout = "";

                //Criação do Seo da página
                var seo = CRUD.Obter<MLPaginaSeo>(pagina.Codigo.Value, portal.ConnectionString);

                if (seo == null)
                    seo = new MLPaginaSeo();

                seo.Url = Request.Url.ToString();
                seo.Titulo = pagina.Titulo;
                seo.Description = pagina.Descricao;
                seo.Tags = pagina.Tags;
                seo.SetPadroes();

                BLConteudo.AdicionarSeo(seo);

                if (!string.IsNullOrEmpty(seo.Titulo))
                {
                    BLConteudo.AdicionarTitleAoHead(seo.Titulo);
                }

                //ViewBag.Title = seo.Titulo;
                ViewBag.Schema = seo.Schema;

                Renderizar(portal, pagina, ref caminhoView, ref caminhoLayout);
                SetHeaders();
                return Content(BLConteudoHelper.RenderViewToString(this, caminhoView, caminhoLayout, pagina));
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                ViewData["erro"] = ex;

                var urlRedirect = ObterUrlErroInterno();

                return Redirect(string.Concat(urlRedirect, "?url=", Url.Encode(url)));
            }
        }

        #endregion

        private void SetHeaders()
        {
            var cp = StringComparison.CurrentCultureIgnoreCase;
            try
            {
                var configuracoes = BLCmsConfiguracao.Listar();
                var headerConfig = configuracoes.Find(o => o.Chave.Equals("headers-config", cp));
                if (headerConfig == null || string.IsNullOrEmpty(headerConfig.Valor))
                    return;

                var headers = headerConfig.Valor.Split('|');

                foreach (var hd in headers)
                {
                    var value = configuracoes.Find(o => o.Chave.Equals(hd, cp));
                    if (value != null && !string.IsNullOrEmpty(value.Valor))
                    {
                        if (Response.Headers.AllKeys.Contains(hd))
                            Response.Headers.Remove(hd);

                        Response.Headers.Add(value.Chave, value.Valor);
                    }
                }

                //Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                //Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                //Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                //Response.Headers.Add("X-Content-Type-Options", "nosniff");
                //Response.Headers.Add("Referrer-Policy", "strict-origin");            
                //Response.Headers.Add("Content-Security-Policy", "");

                //Response.Headers.Remove("X-Powered-By");
                //Response.Headers.Remove("X-AspNet-Version");
                //Response.Headers.Remove("X-AspNetMvc-Version");
                //Response.Headers.Remove("Server");    
                var headersremove = configuracoes.Find(o => o.Chave.Equals("headers-remove", cp));
                if (headersremove == null || string.IsNullOrEmpty(headersremove.Valor))
                    return;

                if (!Response.HeadersWritten)
                {
                    Response.AddOnSendingHeaders((c) =>
                    {
                        if (c != null && c.Response != null && c.Response.Headers != null)
                        {
                            foreach (string header in headersremove.Valor.Split('|'))
                            {
                                if (c.Response.Headers[header] != null)
                                {
                                    c.Response.Headers.Remove(header);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }

        #region Obter Url's

        /// <summary>
        /// Retorna a URL da home configurada para o portal
        /// Caso não esteja configurada, o valor padrão é [home]
        /// </summary>
        private string ObterUrlHome(MLPortal portal)
        {
            var cacheKey = string.Format("portal_{0}_home_url", portal.Codigo);
            var cachedValue = HttpContext.Cache.Get(cacheKey);

            if (cachedValue == null)
            {
                var url = CRUD.Obter<MLPortalPublico>(portal.Codigo.Value, portal.ConnectionString).UrlHome;
                if (string.IsNullOrEmpty(url)) url = "home";

                HttpContext.Cache.Add(cacheKey, url, null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                return url;
            }

            return Convert.ToString(cachedValue);
        }

        private string ObterUrlPaginaNaoEncontrada(MLPortal portal = null)
        {
            if (portal == null || !portal.Codigo.HasValue)
            {
                return _defaultPaginaNaoEncontrada;
            }

            var cacheKey = string.Format("portal_{0}_pagina_404_url", portal.Codigo);
            var cachedValue = Convert.ToString(HttpContext.Cache.Get(cacheKey));

            if (string.IsNullOrEmpty(cachedValue))
            {
                var url = CRUD.Obter<MLPortalPublico>(portal.Codigo.Value, portal.ConnectionString).Url404;
                if (string.IsNullOrEmpty(url)) url = _defaultPaginaNaoEncontrada;

                HttpContext.Cache.Add(cacheKey, url, null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                return url;
            }

            //if (!cachedValue.StartsWith("/") && !cachedValue.StartsWith("~/"))
            //{
            //    return string.Concat("~/", cachedValue);
            //}

            return cachedValue;
        }

        private string ObterUrlErroInterno(MLPortal portal = null)
        {
            if (portal == null || !portal.Codigo.HasValue)
            {
                return _defaultErroInterno;
            }

            var cacheKey = string.Format("portal_{0}_pagina_500_url", portal.Codigo);
            var cachedValue = Convert.ToString(HttpContext.Cache.Get(cacheKey));

            if (string.IsNullOrEmpty(cachedValue))
            {
                var url = CRUD.Obter<MLPortalPublico>(portal.Codigo.Value, portal.ConnectionString).Url500;
                if (string.IsNullOrEmpty(url)) url = _defaultErroInterno;

                HttpContext.Cache.Add(cacheKey, url, null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                return url;
            }

            if (!cachedValue.StartsWith("/") && !cachedValue.StartsWith("~/"))
            {
                return string.Concat("~/", cachedValue);
            }

            return cachedValue;
        }

        #endregion

        #region Obter Pagina

        /// <summary>
        /// Obtem a model da página para apresentação na área pública
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private MLPaginaPublico ObterPagina(string url, MLPortal portal)
        {
            return BLPagina.ObterPaginaPublica(url, portal);
        }

        #endregion

        #region Validar Permissoes

        /// <summary>
        /// Obtem as permissoes de portal, seção e página para serem verificadas na renderização da página
        /// Valida se o usuário autenticado tem permissões de visualizar a página
        /// </summary>
        /// <returns></returns>
        private PermissaoPublica ValidarPermissoes(decimal codigoPagina, MLPortal portal)
        {
            var cacheKey = string.Format("portal_{0}_permissoes_{1}", portal.Codigo, codigoPagina);
            var cachedValue = BLCachePortal.Get<MLPaginaPermissaoCompleta>(cacheKey);

            if (cachedValue == null)
            {
                var permissoes = new BLPagina(portal.ConnectionString).ObterPermissoes(codigoPagina);

                BLCachePortal.Add(portal.Codigo.Value, cacheKey, permissoes);

                return ValidarPermissoes(codigoPagina, permissoes);
            }

            return ValidarPermissoes(codigoPagina, cachedValue);
        }

        /// <summary>
        /// Valida as permissões da página com o usuário autenticado
        /// </summary>
        /// <remarks>
        /// True: libera acesso
        /// False: negar acesso
        /// </remarks>
        private PermissaoPublica ValidarPermissoes(decimal codigoPagina, MLPaginaPermissaoCompleta permissoes)
        {
            var usuario = BLCliente.ObterLogado();

            // Se não está autenticado, qualquer restrição deve impedir acesso

            if (usuario == null || !usuario.Codigo.HasValue)
            {
                if (!permissoes.PaginaRestrita.HasValue && !permissoes.SecaoRestrita.HasValue && !permissoes.PortalRestrito.HasValue) return PermissaoPublica.AcessoLiberado;

                if (permissoes.PaginaRestrita.HasValue && permissoes.PaginaRestrita.Value == false) return PermissaoPublica.AcessoLiberado;
                if (permissoes.SecaoRestrita.HasValue && permissoes.SecaoRestrita.Value == false) return PermissaoPublica.AcessoLiberado;
                if (permissoes.PortalRestrito.HasValue && permissoes.PortalRestrito.Value == false) return PermissaoPublica.AcessoLiberado;

                return PermissaoPublica.NaoLogado;
            }

            var gruposDoUsuario = usuario.Grupos.Select(g => g.CodigoGrupo);

            // Permissao definida na página
            if (permissoes.PaginaRestrita.HasValue)
            {
                if (permissoes.PaginaRestrita == false) return PermissaoPublica.AcessoLiberado;

                var liberado = permissoes.GruposPagina.Select(o => o.CodigoGrupo)
                    .Intersect(gruposDoUsuario);

                if (liberado != null && liberado.Count() > 0) return PermissaoPublica.Logado;

                return PermissaoPublica.AcessoNegado;
            }

            // Permissão definida na seção
            if (permissoes.SecaoRestrita.HasValue)
            {
                if (permissoes.SecaoRestrita == false) return PermissaoPublica.AcessoLiberado;

                var teste = permissoes.GruposSecao;

                var secaoLiberado = permissoes.GruposSecao.Select(o => o.CodigoGrupoCliente)
                    .Intersect(gruposDoUsuario);

                if (secaoLiberado != null && secaoLiberado.Count() > 0) return PermissaoPublica.Logado;

                return PermissaoPublica.AcessoNegado;
            }

            // Permissão definida no portal
            if (permissoes.PortalRestrito.HasValue && permissoes.PortalRestrito.Value) return PermissaoPublica.AcessoNegado;
            else return PermissaoPublica.AcessoLiberado;
        }

        #endregion

        #region Renderizar

        /// <summary>
        /// Verifica se a página e o template já estão renderizados em disco
        /// caso não esteja, realiza a criação dos arquivos
        /// </summary>
        /// <param name="caminhoView">Retorna o caminho lógico da view para apresentação</param>
        /// <param name="caminhoLayout">Retorna o caminho lógico do layout para apresentação</param>
        /// <returns></returns>
        private bool Renderizar(MLPortal portal, MLPaginaPublico pagina, ref string caminhoView, ref string caminhoLayout)
        {
            var objBLConteudo = new BLConteudo(portal.Diretorio, portal.ConnectionString);

            string nomeLayout = BLLayout.ObterCaminhoRelativo(portal, pagina.NomeLayout);
            if (string.IsNullOrEmpty(nomeLayout)) throw new Exception("Página sem layout definido");

            var caminhoFisicoView = objBLConteudo.MontarCaminhoFisicoView(pagina.Codigo.Value);

            if (!System.IO.File.Exists(caminhoFisicoView))
            {
                objBLConteudo.CarregarPaginaPublicada(pagina.Codigo.Value, false);
            }

            caminhoView = objBLConteudo.MontarCaminhoView(pagina.Codigo.Value);
            caminhoLayout = nomeLayout;

            return true;
        }

        #endregion

        #region Idioma
        public ActionResult ListarIdioma()
        {
            var ListaPaginaIdioma = new List<MLPaginaIdioma>();
            var listaIdiomasAtivos = BLIdioma.Listar(true);
            var url = "/" + BLPortal.Atual.Diretorio;

            var queryString = Request.QueryString.ToString();

            var extra1 = "";
            if (RouteData.Values["extra1"] != null)
            {
                extra1 = Convert.ToString(RouteData.Values["extra1"]);
            }
            var extra2 = "";
            if (RouteData.Values["extra2"] != null)
            {
                extra2 = Convert.ToString(RouteData.Values["extra2"]);
            }

            foreach (var item in listaIdiomasAtivos)
            {
                if (item.Codigo != BLIdioma.CodigoAtual)
                {
                    var model = new MLPaginaIdioma();
                    model.inativo = false;
                    var urlIdioma = "";
                    var itemContem = BLPagina.Atual.listaIdiomas.Find(e => e.Codigo == item.Codigo);
                    if (itemContem != null && !string.IsNullOrEmpty(itemContem.urlPagina) && itemContem.CodigoPagina > 0)
                    {
                        urlIdioma = "/" + itemContem.urlPagina;

                        if (!string.IsNullOrEmpty(extra1))
                        {
                            var relacionado = BLPagina.Atual.ListaDetalhesRelacionados.Find(e => e.CodigoIdioma == item.Codigo);
                            if (relacionado != null && !string.IsNullOrEmpty(relacionado.Url))
                            {
                                urlIdioma = urlIdioma + "/" + relacionado.Url;
                            }
                            else
                            {
                                model.inativo = true;
                                urlIdioma = urlIdioma + "/" + extra1;
                            }
                        }

                        if (!string.IsNullOrEmpty(extra2))
                        {
                            urlIdioma = urlIdioma + "/" + extra2;
                        }

                        if (!string.IsNullOrEmpty(queryString))
                        {
                            urlIdioma += "?" + Request.QueryString;
                        }

                        model.texto = item.Sigla;
                        model.url = url + urlIdioma;

                        ListaPaginaIdioma.Add(model);
                    }
                }
            }

            return View(ListaPaginaIdioma);
        }

        public class MLPaginaIdioma
        {
            public string texto { get; set; }

            public string url { get; set; }

            public bool inativo { get; set; }
        }
        #endregion
    }
}
