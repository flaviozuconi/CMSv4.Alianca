using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_LTP_CLI_CLIENTE_ASSUNTO_INTERESSE")]
    public class MLClienteXAgrupador
    {
        [DataField("CAI_CLI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoCliente { get; set; }

        [DataField("CAI_CAA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoAgrupador { get; set; }
    }
}
