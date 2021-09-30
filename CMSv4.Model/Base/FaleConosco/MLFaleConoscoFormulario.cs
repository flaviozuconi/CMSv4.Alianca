using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_FAL_FALE_CONOSCO_FORMULARIOS")]
    public class MLFaleConoscoFormulario
    {
        [DataField("FCA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("FCA_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("FCA_C_NOME", SqlDbType.VarChar, 256)]
        public string Nome { get; set; }

        [DataField("FCA_C_ASSUNTO", SqlDbType.VarChar, 256)]
        public string Assunto { get; set; }

        [DataField("FCA_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("FCA_C_SCRIPT", SqlDbType.VarChar, -1)]
        public string Script { get; set; }

        [DataField("FCA_C_EMAIL_DESTINATARIO", SqlDbType.VarChar, 200)]
        public string EmailDestinatario { get; set; }

        /// <summary>
        /// Separar por vírgula para mais de um e-mail
        /// </summary>
        [DataField("FCA_C_EMAIL_COPIA", SqlDbType.VarChar, -1)]
        public string EmailsCopia { get; set; }
    }
}
