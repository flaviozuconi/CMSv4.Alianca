using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("CMS_PAG_PAGINA_SEO")]
    public class MLPaginaSeo : MLConteudoSeo
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }
    }
}
