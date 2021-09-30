using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_JBR_CLI_CLIENTE_AREA_INTERESSE")]
    public class MLClienteAreaInteresse
    {
        [DataField("ARI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ARI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("ARI_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
