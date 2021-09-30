using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLModuloEventos
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("EVE_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("EVE_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }
                
        [DataField("EVE_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [StringLength(100)]
        [DataField("EVE_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [StringLength(100)]
        [DataField("EVE_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }
        
        [DataField("EVE_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("EVE_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [StringLength(100)]
        [DataField("EVE_C_PAGINACAO", SqlDbType.VarChar, 100)]
        public string TipoPaginacao { get; set; }

        public int Pagina { get; set; }
        public int? Ano { get; set; }
        public int? Semestre { get; set; }
    }

    [Table("MOD_EVE_EVENTO_EDICAO")]
    public class MLModuloEventosEdicao : MLModuloEventos { }

    [Table("MOD_EVE_EVENTO_PUBLICADO")]
    public class MLModuloEventosPublicado : MLModuloEventos { }

}
