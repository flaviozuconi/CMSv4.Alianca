using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    public class MLPaginaModulo
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, true, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, true, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("PAG_MOD_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoModulo { get; set; }
    }

    [Table("CMS_PAG_PAGINA_EDICAO_MODULO")]
    public class MLPaginaModuloEdicao : MLPaginaModulo
    {
        [JoinField("PAG_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_C_URL")]
        [DataField("MOD_C_URL", SqlDbType.VarChar, 50)]
        public string UrlModulo { get; set; }

        [JoinField("PAG_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_B_EDITAVEL")]
        [DataField("MOD_B_EDITAVEL", SqlDbType.Bit)]
        public bool? Editavel { get; set; }
    }

    [Table("CMS_PAG_PAGINA_PUBLICACAO_MODULO")]
    public class MLPaginaModuloPublicado : MLPaginaModulo
    {
        [JoinField("PAG_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_C_URL")]
        [DataField("MOD_C_URL", SqlDbType.VarChar, 50)]
        public string UrlModulo { get; set; }
    }
}
