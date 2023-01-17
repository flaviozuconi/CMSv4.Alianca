using System.Data;
using Framework.Model;
using System;

namespace CMSv4.Model
{
    [Table("MOD_FAL_FALE_CONOSCO_RELATORIOS")]
    public class MLFaleConoscoRelatorios
    {

        [DataField("FRE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("FRE_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("FRE_C_VIEW", SqlDbType.VarChar, 50)]
        public string View { get; set; }

        [DataField("FRE_C_NOME_ARQUIVO", SqlDbType.VarChar, 50)]
        public string NomeArquivo { get; set; }

        [DataField("FRE_C_GRUPOS", SqlDbType.VarChar, 50)]
        public string Grupos { get; set; }

       
    }
}
