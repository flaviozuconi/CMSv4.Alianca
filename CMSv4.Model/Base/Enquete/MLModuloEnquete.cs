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
    public class MLModuloEnquete
    {
        public MLModuloEnquete()
        {
            Enquete = new MLEnqueteResultado();
        }

        [StringLength(100)]
        [DataField("ENQ_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("ENQ_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEnquete { get; set; }

        [DataField("ENQ_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("ENQ_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("ENQ_C_VIEW", SqlDbType.VarChar, 200)]
        public string View { get; set; }

        [JoinField("ENQ_N_CODIGO", "MOD_ENQ_ENQUETE", "ENQ_N_CODIGO", "ENQ_D_INICIO")]
        [DataField("ENQ_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [JoinField("ENQ_N_CODIGO", "MOD_ENQ_ENQUETE", "ENQ_N_CODIGO", "ENQ_D_TERMINO")]
        [DataField("ENQ_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataFim { get; set; }

        [DataField("ENQ_B_VOTAR_RESTRITO", SqlDbType.Bit)]
        public bool? VotarRestrito { get; set; }

        [DataField("ENQ_B_RESULTADO_RESTRITO", SqlDbType.Bit)]
        public bool? ResultadoRestrito { get; set; }

        public MLEnqueteResultado Enquete { get; set; }
    }
    
    [Table("MOD_ENQ_ENQUETE_EDICAO")]
    public class MLModuloEnqueteEdicao : MLModuloEnquete {}

    [Table("MOD_ENQ_ENQUETE_PUBLICADO")]
    public class MLModuloEnquetePublicado : MLModuloEnquete {}

    [Table("MOD_ENQ_ENQUETE_HISTORICO")]
    public class MLModuloEnqueteHistorico : MLModuloEnquete
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}