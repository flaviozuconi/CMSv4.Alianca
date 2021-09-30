using System;
using Framework.Model;
using System.Data;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("CMS_POR_PORTAL")]
    public class MLPortalPublico
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_PAG_C_URL_HOME", SqlDbType.VarChar, 100)]
        public string UrlHome { get; set; }

        [DataField("POR_PAG_C_URL_404", SqlDbType.VarChar, 100)]
        public string Url404 { get; set; }

        [DataField("POR_PAG_C_URL_500", SqlDbType.VarChar, 100)]
        public string Url500 { get; set; }

        [DataField("POR_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }
    }
}
