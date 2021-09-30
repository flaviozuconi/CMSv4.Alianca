using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Framework.Utilities
{
    /// <summary>
    /// Controla as funcionalidades e itens no cache do portal
    /// </summary>
    public class BLCachePortal
    {
        #region Funcoes Basicas

        /// <summary>
        /// Obter
        /// </summary>
        public static Tipo Get<Tipo>(string cacheKey)
        {
            try
            {
                return (Tipo)HttpContextFactory.Current.Cache.Get(cacheKey);
            }
            catch
            {
                return Activator.CreateInstance<Tipo>();
            }
        }

        /// <summary>
        /// Adicionar com cache de UMA HORA
        /// </summary>
        public static object Add(string cacheKey, object value)
        {
            return Add(cacheKey, value, 60);
        }

        /// <summary>
        /// Limpar o cache da chave especificada
        /// </summary>
        /// <param name="cachekey"></param>
        public static void Remove(string cachekey)
        {
            if (HttpContextFactory.Current.Cache.Get(cachekey) != null)
                HttpContextFactory.Current.Cache.Remove(cachekey);
        }

        public static void RemoveAll()
        {
            RemoveAll("");
        }

        /// <summary>
        /// Remove todas as chaves que contenham a string passada por parametro
        /// </summary>
        /// <param name="strChave"></param>
        public static void RemoveAll(string strChave)
        {
            foreach (DictionaryEntry entry in HttpContextFactory.Current.Cache)
                if (string.IsNullOrEmpty(strChave) || entry.Key.ToString().Contains(strChave))
                    HttpContextFactory.Current.Cache.Remove(entry.Key.ToString());

        }
        /// <summary>
        /// Adicionar ao cache pela quantidade de horas informado
        /// </summary>
        public static object Add(string cacheKey, object value, int horas)
        {
            try
            {
                return HttpContextFactory.Current.Cache.Add(cacheKey, value, null, DateTime.Now.AddMinutes(horas), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            catch { return null; }
        }

        #endregion

        #region Funcoes de Cache Associadas ao Portal

        /// <summary>
        /// Inclui um item em cache para ser o associador da lista de cache,
        /// se esse tem não for incluído, os caches dependentes também não serão
        /// </summary>
        public static object AtivarCachePortal(decimal codigoPortal)
        {
            return HttpContextFactory.Current.Cache.Add("portal_" + codigoPortal, true, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0, 0), CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// Reseta o cache do portal, isso causa que os CACHES em dependência
        /// também sejam expirados
        /// </summary>
        public static object ResetarCachePortal(decimal codigoPortal)
        {
            HttpContextFactory.Current.Cache.Remove("portal_" + codigoPortal);
            return AtivarCachePortal(codigoPortal);
        }

        /// <summary>
        /// Adiciona um item em cache por UMA HORA, dependente do cache do portal
        /// </summary>
        public static object Add(decimal codigoPortal, string cacheKey, object value)
        {
            return Add(codigoPortal, cacheKey, value, 1);
        }

        /// <summary>
        /// Adiciona um item em cache por algumas horas, dependente do cache do portal
        /// </summary>
        public static object Add(decimal codigoPortal, string cacheKey, object value, int horas)
        {
            var dependency = "portal_" + codigoPortal;
            try
            {
                AtivarCachePortal(codigoPortal);
                return HttpContextFactory.Current.Cache.Add(cacheKey, value, new CacheDependency(null, new[] { dependency }), DateTime.Now.AddHours(horas), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            catch { return null; }
        }

        public static void LimparCache(string chave)
        {
            if (HttpContextFactory.Current.Cache[chave] != null)
                HttpContextFactory.Current.Cache.Remove(chave);
            
        }


        #endregion
    }
}
