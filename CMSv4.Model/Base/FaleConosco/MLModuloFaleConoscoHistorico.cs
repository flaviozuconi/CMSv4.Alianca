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
    [Table("MOD_FAL_FALE_CONOSCO_HISTORICO")]
    public class MLModuloFaleConoscoHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("FAL_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("FAL_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("FAL_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("FAL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }
}
