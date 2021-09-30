using System;
using Framework.Utilities;

/// <summary>
/// PortalAtual.Obter USAR NA ÁREA ADMINISTRATIVA
/// </summary>
public static class PortalAtual
{
    #region Obter

    /// <summary>
    /// Portal Atual
    /// </summary>
    public static MLPortal Obter
    {
        get 
        {
            var portal = BLPortal.Portais.Find(a => a.Diretorio.Equals(BLPortal.Url, StringComparison.InvariantCultureIgnoreCase));
            
            return portal;
        }
    }

    #endregion

    #region Url

    /// <summary>
    /// Retorna a URL do portal atual baseado na URL atual, ou
    /// na URLReferer
    /// </summary>
    /// <returns></returns>
    public static string Url
    {
        get
        {
            var portal = BLPortal.Url;
            
            if (!string.IsNullOrEmpty(portal) && !portal.StartsWith(BLPortal.PORTAL_PREFIX))
                return string.Concat(BLPortal.PORTAL_PREFIX, "/", portal);

            return string.Empty;
        }
    }

    #endregion

    #region Diretorio

    /// <summary>
    /// Retorna o nome da pasta do portal atual baseado na URL atual, ou na URLReferer
    /// </summary>
    public static string Diretorio
    {
        get
        {
            return BLPortal.Url;
        }
    }

    #endregion

    #region ConnectionString

    /// <summary>
    /// Retorna a URL do portal atual baseado na URL atual, ou
    /// na URLReferer
    /// </summary>
    /// <returns></returns>
    public static string ConnectionString
    {
        get
        {
            var atual = Obter;

            if (atual != null)
                return atual.ConnectionString;
            else
                return string.Empty;
        }
    }

    #endregion

    #region Codigo

    /// <summary>
    /// Retorna o codigo do portal atual baseado na URL
    /// </summary>
    /// <returns></returns>
    public static decimal? Codigo
    {
        get
        {
            var atual = Obter;

            if (atual != null)
                return atual.Codigo;
            else
                return null;
        }
    }

    #endregion
}