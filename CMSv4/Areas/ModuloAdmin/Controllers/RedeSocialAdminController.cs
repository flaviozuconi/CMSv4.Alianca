using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class RedeSocialAdminController : SecurePortalController
    {
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            var _urlAuth = WebConfig("AUTH.LINKEDIN.URL_AUTH", "https://www.linkedin.com/uas/oauth2/authorization");
            var _parametersAuth = new List<string>();

            _parametersAuth.Add(string.Concat("response_type", "=", "code"));
            _parametersAuth.Add(string.Concat("scope", "=", "rw_company_admin%20w_share"));
            _parametersAuth.Add(string.Concat("client_id", "=", WebConfig("AUTH.LINKEDIN.CLIENT_ID")));
            _parametersAuth.Add(string.Concat("redirect_uri", "=", WebConfig("AUTH.LINKEDIN.REDIRECT_URI")));
            _parametersAuth.Add(string.Concat("state", "=", WebConfig("AUTH.LINKEDIN.STATE")));

            ViewBag.UrlAuth = string.Concat(_urlAuth, "?", string.Join("&", _parametersAuth));

            if (Request.QueryString["code"] != null)
            {
                var _urlToken = WebConfig("AUTH.LINKEDIN.URL_TOKEN", "https://www.linkedin.com/uas/oauth2/accessToken");
                var _parametersToken = new List<string>();

                _parametersToken.Add(string.Concat("grant_type", "=", "authorization_code"));
                _parametersToken.Add(string.Concat("code", "=", Request.QueryString["code"]));
                _parametersToken.Add(string.Concat("redirect_uri", "=", WebConfig("AUTH.LINKEDIN.REDIRECT_URI")));
                _parametersToken.Add(string.Concat("client_id", "=", WebConfig("AUTH.LINKEDIN.CLIENT_ID")));
                _parametersToken.Add(string.Concat("client_secret", "=", WebConfig("AUTH.LINKEDIN.SECRET")));

                ViewBag.UrlToken = string.Concat(_urlToken, "?", string.Join("&", _parametersToken));
            }

            return View();
        }

        #region NotImplemented

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Excluir folha de dados
        /// </summary>
        [CheckPermission(global::Permissao.Excluir)]
        public JsonResult Excluir(List<string> ids)
        {
            throw new NotImplementedException();
        }
        #endregion

        private string WebConfig(string key, string defaultValue = "")
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }

            return defaultValue;
        }
    }
}