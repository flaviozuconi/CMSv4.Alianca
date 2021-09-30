using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    public class MLModuloBanner
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("BAN_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoBanner { get; set; }

        [DataField("BAN_C_VIEW", SqlDbType.VarChar, 100)]
        public string View { get; set; }

        [StringLength(100)]
        [DataField("BAN_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("BAN_N_LARGURA", SqlDbType.Int)]
        public int? Largura { get; set; }

        [DataField("BAN_N_ALTURA", SqlDbType.Int)]
        public int? Altura { get; set; }

        [DataField("BAN_B_DESCRICAO", SqlDbType.Bit)]
        public bool? ApresentarDescricao { get; set; }

        [DataField("BAN_B_INDICE", SqlDbType.Bit)]
        public bool? ApresentarIndice { get; set; }

        [DataField("BAN_B_SETAS", SqlDbType.Bit)]
        public bool? ApresentarSetas { get; set; }

        [DataField("BAN_B_AUTO", SqlDbType.Bit)]
        public bool? Autoplay { get; set; }

        [StringLength(50)]
        [DataField("BAN_C_ESTILO", SqlDbType.VarChar, 50)]
        public string Estilo { get; set; }

        [DataField("BAN_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("BAN_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("BAN_B_REDIMENSIONAR", SqlDbType.Bit)]
        public bool? Redimensionar { get; set; }

        [JoinField("BAN_N_CODIGO", "MOD_BAN_BANNERS", "BAN_N_CODIGO", "BAN_C_NOME")]
        [DataField("BAN_C_NOME", SqlDbType.VarChar, 250)]
        public string NomeBanner { get; set; }

        public int? Tempo { get; set; }

        public MLBanner Banner { get; set; }
    }

    [Table("MOD_BAN_BANNER_EDICAO")]
    public class MLModuloBannerEdicao : MLModuloBanner { }

    [Table("MOD_BAN_BANNER_PUBLICADO")]
    public class MLModuloBannerPublicado : MLModuloBanner { }

    [Table("MOD_BAN_BANNER_HISTORICO")]
    public class MLModuloBannerHistorico : MLModuloBanner
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
