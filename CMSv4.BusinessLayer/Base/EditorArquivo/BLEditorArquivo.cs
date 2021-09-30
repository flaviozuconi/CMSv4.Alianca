using CMSv4.Model;
using CMSv4.Model.Base.Conteudo;
using Framework.Utilities;
using System.Collections.Generic;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLEditorArquivo
    {
        #region BreadCrumb

        public static List<MLPastaArquivo> BreadCrumb(string pasta)
        {
            var lista = new List<MLPastaArquivo>();

            if (!string.IsNullOrEmpty(pasta))
            {
                string strCaminho = string.Empty;

                MLPastaArquivo objMLPastaArquivo = null;

                for (int i = 0; i < pasta.Split('|').Length; i++)
                {
                    if (!string.IsNullOrEmpty(pasta.Split('|')[i]))
                    {
                        strCaminho = strCaminho + "|" + pasta.Split('|')[i];

                        objMLPastaArquivo = new MLPastaArquivo();

                        if (i == pasta.Split('|').Length - 1)
                        {
                            objMLPastaArquivo.Nome = pasta.Split('|')[i];
                            objMLPastaArquivo.Atual = true;
                        }
                        else
                        {
                            objMLPastaArquivo.Nome = pasta.Split('|')[i];
                            objMLPastaArquivo.Caminho = strCaminho.TrimStart('|');
                        }

                        lista.Add(objMLPastaArquivo);
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Excluir

        public static void Excluir(List<string> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                if (!string.IsNullOrEmpty(ids[i]))
                {
                    var nomeArquivo = BLEncriptacao.DesencriptarQueryString(ids[i]);
                    BLGaleria.Excluir(nomeArquivo);
                    BLReplicar.ExcluirArquivosReplicados(Path.GetDirectoryName(nomeArquivo), nomeArquivo);
                }
            }
        }

        #endregion

        #region Obter

        public static MLConteudoViewModel Obter(string id)
        {
            var model = new MLConteudoViewModel();
            
            model.ArquivoFisico = System.Web.HttpContext.Current.Server.MapPath(BLEncriptacao.DesencriptarQueryString(id.TrimStart('/')));
            model.FileInfo = new FileInfo(model.ArquivoFisico);

            if (!model.FileInfo.Exists)
                return model;

            var tipoArquivo = ArquivosEditaveis.Tipos.Find(o => o.Extensao.Equals(model.FileInfo.Extension));

            if (!string.IsNullOrWhiteSpace(tipoArquivo.AceEditorMode))
            {
                model.Nome = Path.GetFileName(model.ArquivoFisico);
                model.Diretorio = Path.GetDirectoryName(model.ArquivoFisico);
                model.AceEditorMode = tipoArquivo.AceEditorMode;
                model.Editavel = true;

                using (var file = model.FileInfo.OpenText())
                    model.Conteudo = file.ReadToEnd();
            }
            
            return model;
        }

        #endregion

        #region Salvar

        public static void Salvar(string NomeAnterior, string Diretorio, string Nome, string Conteudo)
        {
            string nomeAnterior = string.Concat(Diretorio, "/", NomeAnterior);
            string nomeAtual = string.Concat(Diretorio, "/", Nome);

            Conteudo = Conteudo.Unescape();

            var fileInfo = new FileInfo(nomeAnterior);

            if (fileInfo.Exists)
            {
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                    fileInfo.Refresh();
                }

                if (!NomeAnterior.Equals(Nome))
                {
                    fileInfo.MoveTo(nomeAtual);
                }

                using (var arquivo = new StreamWriter(nomeAtual, false))
                {
                    arquivo.Write(Conteudo);
                    arquivo.Flush();
                    arquivo.Close();
                }
            }
        }

        #endregion

        #region Upload

        public static void Upload(string diretorio)
        {
            var diretorioVirtualRaiz = string.Format("~/portal/{0}/", PortalAtual.Diretorio);
            var diretorioRaiz = System.Web.HttpContext.Current.Server.MapPath(string.Concat(diretorioVirtualRaiz, diretorio.Replace("|", "/")));

            for (int i = 0; i < HttpContextFactory.Current.Request.Files.Count; i++)
            {
                HttpContextFactory.Current.Request.Files[i].SaveAs(string.Concat(diretorioRaiz, "/", HttpContextFactory.Current.Request.Files[i].FileName));
                BLReplicar.Arquivo(string.Concat(diretorioRaiz, "/", HttpContextFactory.Current.Request.Files[i].FileName), string.Concat(diretorioVirtualRaiz, diretorio.Replace("|", "/")));
            }
        }

        #endregion
    }
}
