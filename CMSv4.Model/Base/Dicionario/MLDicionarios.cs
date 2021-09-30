using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_DIC_DICIONARIO")]
    [Auditing("/cms/dicionarioadmin", "CodigoPortal")]
    public class MLDicionarios
    {
        [DataField("DIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("DIC_POR_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPortal { get; set; }

        [DataField("DIC_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("DIC_DIG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupo { get; set; }

        [DataField("DIC_C_TERMO", SqlDbType.VarChar, 200)]
        public string Termo { get; set; }

        [DataField("DIC_C_DEFINICAO", SqlDbType.VarChar, 500)]
        public string Definicao { get; set; }

    }
}
