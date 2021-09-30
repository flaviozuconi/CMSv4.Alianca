using Framework.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml.Linq;

namespace Framework.Utilities
{
    public class BLSitemap
    {
        #region GetSitemapDocument

        /// <summary>
        /// Transformar elementos no padrão XML do site map
        /// </summary>
        /// <param name="sitemapNodes"></param>
        /// <returns></returns>
        public static string GetSitemapDocument(List<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        #endregion

        #region Listar Páginas

        /// <summary>
        /// XML com as páginas ativa do portal
        /// </summary>
        /// <param name="codigoPortal"></param>
        /// <returns></returns>
        public static string Paginas(decimal codigoPortal)
        {
            try
            {
                var retorno = new List<SitemapNode>();

                using (var command = Database.NewCommand("USP_CMS_L_SITEMAP"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, codigoPortal);

                    var paginas = Database.ExecuteReader<MLSitemapPagina>(command);

                    //Converter páginas para model padrão para sitemap
                    foreach (var item in paginas)
                    {
                        retorno.Add(new SitemapNode()
                        {
                            Url = item.UrlPaginaCompleta(),
                            LastModified = item.DataAlteracao,
                            Frequency = SitemapFrequency.Monthly,
                            Priority = 0.8
                        });
                    }

                    return GetSitemapDocument(retorno);
                }
            }
            catch { }

            return null;
        }

        #endregion
    }
}
