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
    public class MLModuloMenuModulo
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("MEN_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoMenu { get; set; }

        [StringLength(50)]
        [DataField("MEN_C_CLASSE_CSS", SqlDbType.VarChar, 50)]
        public string ClasseCSS { get; set; }

        [DataField("MEN_C_ADICIONAL", SqlDbType.VarChar, 500)]
        public string Adicional { get; set; }

        [DataField("MEN_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("MEN_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("MEN_C_VIEW", SqlDbType.VarChar, 250)]
        public string View { get; set; }
    }

    [Table("MOD_MEN_MENU_EDICAO")]
    public class MLModuloMenuEdicao : MLModuloMenuModulo { }

    [Table("MOD_MEN_MENU_PUBLICADO")]
    public class MLModuloMenuPublicado : MLModuloMenuModulo { }

}
