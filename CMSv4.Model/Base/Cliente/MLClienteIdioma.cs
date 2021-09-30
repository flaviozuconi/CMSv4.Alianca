using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_JBR_CLI_CLIENTE_IDIOMAS")]
    public class MLClienteIdioma
    {
        [DataField("IDI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("IDI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("IDI_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
