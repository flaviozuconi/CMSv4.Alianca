using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_MUL_MULTIMIDIA_TIPO")]
    public class MLMultimidiaTipo
    {
        [DataField("MTI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("MTI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("MTI_C_HTML", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string Html { get; set; }

        [DataField("MTI_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

    }
}
