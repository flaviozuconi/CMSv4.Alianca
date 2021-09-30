using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// NOTICIA
    /// </summary>
    [Serializable]
    [Table("MOD_NOT_NOTICIAS")]
    public class MLNoticiaLista 
    {
        [DataField("NOT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("NOT_NCA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

        [DataField("NOT_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("NOT_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("NOT_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("NOT_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("NOT_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("NOT_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("NCA_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeCategoria { get; set; }

        [DataField("ROWNUMBER", SqlDbType.BigInt)]
        public Int64? RowNumber { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public Int32? TotalRows { get; set; }

        public string DataString
        {
            get
            {
                if (Data.HasValue) return Data.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
                return "";
            }
        }
    }
}
