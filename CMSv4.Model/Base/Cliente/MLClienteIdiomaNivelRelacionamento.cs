using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_JBR_CLI_CLIENTE_X_IDIOMA_X_NIVEL")]
    public class MLClienteIdiomaNivelRelacionamento
    {
        [DataField("CID_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CID_CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCliente { get; set; }

        [DataField("CID_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("CID_NII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoNivel { get; set; }
    }
}
