using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// Funções de facilitação para a área administrativa
/// </summary>
public static class AdminHelper
{
    #region CheckPermission

    /// <summary>
    /// Verifica permissão do usuário da página atual da requisição
    /// </summary>
    public static bool CheckPermission(Permissao permisssao)
    {
        var usuario = BLUsuario.ObterLogado();

        if (usuario == null) return false;

        return usuario.CheckPermissao(permisssao, HttpContext.Current.Request.Url.LocalPath);
    }

    /// <summary>
    /// Verifica permissão do usuário da página atual da requisição
    /// </summary>
    public static bool CheckPermission(Permissao permisssao, string url)
    {
        var usuario = BLUsuario.ObterLogado();

        if (usuario == null) return false;

        return usuario.CheckPermissao(permisssao, url);
    }

    #endregion

    #region IsDebugEnabled

    /// <summary>
    /// Retorna se o Web.Config está marcado como DEBUG ou não
    /// </summary>
    public static bool IsDebugEnabled
    {
        get {
            CompilationSection compilationSection = (CompilationSection)ConfigurationManager.GetSection(@"system.web/compilation");
            return compilationSection.Debug;
        }
    }

    #endregion

    public static List<MLPaginaPublicada> ListarPaginasPublicadas(string busca)
    {
        return CRUD.Listar(new MLPaginaPublicada() { Titulo = busca }, PortalAtual.ConnectionString);
    }

    public static bool ExcluirArquivosReplicadosClientes(string nomeImagem)
    {
        var diretorio = string.Concat(BLConfiguracao.Pastas.ClientesPortal("[[PORTAL]]"));
        return ExcluirArquivosReplicados(nomeImagem, diretorio);
    }

    private static bool ExcluirArquivosReplicados(string nomeImagem, string diretorio)
    {
        try
        {
            var listaPortais = BLPortal.Portais;

            for (int i = 0; i < listaPortais.Count; i++)
            {
                var imagem = string.Concat(diretorio.Replace("[[PORTAL]]", listaPortais[i].Diretorio), nomeImagem);

                FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath(imagem));
                if (fi.Exists)
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch { }
                }
            }

            return true;
        }
        catch (System.Exception ex)
        {
            ApplicationLog.ErrorLog(ex);

            return false;
        }
    }

    public static bool ReplicarArquivoClientes(this System.Web.Helpers.WebImage imagem, decimal? codigo)
    {
        var diretorio = string.Concat(BLConfiguracao.Pastas.ClientesPortal("[[PORTAL]]"));
        
        return ReplicarArquivo(imagem, codigo, diretorio);
    }

    private static bool ReplicarArquivo(this System.Web.Helpers.WebImage imagem, decimal? codigo, string diretorio)
    {
        var ext = ".png";

        try
        {
            var listaPortais = BLPortal.Portais;

            for (int i = 0; i < listaPortais.Count; i++)
            {
                var dirPortal = diretorio.Replace("[[PORTAL]]", listaPortais[i].Diretorio);
                var nomeImagem = string.Concat(dirPortal, codigo, ext);
                var di = new DirectoryInfo(HttpContext.Current.Server.MapPath(dirPortal));
                var di_thumb = new DirectoryInfo(HttpContext.Current.Server.MapPath(string.Concat(dirPortal, "/_thumb")));
                var fi = new FileInfo(HttpContext.Current.Server.MapPath(nomeImagem));

                if (!di.Exists)
                {
                    di.Create();
                }

                if (fi.Exists)
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch { }
                }

                //excluir thumbs de imagens anteriores
                if (di_thumb.Exists)
                {
                    try
                    {
                        di_thumb.Delete(true);
                    }
                    catch { }
                }

                imagem.Save(HttpContext.Current.Server.MapPath(nomeImagem), imageFormat: "png");
                BLReplicar.Arquivo(HttpContext.Current.Server.MapPath(nomeImagem));
            }

            return true;
        }
        catch (System.Exception ex)
        {
            ApplicationLog.ErrorLog(ex);

            return false;
        }
    }
}



