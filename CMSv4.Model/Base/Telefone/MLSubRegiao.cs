using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_SRE_SUB_REGIAO")]
    [Auditing("/cms/subregiaoadmin", "CodigoPortal")]
    public class MLSubRegiao
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("SRE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("SRE_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string Descricao { get; set; }

        [DataField("SRE_PRG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPaisRegiao { get; set; }

        [DataField("SRE_C_IMAGEM", SqlDbType.VarChar, 50)]
        public string Imagem { get; set; }

        [JoinField("SRE_PRG_N_CODIGO", "MOD_PRG_PAIS_REGIAO", "PRG_N_CODIGO", "PRG_C_DESCRICAO")]
        [DataField("PRG_C_DESCRICAO", SqlDbType.VarChar, 50)]
        public string NomePaisRegiao { get; set; }

    }
}
