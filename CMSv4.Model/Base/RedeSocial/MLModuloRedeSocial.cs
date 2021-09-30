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
    public class MLModuloRedeSocial
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("RSO_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [StringLength(100)]
        [DataField("RSO_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("RSO_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [DataField("RSO_N_LIMITE_CHAR", SqlDbType.Int)]
        public int? LimiteChar { get; set; }

        [DataField("RSO_B_FACEBOOK", SqlDbType.Bit)]
        public bool? IsFacebook { get; set; }

        [DataField("RSO_B_TWITTER", SqlDbType.Bit)]
        public bool? IsTwitter { get; set; }

        [DataField("RSO_B_LINKEDIN", SqlDbType.Bit)]
        public bool? IsLinkedin { get; set; }
        
        [StringLength(250)]
        [DataField("RSO_C_URL_LISTA", SqlDbType.VarChar, 250)]
        public string UrlLista { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_FAC_APPID", SqlDbType.VarChar, 250)]
        public string FaceAppID { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_FAC_APPIDSECRET", SqlDbType.VarChar, 250)]
        public string FaceAppIDSecret { get; set; }

        [StringLength(50)]
        [DataField("RSO_C_FAC_PAGINA", SqlDbType.VarChar, 50)]
        public string IdPagina { get; set; }

        [StringLength(50)]
        [DataField("RSO_C_TWI_PAGINA", SqlDbType.VarChar, 50)]
        public string TwitterPagina { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_TWI_CONSUMER", SqlDbType.VarChar, 250)]
        public string TwitterConsumer { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_TWI_CONSUMER_SECRET", SqlDbType.VarChar, 250)]
        public string TwitterConsumerSecret { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_TWI_TOKEN", SqlDbType.VarChar, 250)]
        public string TwitterToken { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_TWI_TOKEN_SECRET", SqlDbType.VarChar, 250)]
        public string TwitterTokenSecret { get; set; }

        [StringLength(50)]
        [DataField("RSO_C_LIK_COMPANY", SqlDbType.VarChar, 50)]
        public string LinkedinCompany { get; set; }

        [StringLength(250)]
        [DataField("RSO_C_LIK_TOKEN", SqlDbType.VarChar, 250)]
        public string LinkedinToken { get; set; }

        [DataField("RSO_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("RSO_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }

    [Table("MOD_RSO_REDE_SOCIAL_EDICAO")]
    public class MLModuloRedeSocialEdicao : MLModuloRedeSocial { }

    [Table("MOD_RSO_REDE_SOCIAL_PUBLICADO")]
    public class MLModuloRedeSocialPublicado : MLModuloRedeSocial { }

    [Table("MOD_RSO_REDE_SOCIAL_HISTORICO")]
    public class MLModuloRedeSocialHistorico : MLModuloRedeSocial {

        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    
    }

}
