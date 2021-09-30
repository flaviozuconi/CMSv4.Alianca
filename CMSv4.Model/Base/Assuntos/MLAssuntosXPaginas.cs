using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("CMS_AXP_ASSUNTO_X_PAGINA")]
    public class MLAssuntosXPaginas
    {
        [DataField("AXP_PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("AXP_ASU_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoAssunto { get; set; }

    }
}
