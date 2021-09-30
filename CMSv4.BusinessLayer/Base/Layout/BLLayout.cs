using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public static class BLLayout
    {
        #region Excluir

        public static void Excluir(List<string> ids)
        {
            var pasta = HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.LayoutsPortal(PortalAtual.Diretorio));
            var portal = PortalAtual.Obter;

            foreach (var item in ids)
            {
                var file = Path.Combine(pasta, item + ".cshtml");

                if (File.Exists(file))
                {
                    var backup = new MLLayout();
                    var model = CRUD.Listar(new MLLayout { CodigoPortal = portal.Codigo, Nome = item, Ativo = true }, portal.ConnectionString);
                    if (model != null && model.Count > 0)
                    { backup = model[0]; }

                    backup.Ativo = false;
                    backup.CodigoPortal = portal.Codigo;
                    backup.Nome = item;
                    backup.Conteudo = File.ReadAllText(file);

                    try
                    {
                        File.Delete(file);
                        BLReplicar.ExcluirArquivosReplicados(file);
                        CRUD.Salvar(backup, portal.ConnectionString);
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region Obter

        public static MLLayout Obter(string id)
        {
            var model = new MLLayout();
            var pasta = HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.LayoutsPortal(PortalAtual.Diretorio));

            if (!string.IsNullOrWhiteSpace(id))
            {
                var file = Path.Combine(pasta, id) + ".cshtml";
                model.Nome = id;

                if (Directory.Exists(pasta) && File.Exists(file))
                {
                    model.Conteudo = File.ReadAllText(file);
                }

                var imagemCaminhoFisico = $"{pasta}/{model.Nome}.jpg";

                if (System.IO.File.Exists(imagemCaminhoFisico))
                    model.Imagem = $"{pasta}/{model.Nome}.jpg";

            }

            return model;
        }

        #endregion

        #region ObterCaminhoRelativo

        /// <summary>
        /// Obter caminho
        /// </summary>
        public static string ObterCaminhoRelativo(MLPortal portal, string nome)
        {
            try
            {
                var pasta = BLConfiguracao.Pastas.LayoutsPortal(portal.Diretorio);

                nome = nome.EndsWith(".cshtml") ? nome : nome + ".cshtml";
                
                var nomeretorno =  (pasta + "/" + nome).Replace("//","/");
                
                return VerificarCriarArquivo(portal,nomeretorno);
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
        /// Listar Arquivos de Layout
        /// </summary>
        /// <param name="portal"></param>
        /// <returns></returns>
        public static string[] ListarArquivos(MLPortal portal)
        {
            try
            {
                var lista = new List<MLLayout>();
                var pasta = HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.LayoutsPortal(portal.Diretorio));

                if (Directory.Exists(pasta))
                {
                    return Directory.GetFiles(pasta, "*.cshtml");
                }

                return new string[]{};
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }

        #endregion

        #region Salvar

        public static decimal Salvar(MLLayout model, HttpPostedFileBase Imagem ,string NomeAnterior)
        {
            var pasta = HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.LayoutsPortal(PortalAtual.Diretorio));
            var portal = PortalAtual.Obter;

            model.CodigoPortal = portal.Codigo;
            model.Conteudo = model.Conteudo.Unescape();
            model.Ativo = true;
            model.DataCadastro = DateTime.Now;
            model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
            model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

            //Excluir arquivo anterior
            if (!string.IsNullOrWhiteSpace(NomeAnterior) && !model.Nome.Equals(NomeAnterior))
            {
                var anterior = Path.Combine(pasta, NomeAnterior + ".cshtml");
                if (File.Exists(anterior)) File.Delete(anterior);
            }

            //Criar novo arquivo
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var file = Path.Combine(pasta, model.Nome + ".cshtml");

            File.WriteAllText(file, model.Conteudo, System.Text.Encoding.UTF8);


            if (Imagem != null && Imagem.ContentLength > 0)
            {
                var arquivoImagem = $"{pasta}/{model.Nome}{Path.GetExtension(Imagem.FileName)}";
                Imagem.SaveAs(arquivoImagem);
                BLReplicar.Arquivo(arquivoImagem);
            }

            BLReplicar.Arquivo(file);

            return model.Codigo.GetValueOrDefault(0);
        }

        #endregion

        public static string VerificarCriarArquivo(MLPortal portal, string nome)
        {
            try
            {
                var fisica = HttpContext.Current.Server.MapPath(nome);

                //se nõa existe busca backup
                if (!File.Exists(fisica))
                {
                    var backup = CRUD.Listar(new MLLayout { CodigoPortal = portal.Codigo, Nome = Path.GetFileNameWithoutExtension(nome), Ativo = true }, portal.ConnectionString);
                    if (backup.Count > 0)
                        System.IO.File.WriteAllText(fisica, backup[0].Conteudo, System.Text.Encoding.UTF8);
                    else
                        return string.Empty;

                }                
                return nome;                

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static string ObterImagemDiretorioVirtual(string nome, string portal)
        {
            var diretorioVirtual = BLConfiguracao.Pastas.LayoutsPortal(portal);

            var arquivo = $"{diretorioVirtual}/{nome}.jpg";
            if (File.Exists(HttpContext.Current.Server.MapPath(arquivo)))
                return arquivo;

            return $"{diretorioVirtual}/_layout_404.jpg";
        }

    }
}
