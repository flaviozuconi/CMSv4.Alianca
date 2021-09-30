using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Table("CMS_COE_COMPARTILHADO_EMAIL")]
    public class MLCompartilharEmail
    {
        [DataField("COE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("COE_CET_N_CODIGO", SqlDbType.Int)]
        public int? CodigoTipo { get; set; }

        [DataField("COE_LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("COE_CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCliente { get; set; }

        [DataField("COE_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("COE_C_NOME", SqlDbType.VarChar, 250)]
        public string Nome { get; set; }

        [DataField("COE_C_EMAIL", SqlDbType.VarChar, 250)]
        public string Email { get; set; }

        [DataField("COE_C_AMIGO_NOME", SqlDbType.VarChar, 250)]
        public string NomeAmigo { get; set; }

        [DataField("COE_C_AMIGO_EMAIL", SqlDbType.VarChar, 250)]
        public string EmailAmigo { get; set; }

        [DataField("COE_C_COMENTARIO", SqlDbType.VarChar, 500)]
        public string Comentario { get; set; }

        [DataField("COE_LIT_C_URL", SqlDbType.VarChar, 300)]
        public string UrlCompartilhada { get; set; }

        public string UrlSite { get; set; }

        public enum Tipos
        {
            Noticia = 1
        }
    }
}
