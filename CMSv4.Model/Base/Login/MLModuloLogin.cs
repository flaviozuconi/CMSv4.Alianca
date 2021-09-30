using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;
using CMSv4.Model;

namespace CMSv4.Model
{
    [Serializable]
    public class MLModuloLogin
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("LOG_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("LOG_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("LOG_B_LOGIN_PORTAL", SqlDbType.Bit)]
        public bool? LoginPortal { get; set; }

        [DataField("LOG_C_VIEW", SqlDbType.VarChar, 150)]
        public string View { get; set; }

        [DataField("LOG_C_FACEBOOK_APPID", SqlDbType.VarChar, 150)]
        public string FacebookAppId { get; set; }

        [DataField("LOG_C_FACEBOOK_APPID_SECRET", SqlDbType.VarChar, 150)]
        public string FacebookAppSecret { get; set; }

        public string UrlPaginaPerfil
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CMS.ModuloLogin.UrlPerfil"] != null)
                    return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["CMS.ModuloLogin.UrlPerfil"]);
                return string.Empty;
            }
        }

        public bool? EscolherPortal
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CMS.ModuloLogin.ExibirPortais"] != null)
                    return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["CMS.ModuloLogin.ExibirPortais"]);
                return null;
            }
        }

        public string UrlPaginaLogin { get; set; }

        public string Email { get; set; }

        public string UrlRetorno { get; set; }

        public string Token { get; set; }
    }

    [Table("MOD_LOG_LOGIN_EDICAO")]
    public class MLModuloLoginEdicao : MLModuloLogin { }

    [Table("MOD_LOG_LOGIN_PUBLICADO")]
    public class MLModuloLoginPublicado : MLModuloLogin { }

    [Serializable]
    [Table("MOD_LOG_LOGIN_HISTORICO")]
    public class MLModuloLoginHistorico : MLModuloLogin 
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
