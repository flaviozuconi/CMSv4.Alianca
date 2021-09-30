using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_JBR_CLI_CLIENTE_X_AREA_INTERESSE")]
    public class MLClienteAreaInteresseRelacionamento
    {
        [DataField("CAR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CAR_CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCliente { get; set; }

        [DataField("CAR_ARI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoAreaInteresse { get; set; }
    }
}
