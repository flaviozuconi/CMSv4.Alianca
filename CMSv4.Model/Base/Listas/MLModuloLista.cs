using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLModuloLista
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("LIS_CODIGO_LISTA", SqlDbType.Decimal)]
        public decimal? CodigoLista { get; set; }

        [StringLength(100)]
        [DataField("LIS_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("LIS_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("LIS_C_CATEGORIAS", SqlDbType.VarChar, 100)]
        public string Categorias { get; set; }

        [DataField("LIS_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [StringLength(100)]
        [DataField("LIS_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [StringLength(100)]
        [DataField("LIS_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }

        [DataField("LIS_USU_N_CODIGO", SqlDbType.DateTime)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("LIS_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

    [Table("MOD_LIS_LISTA_EDICAO")]
    public class MLModuloListaEdicao : MLModuloLista { }

    [Table("MOD_LIS_LISTA_PUBLICADO")]
    public class MLModuloListaPublicado : MLModuloLista { }

    [Table("MOD_LIS_LISTA_HISTORICO")]
    public class MLModuloListaHistorico : MLModuloLista
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

    }



}
