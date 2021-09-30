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
    public class MLModuloResultado
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("RES_C_VIEW", SqlDbType.VarChar, 100)]
        public string View { get; set; }

        [StringLength(100)]
        [DataField("RES_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [StringLength(50)]
        [DataField("RES_C_URL_DETALHE", SqlDbType.VarChar, 250)]
        public string UrlDetalhe { get; set; }

        [DataField("RES_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("RES_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }


    }

    [Table("MOD_RES_RESULTADO_EDICAO")]
    public class MLModuloResultadoEdicao : MLModuloResultado { }

    [Table("MOD_RES_RESULTADO_PUBLICADO")]
    public class MLModuloResultadoPublicado : MLModuloResultado { }

}
