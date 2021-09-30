using Framework.Model;
using Framework.Utilities;
using System;
using System.Data;

namespace CMSv4.Model
{
    public class MLListaConteudoPublico
    {
    }
    public class MLListaConteudoPublicoListagem : MLListaConteudo
    {
        [DataField("ROWNUMBER", SqlDbType.BigInt)]
        public Int64? RowNumber { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public Int32? TotalRows { get; set; }

        [DataField("LIT_C_URL_ORIGINAL", SqlDbType.VarChar, 250)]
        public string UrlOriginal { get; set; }

        [DataField("LIC_C_NOME_CATEGORIA", SqlDbType.VarChar, 100)]
        public string Categoria { get; set; }

        public string DataString
        {
            get
            {
                if (Data.HasValue) return Data.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
                return "";
            }
        }
    }
    
    public class MLListaEventosCalendario
    {
        public string id { get; set; }

        public string title { get; set; }

        public string start { get; set; }       

        public string color { get; set; }

        public string display { get; set; }
    }
    public class MLListaPortosCalendarios
    {
        public string servico { get; set; }

        public string porto { get; set; }
     
    }

}
