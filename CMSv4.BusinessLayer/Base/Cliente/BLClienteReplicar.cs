using Framework.Utilities;
using System.IO;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLClienteReplicar
    {
        public static bool ReplicarArquivo(HttpPostedFile file, decimal? codigo)
        {
            var diretorio = string.Concat(BLConfiguracao.Pastas.ClientesPortal("[[PORTAL]]"));

            return ReplicarArquivo(file, codigo, diretorio);
        }

        private static bool ReplicarArquivo(HttpPostedFile file, decimal? codigo, string diretorio)
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

                    file.SaveAs(HttpContext.Current.Server.MapPath(nomeImagem));
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

        public static bool ExcluirArquivosReplicados(string nomeImagem)
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
    }
}
