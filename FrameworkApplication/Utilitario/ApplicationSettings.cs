using System;
using System.Configuration;
using System.Web;

namespace Framework.Utilities
{
    public class ApplicationSettings
    {
        #region GetCachedSetting

        /// <summary>
        /// Obtem uma configuração e a armazena em cache para consultas futuras.
        /// </summary>
        /// <param name="key">nome da chave no AppSettings</param>
        /// <returns>configuracao</returns>
        private static string GetCachedSetting(string key)
        {
            if (ConfigurationManager.AppSettings[key] == null) return string.Empty;

            if (HttpContextFactory.Current.Cache[key] == null)
                HttpContextFactory.Current.Cache[key] = ConfigurationManager.AppSettings[key];

            return Convert.ToString(HttpContextFactory.Current.Cache[key]);
        }

        #endregion

        /// <summary>
        /// Obtem as connection strings do sistema
        /// </summary>
        public static class ConnectionStrings
        {
            #region Default

            /// <summary>
            /// Retorna a connections string padrão do sistema
            /// </summary>
            /// <returns>connection string armarmazenada</returns>
            public static string Default
            {
                get
                {
                    return Get("Default");
                }
            }

            /// <summary>
            /// Retorna a connections string padrão do sistema
            /// </summary>
            /// <returns>connection string armarmazenada</returns>
            public static string Get(string key)
            {
                string cacheKey = "ConnectionString" + key;
                string connString = BLEncriptacao.DesencriptarAes(ConfigurationManager.ConnectionStrings[key].ConnectionString);

                if (HttpContextFactory.Current == null)
                    return connString;
                else if (HttpContextFactory.Current.Cache[cacheKey] == null)
                    HttpContextFactory.Current.Cache[cacheKey] = connString;

                return Convert.ToString(HttpContextFactory.Current.Cache[cacheKey]);
            }

            #endregion
        }

        /// <summary>
        /// Configurações adicionais do banco SQL
        /// </summary>
        public static class SqlSettings
        {
            #region CommandTimeOut

            /// <summary>
            /// Tempo de espera para desistência da chamada do comando
            /// </summary>
            public static int CommandTimeOut
            {
                get
                {
                    const int valorPadrao = 30;
                    const string key = "SqlSettings.CommandTimeOut";

                    int valor;
                    int.TryParse(GetCachedSetting(key), out valor);

                    if (valor <= 0) return valorPadrao;
                    else return valor;
                }
            }

            #endregion
        }

        /// <summary>
        /// Configuração do Analytics
        /// </summary>
        public static class AnalyticsSettings
        {
            #region DataFeedUrl

            /// <summary>
            /// Retorna a url de onde serão buscadas as informações no Google Analytics (v2)
            /// </summary>
            public static string DataFeedUrl
            {
                get
                {
                    const string valorPadrao = "https://www.googleapis.com/analytics/v2.4/data";
                    const string key = "AnalyticsSettings.DataFeedUrl";

                    var valor = GetCachedSetting(key);
                    if (string.IsNullOrEmpty(valor)) return valorPadrao;

                    return valor;
                }
            }

            #endregion

            #region ServiceName

            /// <summary>
            /// Retorna o nome de serviço configurado no Google Analytics (v2)
            /// </summary>
            public static string ServiceName
            {
                get
                {
                    const string valorPadrao = "CMSAnalytics";
                    const string key = "AnalyticsSettings.ServiceName";

                    var valor = GetCachedSetting(key);
                    if (string.IsNullOrEmpty(valor)) return valorPadrao;

                    return valor;
                }
            }

            #endregion

        }
    }
}
