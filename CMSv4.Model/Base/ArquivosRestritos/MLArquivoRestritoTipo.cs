using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ARE_ARQUIVO_RESTRITO_TIPO")]
    public class MLArquivoRestritoTipo
    {
        [DataField("ART_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("ART_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }
        
        [DataField("ART_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("ART_B_ARQUIVO", SqlDbType.Bit)]
        public bool? IsArquivo { get; set; }
    }
}
