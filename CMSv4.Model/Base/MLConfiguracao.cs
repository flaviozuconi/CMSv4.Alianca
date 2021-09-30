using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("CMS_CON_CONFIGURACAO")]
    public class MLConfiguracao
    {
        [DataField("CON_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CON_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("CON_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string Descricao { get; set; }

        [DataField("CON_C_CHAVE", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        public string Chave { get; set; }

        [DataField("CON_C_VALOR", SqlDbType.VarChar, 250)]
        public string Valor { get; set; }
    }
}
