using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLTemplate
    {
        #region CarregarArquivo

        /// <summary>
        /// Carregar Arquivo
        /// </summary>
        public static string CarregarArquivo(MLPortal portal, string nome)
        {
            try
            {

                var pasta = HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.TemplatesPortal(portal.Diretorio));

                if (Directory.Exists(pasta))
                {
                    nome = nome.EndsWith(".cshtml") ? nome : nome + ".cshtml";

                    if (File.Exists(Path.Combine(pasta, nome)))
                    {
                        return File.ReadAllText(Path.Combine(pasta, nome));
                    }
                    else
                    {
                        var backup = CRUD.Listar(new MLTemplate { CodigoPortal = portal.Codigo, Nome = Path.GetFileNameWithoutExtension(nome), Ativo = true }, portal.ConnectionString);
                        if (backup.Count > 0)
                            return backup[0].Conteudo;
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }

        #endregion

        #region ListarArquivos

        /// <summary>
        /// Listar Arquivos de Template
        /// </summary>
        /// <param name="portal"></param>
        /// <returns></returns>
        public static List<MLTemplate> ListarArquivos(MLPortal portal)
        {
            try
            {

                var lista = CRUD.Listar(new MLTemplate(), portal.ConnectionString);

                var pasta = BLConfiguracao.Pastas.TemplatesPortal(portal.Diretorio);


                foreach (var l in lista)
                {

                    l.Imagem = ObterImagemDiretorioVirtual(l.Nome, portal.Diretorio);

                }


                return lista;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }

        #endregion

        #region Diretorios
        public static string ObterImagemDiretorioVirtual(string nome, string portal)
        {
            var diretorioVirtual = BLConfiguracao.Pastas.TemplatesPortal(portal);

            var arquivo = $"{diretorioVirtual}/{nome}.jpg";
            if (File.Exists(HttpContext.Current.Server.MapPath(arquivo)))
                return arquivo;

            return $"{diretorioVirtual}/_template_404.jpg";

        }
        #endregion
    }
}
