using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// PAGINA
    /// </summary>
    [Serializable]
    [Table("CMS_PAG_PAGINA")]
    public class MLPagina : BaseModel
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("PAG_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required]
        [DataField("PAG_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("PAG_SEC_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoSecao { get; set; }

        [Required]
        [DataField("PAG_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("PAG_B_EXCLUIDA", SqlDbType.Bit)]
        public bool? Excluida { get; set; }

        [DataField("PAG_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        [DataField("PAG_D_REGISTRO_EDICAO", SqlDbType.DateTime, IgnoreEmpty=true)]
        public DateTime? DataEdicao { get; set; }

        [DataField("PAG_C_TITULO_EDICAO", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        public string TituloEdicao { get; set; }
        
        [DataField("PAG_D_REGISTRO_PUBLICACAO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataPublicacao { get; set; }

        [DataField("PAG_C_TITULO_PUBLICACAO", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        public string TituloPublicacao { get; set; }

        [DataField("PAG_B_PAGINA_PRINCIPAL", SqlDbType.Bit)]
        public bool? PaginaPrincipal { get; set; }

        [DataField("PAG_B_HTTPS", SqlDbType.Bit)]
        public bool? Https { get; set; }



        [DataField("PAG_IDI_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("PAG_PAG_N_CODIGO", SqlDbType.Decimal, 18,IgnoreEmpty = true)]
        public decimal? CodigoPai { get; set; }

        [DataField("PAG_C_NOME_LAYOUT", SqlDbType.VarChar, 250)]
        public string NomeLayout { get; set; }

        [DataField("PAG_C_NOME_TEMPLATE", SqlDbType.VarChar, 250)]
        public string NomeTemplate { get; set; }

        // JOIN

        [JoinField("PAG_SEC_N_CODIGO", "CMS_SEC_SECAO", "SEC_N_CODIGO", "SEC_C_NOME")]
        [DataField("SEC_C_NOME", SqlDbType.VarChar, 50)]
        public string NomeSecao { get; set; }

        [DataField("PAG_C_DESCRICAO", SqlDbType.VarChar, 250)]
        public string Descricao { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }


        //Idiomas relacionados a este registros 
        //ex. string contem codigo,idioma# 

        [DataField("IDIOMAS", SqlDbType.VarChar, -1, IgnoreEmpty=true)]
        public string Idiomas { get; set; }


        public string Titulo
        { get { return TituloEdicao ?? TituloPublicacao ?? Nome; } }
    }

}
