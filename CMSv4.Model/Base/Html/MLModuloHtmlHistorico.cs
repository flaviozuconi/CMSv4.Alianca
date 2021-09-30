using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_HTM_HTML_HISTORICO")]
    public class MLModuloHtmlHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("HTM_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("HTM_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("HTM_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("HTM_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }
}
