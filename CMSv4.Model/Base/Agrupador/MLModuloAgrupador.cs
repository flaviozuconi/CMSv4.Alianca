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
    public class MLModuloAgrupador
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("AGM_AGR_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoAgrupador { get; set; }

        [StringLength(100)]
        [DataField("AGM_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("AGM_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("AGM_C_CATEGORIAS", SqlDbType.VarChar, 100)]
        public string Categorias { get; set; }

        [DataField("AGM_LIS_C_CODIGOS", SqlDbType.VarChar, 100)]
        public string Modulos { get; set; }

        [DataField("AGM_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [DataField("AGM_USU_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("AGM_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("AGM_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("AGM_CAA_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoCategoriaAgrupador { get; set; }

        [DataField("AGM_B_DESTAQUES", SqlDbType.Bit)]
        public bool? Destaques { get; set; }
    }

    [Table("MOD_AGR_AGRUPADOR_EDICAO")]
    public class MLModuloAgrupadorEdicao : MLModuloAgrupador { }

    [Table("MOD_AGR_AGRUPADOR_PUBLICADO")]
    public class MLModuloAgrupadorPublicado : MLModuloAgrupador { }

    [Table("MOD_AGR_AGRUPADOR_HISTORICO")]
    public class MLModuloAgrupadorHistorico : MLModuloAgrupador
    {

        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

    }
}
