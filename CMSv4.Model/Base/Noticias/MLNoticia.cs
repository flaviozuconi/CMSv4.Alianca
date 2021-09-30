using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// NOTICIA
    /// </summary>
    [Serializable]
    [Table("MOD_NOT_NOTICIAS")]
    public class MLNoticia: BaseModel
    {
        [DataField("NOT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("NOT_NCA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

        [Required]
        [DataField("NOT_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("NOT_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [Required]
        [DataField("NOT_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("NOT_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("NOT_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("NOT_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("NOT_D_APROVACAO", SqlDbType.DateTime)]
        public DateTime? DataAprovacao { get; set; }

        [DataField("NOT_USU_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioAprovacao { get; set; }

        [DataField("NOT_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("NOT_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("NOT_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }


        [DataField("NOT_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Idioma { get; set; }


     
        

        // JOIN

        [JoinField("NOT_NCA_N_CODIGO", "MOD_NOT_CATEGORIAS_NOTICIA", "NCA_N_CODIGO", "NCA_C_NOME")]
        [DataField("NCA_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeCategoria { get; set; }

        [JoinField("NOT_NCA_N_CODIGO", "MOD_NOT_CATEGORIAS_NOTICIA", "NCA_N_CODIGO", "POR_N_CODIGO")]
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [JoinField("NOT_IDI_N_CODIGO", "FWK_IDI_IDIOMA", "IDI_N_CODIGO", "IDI_C_NOME")]
        [DataField("IDI_C_NOME", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string IdiomaNome { get; set; }


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
