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
    public class MLModuloNoticias
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("NOT_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("NOT_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("NOT_C_CATEGORIAS", SqlDbType.VarChar, 100)]
        public string Categorias { get; set; }

        [DataField("NOT_B_DESTAQUES", SqlDbType.Bit)]
        public bool? Destaques { get; set; }

        [DataField("NOT_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [StringLength(100)]
        [DataField("NOT_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [StringLength(100)]
        [DataField("NOT_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }


        [DataField("NOT_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("NOT_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }

    [Table("MOD_NOT_NOTICIA_EDICAO")]
    public class MLModuloNoticiasEdicao : MLModuloNoticias { }

    [Table("MOD_NOT_NOTICIA_PUBLICADO")]
    public class MLModuloNoticiasPublicado : MLModuloNoticias { }

}
