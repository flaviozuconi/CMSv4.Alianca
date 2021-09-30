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
    [Table("MOD_MEN_MENU_HISTORICO")]
    public class MLModuloMenuHistoricoModulo
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [DataField("MEN_N_CODIGO", SqlDbType.Decimal)]
        public int? CodigoMenu { get; set; }

        [StringLength(50)]
        [DataField("MEN_C_CLASSE_CSS", SqlDbType.VarChar, 50)]
        public string ClasseCSS { get; set; }

        [DataField("MEN_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("MEN_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }
}

