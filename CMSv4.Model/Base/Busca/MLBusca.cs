using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_BUS_BUSCA")]
    [Auditing("/cms/buscaadmin", "CodigoPortal")]
    public class MLBusca
    {
        [DataField("BUS_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("BUS_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("BUS_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("BUS_AGR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoAgrupador { get; set; }

        [DataField("BUS_LIS_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoLista { get; set; }

        [DataField("BUS_MOD_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoModulo { get; set; }

        [DataField("BUS_C_URL", SqlDbType.VarChar, 500)]
        public string UrlPagina { get; set; }

        [JoinField("BUS_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_C_NOME")]
        [DataField("MOD_C_NOME", SqlDbType.VarChar, 500)]
        public string NomeModulo { get; set; }

        [JoinField("BUS_IDI_N_CODIGO", "FWK_IDI_IDIOMA", "IDI_N_CODIGO", "IDI_C_NOME")]
        [DataField("IDI_C_NOME", SqlDbType.VarChar, 500)]
        public string NomeIdioma { get; set; }

        [JoinField("BUS_AGR_N_CODIGO", "MOD_CAA_CATEGORIA_AGRUPADOR", "CAA_N_CODIGO", "CAA_C_NOME")]
        [DataField("CAA_C_NOME", SqlDbType.VarChar, 200)]
        public string NomeAgrupador { get; set; }
    }
}
