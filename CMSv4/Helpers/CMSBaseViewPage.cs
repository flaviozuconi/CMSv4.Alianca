using System.Web;
public abstract class CMSBaseViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
{
    /// <summary>
    /// Tradução de Termos
    /// </summary>
    /// <param name="texto">Texto original</param>
    /// <returns>Termo traduzido</returns>
    public static string T(string texto)
    {        
        var T = new Framework.Utilities.BLTraducao(Framework.Utilities.BLPortal.Atual);        
        return T.Obter(texto);
    }

    public static string TAdm(string texto)
    {
        var T = new Framework.Utilities.BLTraducao();
        return T.ObterAdm(texto);
    }
    
}