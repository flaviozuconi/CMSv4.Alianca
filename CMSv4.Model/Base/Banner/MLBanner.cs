using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// BANNER
    /// </summary>
    [Serializable]
    [Table("MOD_BAN_BANNERS")]
    [Auditing("/cms/banneradmin", "CodigoPortal")]
    public class MLBanner
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("BAN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("BAN_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("BAN_N_TEMPO_PADRAO", SqlDbType.Int)]
        public int? TempoPadrao { get; set; }

        [DataField("BAN_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("BAN_D_INICIO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataInicio { get; set; }

        [DataField("BAN_D_TERMINO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataTermino { get; set; }

        [DataField("BAN_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("BAN_C_CSS_CLASS_1", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string CssClass1 { get; set; }

        [DataField("BAN_C_CSS_CLASS_2", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string CssClass2 { get; set; }

        [DataField("BAN_C_CSS_CLASS_3", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string CssClass3 { get; set; }

        [DataField("BAN_C_SUGESTAO_RESOLUCAO", SqlDbType.VarChar, 500, IgnoreEmpty = true)]
        public string SugestaoResolucao { get; set; }
    }
}
