using Framework.Utilities;
using System.IO;
using System.IO.Compression;

namespace CMSv4.BusinessLayer
{
    public class BLEditorArquivoZip
    {
        #region Compactar

        public static void Compactar(string[] arquivos, string diretorioAtual, string nome)
        {
            var diretorioVirtualRaiz = string.Format("~/portal/{0}/", PortalAtual.Diretorio);
            var diretorioFisicoRaiz = HttpContextFactory.Current.Server.MapPath(diretorioVirtualRaiz + diretorioAtual.Replace("|", "/"));

            using (FileStream fs = new FileStream(diretorioFisicoRaiz + "/" + nome + ".zip", FileMode.Create))
            {
                using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    foreach (var item in arquivos)
                    {
                        var file = System.Web.HttpContext.Current.Server.MapPath(BLEncriptacao.DesencriptarQueryString(item.TrimStart('/')));
                        var info = new FileInfo(file);

                        arch.CreateEntryFromFile(file, info.Name);
                    }
                }
            }

            BLReplicar.Arquivo(diretorioFisicoRaiz + "/" + nome + ".zip", diretorioVirtualRaiz + diretorioAtual.Replace("|", "/"));
        }

        public static void CompactarDiretorio(string diretorioAtual, string nome)
        {
            var diretorioVirtualRaiz = string.Format("~/portal/{0}/", PortalAtual.Diretorio);
            var diretorioFisicoRaiz = HttpContextFactory.Current.Server.MapPath(diretorioVirtualRaiz + diretorioAtual.Replace("|", "/"));
            var diretorioFisicoPai = HttpContextFactory.Current.Server.MapPath("~/portal/");

            ZipFile.CreateFromDirectory(diretorioFisicoRaiz, diretorioFisicoPai + "/" + nome + ".zip",CompressionLevel.Fastest,true);


            //BLReplicar.Arquivo(diretorioFisicoRaiz + "/" + nome + ".zip", diretorioVirtualRaiz + diretorioAtual.Replace("|", "/"));
        }
        #endregion

        #region Descompactar

        public static bool Descompactar(string arquivo, string diretorioAtual)
        {
            var arquivoFisico = HttpContextFactory.Current.Server.MapPath(BLEncriptacao.DesencriptarQueryString(arquivo.TrimStart('/')));
            var fileInfo = new FileInfo(arquivoFisico);

            if (fileInfo.Exists)
            {
                string appPath = HttpContextFactory.Current.Server.MapPath("~");

                ZipFile.ExtractToDirectory(arquivoFisico, fileInfo.DirectoryName);
                BLReplicar.DescompactarArquivo(arquivoFisico, string.Format("~{0}", fileInfo.DirectoryName.Replace(appPath, "").Replace("\\", "/")));

                return true;
            }

            return false;
        }

        #endregion

        #region Obter Url Publica

        public static string ObterUrlPublica(string arquivo, string diretorio)
        {
            var arquivoVirtual = BLEncriptacao.DesencriptarQueryString(arquivo.TrimStart('/'));
            var arquivoFisico = System.Web.HttpContext.Current.Server.MapPath(arquivoVirtual);
            var fileInfo = new FileInfo(arquivoFisico);
            var tradutor = new BLTraducao();

            if (fileInfo.Exists)
                return HttpContextFactory.Current.Request.Url.Scheme + "://" + HttpContextFactory.Current.Request.Url.Authority + arquivoVirtual.Replace("~", "").Replace("//", "/");

            return tradutor.ObterAdm("Não foi possível obter a url");
        }

        #endregion
    }
}
