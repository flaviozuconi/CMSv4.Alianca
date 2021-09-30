using System;
using System.Linq;

namespace Framework.Utilities
{
    /// <summary>
    /// MÉTODOS AUXILIARES PARA CORREÇÃO DE URL
    /// </summary>
    public static class Portal
    {
        /// <summary>
        /// EX: HTTP://LOCALHOST
        /// </summary>
        public static string Url()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                return string.Concat(context.Request.Url.Scheme, "://", context.Request.Url.Authority);
            }

            return string.Empty;
        }

        /// <summary>
        /// EX: { "DETALHE-EXEMPLO", "123", "?QS=TESTE" } PARA HTTP://LOCALHOST/DETALHE-EXEMPLO/123?QS=TESTE
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static string Url(params object[] urls)
        {
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < urls.Length; i++)
            {
                if (urls[i] == null)
                {
                    continue;
                }

                var value = Convert.ToString(urls[i]);

                if (i == 0)
                {
                    sb.Append(value);
                }
                else
                {
                    if (!value.StartsWith("/") && !value.StartsWith("?") && !value.StartsWith("&"))
                    {
                        sb.Append("/");
                    }

                    sb.Append(value);
                }
            }

            return sb.ToString();
        }
                
        /// <summary>
        /// EX: HTTP://LOCALHOST/CMS/PRINCIPAL
        /// </summary>
        public static string UrlCms(MLPortal portal)
        {
            var urlSite = Url();

            return string.Format("{0}/cms/{1}", urlSite, portal.Diretorio);
        }

        /// <summary>
        /// EX: HTTP://LOCALHOST/PRINCIPAL OU SE A URL FOR A MESMA PRESENTE NA TABELA ADM_PORTAL, APENAS HTTP://LOCALHOST
        /// </summary>
        public static string UrlDiretorio(MLPortal portal)
        {
            if (portal.Url == null)
                portal.Url = string.Empty;

            var urlSite = Url();
            var urlEncontrada = portal.Url.Split(',').Where(a => urlSite.EndsWith(a)).ToList();

            if (urlEncontrada.Count > 0 && !string.IsNullOrEmpty(urlEncontrada[0]))
                return string.Format("{0}", urlSite);

            return string.Format("{0}/{1}", urlSite, portal.Diretorio);
        }

        /// <summary>
        /// EX: HTTP://LOCALHOST/PRINCIPAL
        /// </summary>
        public static string UrlImagem(MLPortal portal)
        {
            var urlSite = Url();
            
            return string.Format("{0}/{1}", urlSite, portal.Diretorio);
        }

        /// <summary>
        /// EX: HTTP://LOCALHOST/PORTAL/PRINCIPAL
        /// </summary>
        public static string UrlPortal(MLPortal portal)
        {
            var urlSite = Url();
            
            return string.Format("{0}/portal/{1}", urlSite, portal.Diretorio);
        }

        /// <summary>
        /// EX: WWW.SITE.COM PARA HTTP://WWWW.SITE.COM 
        ///     /TESTE PARA HTTP://LOCALHOST/TESTE E 
        ///     TESTE PARA HTTP://LOCALHOST/TESTE
        /// </summary>
        public static string ResolveUrl(string url, MLPortal portal = null)
        {
            var urlSite = portal != null ? UrlDiretorio(portal) : Url();

            if (url == null)
                url = string.Empty;

            if (url.Equals("javascript:;", System.StringComparison.InvariantCultureIgnoreCase))
                return url;

            if (!url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)) //ex: url = 'http://teste.com'
            {
                if (url.StartsWith("www", StringComparison.InvariantCultureIgnoreCase)) //ex: url = 'www.teste.com'
                    url = string.Concat("http://", url);
                else if (url.StartsWith("/")) //ex: url = '/teste'
                    url = string.Concat(urlSite, url);
                else //ex: url = 'teste'
                    url = string.Concat(urlSite, "/", url);
            }

            return url;
        }
    }
}
