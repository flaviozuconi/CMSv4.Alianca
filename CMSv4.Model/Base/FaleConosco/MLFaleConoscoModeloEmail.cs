using Framework.Model;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_FAL_FALE_CONOSCO_MODELO_EMAIL")]
    public class MLFaleConoscoModeloEmail
    {

        [DataField("FME_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("FME_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("FME_C_NOME", SqlDbType.VarChar, 256)]
        public string Nome { get; set; }

        [DataField("FME_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

    }
}
