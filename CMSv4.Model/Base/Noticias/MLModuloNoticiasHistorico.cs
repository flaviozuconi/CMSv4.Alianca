using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_NOT_NOTICIA_HISTORICO")]
    public class MLModuloNoticiasHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [DataField("NOT_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("NOT_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }
             
        [DataField("NOT_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [DataField("NOT_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [DataField("NOT_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }


        [DataField("NOT_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("NOT_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }
}
