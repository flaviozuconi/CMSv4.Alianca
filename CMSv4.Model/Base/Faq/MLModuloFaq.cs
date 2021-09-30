using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    /// <summary>
    /// Módulo de FAQ
    /// </summary>
    [Serializable]
    [Table("MOD_FAQ_FAQ_EDICAO")]
    public class MLModuloFaqEdicao
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [Required]
        [DataField("FAQ_C_TITULO", SqlDbType.VarChar, 50)]
        public string Titulo { get; set; }

        [Required]
        [DataField("FAQ_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string Descricao { get; set; }

        [Required]
        [DataField("FAQ_C_CATEGORIAS", SqlDbType.VarChar, -1)]
        public string Categorias { get; set; }

        [DataField("FAQ_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("FAQ_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

    [Table("MOD_FAQ_FAQ_PUBLICADO")]
    public class MLModuloFaqPublicado : MLModuloFaqEdicao { }

    [Table("MOD_FAQ_FAQ_HISTORICO")]
    public class MLModuloFaqHistorico : MLModuloFaqEdicao
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
