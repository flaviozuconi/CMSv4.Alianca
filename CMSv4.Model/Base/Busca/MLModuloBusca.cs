using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLModuloBusca
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("BUS_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("BUS_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("BUS_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("BUS_C_VIEW", SqlDbType.VarChar, 150)]
        public string View { get; set; }

        [DataField("BUS_B_SETAS", SqlDbType.Bit)]
        public bool? IsSetas { get; set; }

        [DataField("BUS_B_AUTO_COMPLETE", SqlDbType.Bit)]
        public bool? IsAutoComplete { get; set; }

        [DataField("BUS_B_IMAGEM", SqlDbType.Bit)]
        public bool? ExibirImagem { get; set; }

        [DataField("BUS_N_QTDE_POR_PAGINA", SqlDbType.Int)]
        public int? QtdePorPagina { get; set; }

        [DataField("BUS_QTDE_PAGINADOR", SqlDbType.Int)]
        public int? QtdePaginador { get; set; }

        [DataField("BUS_C_URL_DETALHE", SqlDbType.VarChar, 500)]
        public string UrlDetalhe { get; set; }

        [DataField("BUS_C_TEXTO", SqlDbType.VarChar, 300)]
        public string TextoPlaceHolder { get; set; }

        public string TermoBusca { get; set; }

        public static string UrlResultadoBusca
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CMS.Busca.UrlResltados"] != null)
                    return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["CMS.Busca.UrlResltados"]);
                return string.Empty;
            }
        }
    }

    [Table("MOD_BUS_BUSCA_EDICAO")]
    public class MLModuloBuscaEdicao : MLModuloBusca { }

    [Table("MOD_BUS_BUSCA_PUBLICADO")]
    public class MLModuloBuscaPublicado : MLModuloBusca { }

    [Table("MOD_BUS_BUSCA_HISTORICO")]
    public class MLModuloBuscaHistorico : MLModuloBusca
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }

}
