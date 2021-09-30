using Framework.Model;
using System;
using System.Data;
using System.Web;

namespace Framework.Utilities
{
    public class MLSitemapPagina
    {
        [DataField("PAG_C_URL", SqlDbType.VarChar, 200)]
        public string UrlPagina { get; set; }

        [DataField("POR_C_URL", SqlDbType.VarChar, 500)]
        public string UrlPortal { get; set; }

        [DataField("LOG_D_ALTERACAO", SqlDbType.DateTime)]
        public DateTime? DataAlteracao { get; set; }

        [DataField("PAG_B_HTTPS", SqlDbType.Bit)]
        public bool? IsHttps { get; set; }

        public string UrlPaginaCompleta()
        {
            var scheme = IsHttps.HasValue && IsHttps.Value ? "https" : "http";

            if (!string.IsNullOrWhiteSpace(UrlPortal))
            {
                var arrayUrl = UrlPortal.Split(',');

                if (arrayUrl.Length > 0)
                    return string.Format("{0}://{1}/{2}", scheme, arrayUrl[0], UrlPagina);
            }

            return string.Format("{0}://{1}/{2}/{3}", scheme, HttpContext.Current.Request.Url.Authority, BLPortal.Atual.Diretorio, UrlPagina);
        }
    }

    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}
