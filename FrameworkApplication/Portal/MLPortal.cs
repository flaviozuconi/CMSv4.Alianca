using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade
    /// </summary>  
    [DataContract]
    [Serializable]
    [Table("ADM_POR_PORTAL")]
    public class MLPortal
    {
        private string connectionstring;
        private string senhabd;

        [DataMember]
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_MEN_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoMenu { get; set; }

        [DataField("POR_MNV_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoMenuView { get; set; }

        [DataMember]
        [Required]
        [StringLength(100)]
        [DataField("POR_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^[a-zA-Z_\\-0-9]+$", ErrorMessage = "Não utilize espaços ou caracteres especiais no nome do diretório")]
        [DataField("POR_C_DIRETORIO", SqlDbType.VarChar, 20)]
        public string Diretorio { get; set; }

        [DataField("POR_C_URL", SqlDbType.VarChar, -1)]
        public string Url { get; set; }

        [DataField("POR_C_PAGESPEED_APIKEY", SqlDbType.VarChar, 300)]
        public string PageSpeedApiKey { get; set; }

        [DataField("POR_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("POR_B_MANUTENCAO", SqlDbType.Bit)]
        public bool? Manutencao { get; set; }

        [DataField("POR_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        [DataField("POR_B_RESTRITO_PUBLICO", SqlDbType.Bit)]
        public bool? RestritoAreaPublica { get; set; }

        [DataField("POR_C_ANALYTICS_USERNAME", SqlDbType.VarChar, 100)]
        public string AnalyticsUsername { get; set; }

        [DataField("POR_C_ANALYTICS_PASSWORD", SqlDbType.VarChar, 100)]
        public string AnalyticsPassword { get; set; }

        [DataField("POR_C_ANALYTICS_PROFILE_ID", SqlDbType.VarChar, 30)]
        public string AnalyticsProfileId { get; set; }

        /*[Required]
        [StringLength(1000)]
        [DataField("POR_C_CONNECTION_STRING", SqlDbType.VarChar, 1000)]*/
        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(ServidorBD) &&
                    !string.IsNullOrEmpty(UsuarioBD) &&
                    !string.IsNullOrEmpty(SenhaBD) &&
                    !string.IsNullOrEmpty(NomeBD))
                {
                    if (!string.IsNullOrEmpty(ServidorBDFailOver))
                    {
                        return string.Format("server={0};uid={1};pwd={2};database={3};failover partner={4}", ServidorBD, UsuarioBD, SenhaBD, NomeBD, ServidorBDFailOver);
                    }
                    
                    return string.Format("server={0};uid={1};pwd={2};database={3}", ServidorBD, UsuarioBD, SenhaBD, NomeBD);
                }
                else if (!string.IsNullOrEmpty(connectionstring))
                {
                    return connectionstring;
                }

                return string.Empty;
            }
            set
            {
                connectionstring = value;
            }
        }

        [DataField("POR_C_SERVIDORBD", SqlDbType.VarChar, 100)]
        public string ServidorBD { get; set; }

        [DataField("POR_C_SERVIDORBD_FAILOVER", SqlDbType.VarChar, 100)]
        public string ServidorBDFailOver { get; set; }

        [DataField("POR_C_USUARIOBD", SqlDbType.VarChar, 100)]
        public string UsuarioBD { get; set; }

        [DataField("POR_C_SENHABD", SqlDbType.VarChar, 100)]
        public string SenhaBDCript { get; set; }

        public string SenhaBD
        {
            get
            {
                if(!string.IsNullOrEmpty(SenhaBDCript))
                    return BLEncriptacao.DesencriptarAes(SenhaBDCript);

                return senhabd;
            }
            set
            {
                senhabd = value;
            }

        }

        [DataField("POR_C_NOMEBD", SqlDbType.VarChar, 100)]
        public string NomeBD { get; set; }

        [DataField("POR_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }
    }

    [Serializable]
    [Table("ADM_POR_PORTAL")]
    public class MLPortalComUsuarios : MLPortal
    {
        public MLPortalComUsuarios()
        {
            Usuarios = new List<MLUsuarioItemPortal>();
            Grupos = new List<MLPortalItemGrupo>();
        }

        public List<MLUsuarioItemPortal> Usuarios { get; set; }

        public List<MLPortalItemGrupo> Grupos { get; set; }
    }

    /// <summary>
    /// Grupos de gestão administrativa da seção
    /// </summary>
    [Serializable]
    [Table("CMS_PER_PERMISSAO_PORTAL")]
    public class MLPortalItemGrupo
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPortal { get; set; }

        [DataField("PER_GRP_N_CODIGO", SqlDbType.Decimal, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }

        public bool? Associado { get; set; }
    }

    /// <summary> 
    /// Model da Entidade Portais
    /// </summary> 
    [Serializable]
    [Table("ADM_PXM_PORTAL_X_MENU")]
    public class MLPortalMenu 
    {
        [DataField("PXM_MNI_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoMenuItem { get; set; }

        [DataField("PXM_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        //[JoinField("PXM_POR_N_CODIGO", "ADM_POR_PORTAL", "POR_N_CODIGO", "POR_C_NOME")]
        //[DataField("POR_C_NOME", SqlDbType.VarChar, 100)]
        //public string Nome { get; set; }

        //[JoinField("PXM_POR_N_CODIGO", "ADM_POR_PORTAL", "POR_N_CODIGO", "POR_C_DIRETORIO")]
        //[DataField("POR_C_DIRETORIO", SqlDbType.VarChar, 20)]
        //public string Diretorio { get; set; }

        //public bool? UsuarioAssociado { get; set; }
    }
}