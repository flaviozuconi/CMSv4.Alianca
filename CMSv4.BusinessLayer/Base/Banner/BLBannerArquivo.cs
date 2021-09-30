using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLBannerArquivo : BLCRUD<MLBannerArquivo>
    {
        #region Gerar Url Thumb

        /// <summary>
        /// Gerar url para src de imagem utilizando thumb
        /// </summary>
        /// <param name="urlPortal"></param>
        /// <param name="CodigoBanner"></param>
        /// <param name="Largura"></param>
        /// <param name="Altura"></param>
        /// <param name="NomeImagem"></param>
        /// <returns></returns>
        public static string GerarUrlThumb(string urlPortal, decimal CodigoBanner, int Largura, int Altura, string NomeImagem)
        {
            return $"{Portal.Url()}/thumb/{urlPortal}/Banner/{CodigoBanner}/{Largura}/{Altura}/{NomeImagem}/?crop=true";
        }

        #endregion

        #region Incluir Arquivo

        public List<MLBannerArquivo> IncluirArquivos
        (
            decimal CodigoBanner,
            byte CodigoTipo,
            string[] files,
            string UrlIframe
        )
        {            
            var lista = new List<MLBannerArquivo>();

            try
            {
                var portal = PortalAtual.Obter;
                var diretorio = BLBanner.ObterDiretorio(CodigoBanner);
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var imagemBanner = SalvarBannerItem(CodigoBanner, CodigoTipo, Path.GetExtension(file), string.Empty);

                        //Renomear o arquivo fisíco para o código do registro
                        if (File.Exists(Path.Combine(pasta, file)))
                        {
                            File.Move(Path.Combine(pasta, file), Path.Combine(pasta, imagemBanner.Imagem));
                            BLReplicar.Arquivo(Path.Combine(pasta, imagemBanner.Imagem), diretorio);
                        }

                        lista.Add(imagemBanner);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(UrlIframe))
                {
                    lista.Add(SalvarBannerItem(CodigoBanner, CodigoTipo, string.Empty, UrlIframe));
                }

                //Limpar cache do banner para o idioma selecionado.
                BLCachePortal.LimparCache($"banner-{CodigoBanner}");

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private MLBannerArquivo SalvarBannerItem(decimal CodigoBanner, byte CodigoTipo, string ExtensaoArquivo, string UrlIframe)
        {
            var portal = PortalAtual.Obter;

            var imagemBanner = new MLBannerArquivo
            {
                CodigoBanner = CodigoBanner,
                CodigoPortal = portal.Codigo,
                Imagem = "novo",
                Ordem = 9999,
                Ativo = false, //o próprio usuário altera no admin o ativo para quando publicar
                NovaJanela = true,
                CodigoTipo = CodigoTipo,
                Url = UrlIframe
            };

            imagemBanner.Codigo = this.Salvar(imagemBanner, portal.ConnectionString);
            imagemBanner.Imagem = string.Concat(imagemBanner.Codigo, ExtensaoArquivo);

            //Trocar o nome da imagem para o código do novo registro
            imagemBanner.Codigo = this.SalvarParcial(new MLBannerArquivo { Codigo = imagemBanner.Codigo, Imagem = imagemBanner.Imagem }, portal.ConnectionString);

            return imagemBanner;
        }

        #endregion

        #region Mostrar Arquivos em Disco

        /// <summary>
        /// Lista com os arquivos que existem no disco (Foram deletados somente da base de dados) e podem
        /// ser restaurados.
        /// </summary>
        /// <param name="CodigoBanner"></param>
        /// <returns></returns>
        public List<MLBannerArquivo> MostrarArquivosDisco(decimal CodigoBanner)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var lista = this.Listar(new MLBannerArquivo { CodigoBanner = CodigoBanner }, "Ordem", "ASC", portal.ConnectionString);

                // Obter arquivos em disco (normalizando para apenas enviar os NOMES sem o diretório)
                var diretorio = BLBanner.ObterDiretorio(CodigoBanner);
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                var arquivos = new List<string>(Directory.GetFiles(pasta));
                var lstArquivosDisco = new List<MLBannerArquivo>();

                //Encontrar as imagens que estão na pasta, mas não estão cadastradas no banco.
                for (var i = 0; i < arquivos.Count; i++)
                {
                    var nome = Path.GetFileName(arquivos[i]);

                    if (lista.Find(o => o.Imagem == nome) == null)
                        lstArquivosDisco.Add(new MLBannerArquivo { CodigoBanner = CodigoBanner, Imagem = nome });

                }

                return lstArquivosDisco;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Obter

        public MLBannerArquivoView ObterArquivo(decimal Codigo, string connectionString = "")
        {
            var model = new MLBannerArquivoView() { Codigo = Codigo };

            try
            {
                var portal = PortalAtual.Obter;
                var modelBanner = new BLBanner().Obter(Codigo, connectionString);

                if (modelBanner == null || !modelBanner.Codigo.HasValue) return null;

                model.SugestaoResolucao = modelBanner.SugestaoResolucao;

                // Obter arquivos em disco (normalizando para apenas enviar os NOMES sem o diretório)
                var diretorio = (BLConfiguracao.Pastas.ModuloBanner(portal.Diretorio) + "/" + Codigo).Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                var dirInfo = new System.IO.DirectoryInfo(pasta);

                var lista = new List<MLBannerArquivo>();
                var listaArquivos = new CRUD.Select<MLBannerArquivo>()
                                        .Equals(a => a.CodigoBanner, Codigo)
                                        .OrderBy(a => a.Ordem)
                                        .ToList(portal.ConnectionString);

                //Adicionar na lista de arquivos em disco somente as imagens que não estejam nos itens cadastrados.
                if (dirInfo.Exists)
                    foreach (var item in dirInfo.GetFiles())
                        if (!lista.Exists(a => a.Imagem == item.Name) && listaArquivos.Find(a => a.Imagem == item.Name) == null)
                            lista.Add(new MLBannerArquivo { CodigoBanner = Codigo, Imagem = item.Name });

                model.DiretorioGaleria = diretorio;
                model.ListaArquivosEmDisco = lista;
                model.ListaArquivos = listaArquivos;
            }
            catch (Exception)
            {
                throw;
            }

            return model;
        }

        #endregion

        #region Ordenar

        /// <summary>
        /// Redefinir ordem de exibição do arquivo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public bool Ordenar(decimal id, List<decimal> ordem)
        {
            try
            {
                var portal = PortalAtual.Obter;

                // Validar Permissões
                var modelBanner = CRUD.Obter<MLBanner>(id, portal.ConnectionString);
                if (modelBanner == null || !modelBanner.Codigo.HasValue) return false;

                int _ordem = 0;
                foreach (var item in ordem)
                {
                    _ordem += 1;

                    this.SalvarParcial(new MLBannerArquivo
                    {
                        Codigo = item,
                        CodigoBanner = id,
                        Ordem = _ordem
                    }, portal.ConnectionString);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Remover

        /// <summary>
        /// Remover arquivo da base de dados mas manter arquivo old para restauração
        /// </summary>
        /// <param name="CodigoBannerArquivo"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public MLBannerArquivo Remover(decimal CodigoBannerArquivo, string connectionString = "")
        {
            var retorno = new MLBannerArquivo();
            var portal = PortalAtual.Obter;

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = portal.ConnectionString;

            var model = base.Obter(CodigoBannerArquivo, connectionString);
            if (model == null || !model.Codigo.HasValue) return null;

            base.Excluir(CodigoBannerArquivo, connectionString);

            BLCachePortal.LimparCache($"banner-{model.CodigoBanner}");

            var diretorio = (BLConfiguracao.Pastas.ModuloBanner(portal.Diretorio) + "/" + model.CodigoBanner).Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
            var nomeImagemBanner = string.Concat(Path.GetFileNameWithoutExtension(model.Imagem), ".old", Path.GetExtension(model.Imagem));

            if (File.Exists(Path.Combine(pasta, model.Imagem)))
            {
                File.Move(Path.Combine(pasta, model.Imagem), Path.Combine(pasta, nomeImagemBanner));
                BLReplicar.ExcluirArquivosReplicados(diretorio, Path.Combine(diretorio, model.Imagem));
                BLReplicar.Arquivo(Path.Combine(pasta, nomeImagemBanner), diretorio);
            }

            retorno.CodigoBanner = model.CodigoBanner;
            retorno.Imagem = nomeImagemBanner;

            return retorno;
        }

        /// <summary>
        /// Apagar o arquivo físico
        /// </summary>
        public void RemoverDoServidor(decimal id, string imagem)
        {
            var portal = PortalAtual.Obter;
            var diretorio = (BLConfiguracao.Pastas.ModuloBanner(portal.Diretorio) + "/" + id).Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

            try
            {
                File.Delete(Path.Combine(pasta, imagem));
                BLReplicar.ExcluirArquivosReplicados(diretorio, Path.Combine(pasta, imagem));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Salvar

        public override decimal Salvar(MLBannerArquivo model, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;

                if (string.IsNullOrWhiteSpace(connectionString))
                    connectionString = portal.ConnectionString;

                // Validar Permissões
                if (!model.CodigoBanner.HasValue) return 0;

                var conteudo = CRUD.Obter<MLBanner>(model.CodigoBanner.Value, connectionString);
                if (conteudo == null || !conteudo.Codigo.HasValue) return 0;

                model.CodigoPortal = conteudo.CodigoPortal;
                model.Codigo = base.Salvar(model, portal.ConnectionString);

                BLCachePortal.LimparCache($"banner-{model.CodigoBanner}");


                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region UploadGaleria

        /// <summary>
        /// Salvar em disco os arquivos selecionados no Dropzone
        /// </summary>
        /// <param name="id"></param>
        public void UploadGaleria(decimal CodigoBanner)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var diretorio = BLBanner.ObterDiretorio(CodigoBanner);

                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(pasta);
                }

                for (int i = 0; i < HttpContextFactory.Current.Request.Files.Count; i++)
                {
                    var item = HttpContextFactory.Current.Request.Files[i];

                    if (item != null && item.ContentLength > 0)
                        item.SaveAs(Path.Combine(pasta, Path.GetFileName(item.FileName)));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
