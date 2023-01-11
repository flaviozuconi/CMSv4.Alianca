using System;
using System.Data;
using Framework.Model;


namespace CMSv4.Model   
{
    [Serializable]
    [Table("MOD_BAN_BANNERS_VIEWS")]
    public class MLBannerView
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("BAV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("BAV_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("BAV_C_VIEW", SqlDbType.VarChar, 250)]
        public string View { get; set; }
    }
}
