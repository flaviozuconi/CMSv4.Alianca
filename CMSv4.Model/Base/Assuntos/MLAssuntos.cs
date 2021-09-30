using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("CMS_ASU_ASSUNTO")]
    public class MLAssuntos
    {
        [DataField("ASU_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ASU_N_CODIGO_PORTAL", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("ASU_C_ASSUNTO", SqlDbType.VarChar, 50)]
        public string Assunto { get; set; }

        [DataField("ASU_C_CSS", SqlDbType.VarChar, 30)]
        public string Css { get; set; }

        [DataField("ASU_C_URL", SqlDbType.VarChar, 50)]
        public string Url { get; set; }

        [DataField("ASU_B_ATIVO", SqlDbType.Bit)]
        public bool? IsAtivo { get; set; }

    }
}
