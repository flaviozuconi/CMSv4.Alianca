using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_NEW_NEWSLETTER_HISTORICO")]
    public class MLModuloNewsletterHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [DataField("NEW_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("NEW_C_ASSUNTOS", SqlDbType.VarChar, -1)]
        public string Assuntos { get; set; }

        [DataField("NEW_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("NEW_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

}
