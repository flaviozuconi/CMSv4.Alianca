using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_DIG_DICIONARIO_GRUPO")]
    [Auditing("/cms/dicionariogrupoadmin", "CodigoPortal")]
    public class MLDicionarioGrupo
    {
        [DataField("DIG_POR_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPortal { get; set; }

        [DataField("DIG_IDI_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("DIG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("DIG_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("DIG_C_TERMO", SqlDbType.VarChar, 100)]
        public string Termo { get; set; }

        [DataField("DIG_C_DESCRICAO", SqlDbType.VarChar, 100)]
        public string Descricao { get; set; }

        [DataField("DIG_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

    }
}
