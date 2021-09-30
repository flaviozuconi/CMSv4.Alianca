using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_EVE_EVENTO_HISTORICO")]
    public class MLModuloEventosHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [DataField("EVE_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("EVE_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }
                
        [DataField("EVE_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [DataField("EVE_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [DataField("EVE_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }


        [DataField("EVE_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("EVE_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }
}
