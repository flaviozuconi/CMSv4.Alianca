using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// Módulo de FAQ
    /// </summary>
    [Serializable]
    public class MLModuloColaborador
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("COL_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [Required]
        [DataField("COL_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("COL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }        

        [Required]
        [DataField("COL_C_GRUPO_CLIENTE", SqlDbType.VarChar, -1)]
        public string GrupoCliente { get; set; }

        [Required]
        [DataField("COL_C_LOJA_LOTACAO", SqlDbType.VarChar, -1)]
        public string LojaLotacao { get; set; }

        [DataField("COL_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }
    }

    [Table("MOD_COL_COLABORADOR_EDICAO")]
    public class MLModuloColaboradorEdicao : MLModuloColaborador { }

    [Table("MOD_COL_COLABORADOR_PUBLICADO")]
    public class MLModuloColaboradorPublicado : MLModuloColaborador { }

    [Table("MOD_COL_COLABORADOR_HISTORICO")]
    public class MLModuloColaboradorHistorico : MLModuloColaborador
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
