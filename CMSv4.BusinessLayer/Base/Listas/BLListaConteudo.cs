using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Transactions;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLListaConteudo : BLCRUD<MLListaConteudo>
    {
        public readonly BLUsuario _usuario;

        public BLListaConteudo()
        {
            _usuario = new BLUsuario();
        }

        #region ListarAdmin

        public List<MLListaConteudoGrid> ListarAdmin(decimal codigolista, NameObjectCollectionBase collectionQueryString)
        {

            NameValueCollection queryString = (NameValueCollection)collectionQueryString;

            decimal? codigoIdioma = null;
            DateTime? datainicio = null;
            DateTime? datafinal = null;

            var filter = new DataTableFilter.DataTableFilter(queryString).Get();

            if (!string.IsNullOrEmpty(queryString["codigoIdioma"]))
            {
                codigoIdioma = Convert.ToDecimal(queryString["codigoIdioma"]);
            }


            using (var command = Database.NewCommand("USP_CMS_L_LISTA_CONTEUDO_ADMIN", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, 0, codigolista);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, 0, codigoIdioma);
                command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, -1, filter.SearchedValue);
                command.NewCriteriaParameter("@LIT_D_DATA", SqlDbType.DateTime, datainicio);
                command.NewCriteriaParameter("@LIT_D_DATA_FINAL", SqlDbType.DateTime, datafinal);
                command.NewCriteriaParameter("@ORDERBY", SqlDbType.VarChar, 100, filter.OrderBy);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, (filter.Sort == "asc"));
                command.NewCriteriaParameter("@START", SqlDbType.Int, filter.Start);
                command.NewCriteriaParameter("@LENGTH", SqlDbType.Int, filter.Length);

                // Execucao
                return Database.ExecuteReader<MLListaConteudoGrid>(command);
            }

        }
        #endregion

        #region SalvarAdmin

        public MLListaConteudo SalvarAdmin(MLListaConteudo model,
            List<string> GaleriaImagem, List<string> GaleriaTitulo, List<string> GaleriaTexto, List<string> GaleriaFonte,
            List<string> GaleriaVideo, List<string> VideoTitulo, List<string> VideoTexto, List<string> VideoFonte, List<string> VideoGuid,
            List<string> GaleriaAudio, List<string> AudioTitulo, List<string> AudioTexto, List<string> AudioFonte, List<string> AudioGuid,
            bool? RemoverCapa, bool? Autorizar, bool? Publicar, string CodigoUsuarioSolicitado, bool? IsRevisar, string ArquivoHf,
            HttpPostedFileBase Arquivo, HttpPostedFileBase Max1, HttpPostedFileBase ImagemCapa,
            NameObjectCollectionBase collectionQueryString, ref bool publicarAgora)
        {

            NameValueCollection queryString = (NameValueCollection)collectionQueryString;

            decimal codigoLista = 0;
            var portal = PortalAtual.Obter;

            MLUsuario usuarioAviso = null;
            var nomeArquivo = string.Empty;
            var diretorio = string.Empty;

            model.Ativo = model.Ativo.GetValueOrDefault();
            model.Destaque = model.Destaque.GetValueOrDefault();

            if (IsRevisar.GetValueOrDefault())
            {
                if (!String.IsNullOrEmpty(CodigoUsuarioSolicitado))
                    usuarioAviso = CRUD.Obter<MLUsuario>(Convert.ToDecimal(CodigoUsuarioSolicitado));
                else
                {
                    usuarioAviso = null;
                    CodigoUsuarioSolicitado = null;
                    model.CodigoUsuarioSolicitado = null;
                }
            }
            else
            {
                usuarioAviso = null;
                CodigoUsuarioSolicitado = null;
                model.CodigoUsuarioSolicitado = null;
            }

            using (var scope = new TransactionScope(portal.ConnectionString))
            {
                model.Editado = true;

                if (string.IsNullOrWhiteSpace(model.Titulo)) model.Titulo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (Publicar.HasValue && Publicar.Value)
                {
                    model.DataPublicacao = null;
                    publicarAgora = true;
                }

                if (Autorizar.HasValue)
                {
                    if (Autorizar.Value)
                    {
                        var config = CRUD.Obter<MLListaConfig>(model.CodigoLista.Value, portal.ConnectionString);
                        var usuario = BLUsuario.ObterLogado();
                        codigoLista = Convert.ToDecimal(config.Codigo);

                        if (config.CodigoGrupoAprovador.HasValue && usuario.Grupos.Find(o => o.CodigoGrupo == config.CodigoGrupoAprovador.Value) != null)
                        {
                            model.DataAprovacao = DateTime.Now;
                            model.CodigoUsuarioAprovacao = usuario.Codigo;

                            if (!model.DataPublicacao.HasValue || model.DataPublicacao.Value < DateTime.Now) publicarAgora = true;
                        }
                    }

                    if (!Autorizar.Value && usuarioAviso != null)
                    {
                        var config = CRUD.Obter<MLListaConfig>(model.CodigoLista.Value, portal.ConnectionString);
                        //criar email modelo de aviso para Aprovação da Avaliação!
                        BLEmail.Enviar(config.Descricao, usuarioAviso.Email, "<p>Olá,</p><p>A Aprovação de um " + config.Descricao + " foi solicitado a você!</p>");
                    }
                }
                else
                {
                    if (!model.Codigo.HasValue)
                    {
                        //caso não tenha grupo autorizador, autoriza direto
                        var usuario = BLUsuario.ObterLogado();

                        model.DataAprovacao = DateTime.Now;
                        model.CodigoUsuarioAprovacao = usuario.Codigo;
                    }
                }

                var codigo = CRUD.Salvar(model, portal.ConnectionString);
                model.Codigo = codigo;
                diretorio = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");

                if (!Directory.Exists(HttpContextFactory.Current.Server.MapPath(diretorio)))
                    Directory.CreateDirectory(HttpContextFactory.Current.Server.MapPath(diretorio));

                #region Remover Imagem

                if (RemoverCapa.GetValueOrDefault() && !string.IsNullOrEmpty(model.Imagem))
                {
                    diretorio = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");

                    var nomeImagem = string.Concat(diretorio, "capa", model.Imagem);

                    FileInfo fi = new FileInfo(HttpContextFactory.Current.Server.MapPath(nomeImagem));
                    if (fi.Exists)
                    {
                        try
                        {
                            fi.Delete();
                            BLReplicar.ExcluirArquivosReplicados(HttpContextFactory.Current.Server.MapPath(nomeImagem));
                        }
                        catch { }
                    }

                    model.Imagem = String.Empty;
                }
                #endregion

                if (Max1 != null && Max1.ContentLength > 0)
                {
                    var dir = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");
                    model.Max1 = dir + "thumb" + System.IO.Path.GetExtension(Max1.FileName);

                    Max1.SaveAs(HttpContextFactory.Current.Server.MapPath(model.Max1));
                    BLReplicar.Arquivo(HttpContextFactory.Current.Server.MapPath(model.Max1));
                }

                if (Arquivo != null && Arquivo.ContentLength > 0)
                {
                    diretorio = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");

                    nomeArquivo = string.Concat(diretorio, "arquivo", Path.GetExtension(Arquivo.FileName));
                }
                else
                {
                    nomeArquivo = ArquivoHf;
                }

                #region Categorias

                CRUD.Excluir(new MLListaConteudoCategoria { CodigoConteudo = codigo }, portal.ConnectionString);

                if (queryString["Categorias"] != null)
                {
                    foreach (var item in queryString["Categorias"].Split(','))
                    {
                        decimal codigoItem;
                        if (decimal.TryParse(item, out codigoItem))
                        {
                            CRUD.Salvar(new MLListaConteudoCategoria { CodigoConteudo = codigo, CodigoCategoria = Convert.ToDecimal(item) }, portal.ConnectionString);
                        }
                    }
                }
                #endregion

                #region Extra2
                //Necessario para MultiSelect

                if (queryString["Extra2"] != null)
                {
                    model.Extra2 = queryString["Extra2"].Replace("multiselect-all", "").TrimStart(',');
                }
                #endregion

                #region Salvar Arquivo
                if (Arquivo != null && Arquivo.ContentLength > 0)
                {
                    diretorio = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");

                    var arquivo = string.Concat(diretorio, "arquivo", Path.GetExtension(Arquivo.FileName));
                    var di = new DirectoryInfo(HttpContextFactory.Current.Server.MapPath(diretorio));

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    Arquivo.SaveAs(HttpContextFactory.Current.Server.MapPath(arquivo));
                    BLReplicar.Arquivo(HttpContextFactory.Current.Server.MapPath(arquivo));
                }
                #endregion

                #region Salvar Imagem

                if (ImagemCapa != null)
                {
                    model.Imagem = Path.GetExtension(ImagemCapa.FileName);//".png";

                    diretorio = string.Concat(BLConfiguracao.Pastas.ModuloListas(portal.Diretorio), "/", model.Codigo, "/");

                    var nomeImagem = string.Concat(diretorio, "capa", model.Imagem);
                    var di = new DirectoryInfo(HttpContextFactory.Current.Server.MapPath(diretorio));
                    var di_thumb = new DirectoryInfo(HttpContextFactory.Current.Server.MapPath(string.Concat(diretorio, "/_thumb")));
                    var fi = new FileInfo(HttpContextFactory.Current.Server.MapPath(nomeImagem));

                    if (!di.Exists)
                    {
                        di.Create();
                        BLReplicar.Diretorio(diretorio);
                    }

                    if (fi.Exists)
                    {
                        try
                        {
                            fi.Delete();
                            BLReplicar.Arquivo(HttpContextFactory.Current.Server.MapPath(nomeImagem));
                        }
                        catch { }
                    }

                    //excluir thumbs de imagens anteriores
                    if (di_thumb.Exists)
                    {
                        try
                        {
                            di_thumb.Delete(true);
                            BLReplicar.ExcluirDiretoriosReplicados(string.Concat(diretorio, "/_thumb"));
                        }
                        catch { }
                    }

                    ImagemCapa.SaveAs(HttpContextFactory.Current.Server.MapPath(nomeImagem));
                    BLReplicar.Arquivo(HttpContextFactory.Current.Server.MapPath(nomeImagem));

                   
                }

                #endregion

                #region Galeria
                CRUD.Excluir(new MLListaConteudoImagem { CodigoConteudo = model.Codigo }, portal.ConnectionString);

                if (GaleriaImagem != null)
                {
                    for (var i = 0; i < GaleriaImagem.Count; i++)
                    {
                        var listaImagem = new MLListaConteudoImagem();

                        listaImagem.CodigoConteudo = model.Codigo;
                        listaImagem.Imagem = GaleriaImagem[i];
                        listaImagem.Ordem = i;

                        if (i < GaleriaTitulo.Count) listaImagem.Titulo = GaleriaTitulo[i];
                        if (i < GaleriaTexto.Count) listaImagem.Texto = GaleriaTexto[i];
                        if (i < GaleriaFonte.Count) listaImagem.Fonte = GaleriaFonte[i];

                        CRUD.Salvar(listaImagem, portal.ConnectionString);
                    }
                }
                #endregion

                #region Vídeos
                CRUD.Excluir(new MLListaConteudoVideo { CodigoConteudo = model.Codigo }, portal.ConnectionString);

                if (GaleriaVideo != null)
                {
                    for (var i = 0; i < GaleriaVideo.Count; i++)
                    {
                        var video = new MLListaConteudoVideo();

                        video.CodigoConteudo = model.Codigo;
                        video.Video = GaleriaVideo[i];
                        video.Ordem = i;
                        if (i < VideoTitulo.Count) video.Titulo = VideoTitulo[i];
                        if (i < VideoTexto.Count) video.Texto = VideoTexto[i];
                        if (i < VideoFonte.Count) video.Fonte = VideoFonte[i];
                        if (i < VideoGuid.Count) video.Guid = new Guid(VideoGuid[i]);

                        CRUD.Salvar(video, portal.ConnectionString);
                    }
                }
                #endregion

                #region Audio
                CRUD.Excluir(new MLListaConteudoAudio { CodigoConteudo = model.Codigo }, portal.ConnectionString);

                if (GaleriaAudio != null)
                {
                    for (var i = 0; i < GaleriaAudio.Count; i++)
                    {
                        var Audio = new MLListaConteudoAudio();

                        Audio.CodigoConteudo = model.Codigo;
                        Audio.Audio = GaleriaAudio[i];
                        Audio.Ordem = i;

                        if (i < AudioTitulo.Count) Audio.Titulo = AudioTitulo[i];
                        if (i < AudioTexto.Count) Audio.Texto = AudioTexto[i];
                        if (i < AudioFonte.Count) Audio.Fonte = AudioFonte[i];
                        if (i < AudioGuid.Count) Audio.Guid = new Guid(AudioGuid[i]);

                        CRUD.Salvar(Audio, portal.ConnectionString);
                    }
                }
                #endregion

                #region Seo
                CRUD.Excluir<MLListaConteudoSEO>(codigo, portal.ConnectionString);

                if (model.Seo != null)
                {
                    model.Seo.Codigo = codigo;
                    CRUD.Salvar(model.Seo, portal.ConnectionString);
                }
                #endregion

                CRUD.SalvarParcial(model, portal.ConnectionString);

                scope.Complete();
            }

            return model;

        }

        #endregion

        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            int qtdeExcluiros = 0;
            var blListaConteudoPublicado = new BLListaConteudoPublicado();
            var blListaConfig = new BLListaConfig();

            foreach (var item in ids)
            {
                var id = Convert.ToDecimal(item);
                var model = base.Obter(id, connectionString);

                if (blListaConfig.ValidarPermissaoLista(model.CodigoLista.Value))
                {
                    qtdeExcluiros += base.Excluir(id, connectionString);
                    blListaConteudoPublicado.Excluir(id, connectionString);
                }
            }

            return qtdeExcluiros;
        }


        #endregion

        #region Vincular

        public void Vincular(MLListaConteudo model, MLPortal portal)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                base.SalvarParcial(new MLListaConteudo
                {
                    Codigo = model.Codigo,
                    CodigoBase = model.CodigoBase
                }, portal.ConnectionString);

                new BLListaConteudoPublicado().SalvarParcial(new MLListaConteudoPublicado
                {
                    Codigo = model.Codigo,
                    CodigoBase = model.CodigoBase
                }, portal.ConnectionString);

                scope.Complete();
            }

            #endregion

        }
    }

    public class BLListaConteudoCategoria : BLCRUD<MLListaConteudoCategoria> { }

    #region BLListaConteudoImagem

    public class BLListaConteudoImagem : BLCRUD<MLListaConteudoImagem>
    {
        #region UploadGaleria

        public bool UploadGaleria(Guid guid, HttpFileCollectionBase files, MLPortal portal)
        {
            var id = guid.ToString().Replace("-", "");

            var conteudo = CRUD.Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                var lista = CRUD.Obter(new MLListaConfig { CodigoPortal = portal.Codigo, Codigo = conteudo.CodigoLista }, portal.ConnectionString);
                if (lista == null || !lista.Codigo.HasValue) return false;

                if (!new BLListaConfig().ValidarPermissaoLista(lista.Codigo.Value)) return false;
            }

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + id + "/galeria").Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                var caminhoArquivo = Path.Combine(pasta, file.FileName);

                int copia = 1;
                while (System.IO.File.Exists(caminhoArquivo))
                {
                    caminhoArquivo = Path.Combine(pasta, Path.GetFileNameWithoutExtension(file.FileName) + "_" + copia + Path.GetExtension(file.FileName));
                    copia += 1;
                }

                file.SaveAs(caminhoArquivo);
                BLReplicar.Arquivo(caminhoArquivo);
            }

            return true;
        }

        #endregion

        #region IncluirArquivoGaleria

        public List<MLListaConteudoImagem> IncluirArquivoGaleria(Guid guid, string[] files, MLPortal portal)
        {
            decimal? id = null;

            var model = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);

            var lista = new List<MLListaConteudoImagem>();

            foreach (var file in files)
            {
                var nome = Path.GetFileName(file);
                MLListaConteudoImagem imagem = null;

                if (model != null && model.Codigo.HasValue && model.CodigoLista.HasValue)
                {
                    if (!new BLListaConfig().ValidarPermissaoLista(model.CodigoLista.Value)) return null;

                    id = model.Codigo;
                    imagem = base.Obter(new MLListaConteudoImagem { CodigoConteudo = model.Codigo, Imagem = nome }, portal.ConnectionString);
                }

                if (imagem != null && imagem.Codigo.HasValue)
                {
                    imagem.Imagem = nome;
                }
                else
                {
                    imagem = new MLListaConteudoImagem
                    {
                        CodigoConteudo = id,
                        GuidConteudo = guid,
                        Imagem = nome,
                        Ordem = 9999
                    };
                }

                lista.Add(imagem);
            }

            return lista;
        }

        #endregion

        #region RemoverArquivoGaleria

        public bool RemoverArquivoGaleria(Guid guid, string file, MLPortal portal)
        {
            var conteudo = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var model = CRUD.Obter(new MLListaConteudoImagem { CodigoConteudo = conteudo.Codigo, Imagem = file }, portal.ConnectionString);
                if (model != null && model.Codigo.HasValue && model.CodigoConteudo.HasValue)
                {
                    CRUD.Excluir(new MLListaConteudoImagem
                    {
                        Codigo = model.Codigo,
                        CodigoConteudo = model.CodigoConteudo
                    }, portal.ConnectionString);
                }
            }

            return true;
        }

        #endregion

        #region ExcluirArquivoGaleria

        public bool ExcluirArquivoGaleria(Guid guid, string file, MLPortal portal)
        {
            var conteudo = CRUD.Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/galeria").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                if (!Directory.Exists(pasta)) return false;
                if (!System.IO.File.Exists(Path.Combine(pasta, file))) return false;

                System.IO.File.Delete(Path.Combine(pasta, file));
            }

            return true;
        }

        #endregion

        #region ListarConteudoImagemMaisArquivosEmDisco

        public List<MLListaConteudoImagem> ListarConteudoImagemMaisArquivosEmDisco(MLListaConteudo model)
        {
            var portal = PortalAtual.Obter;
            var lista = base.Listar(new MLListaConteudoImagem { CodigoConteudo = model.Codigo }, "Ordem", "ASC", portal.ConnectionString);

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/galeria").Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            var arquivos = new List<string>(Directory.GetFiles(pasta));
            for (var i = 0; i < arquivos.Count; i++)
            {
                var nome = Path.GetFileName(arquivos[i]);
                if (lista.Find(o => o.Imagem == nome) == null)
                {
                    lista.Add(new MLListaConteudoImagem { CodigoConteudo = model.Codigo, Imagem = nome });
                }
            }

            return lista;
        }

        #endregion
    }

    #endregion

    #region BLListaConteudoVideo

    public class BLListaConteudoVideo : BLCRUD<MLListaConteudoVideo>
    {
        #region UploadVideo
        public bool UploadVideo(Guid guid, HttpPostedFileBase Arquivo, ref string caminho, MLPortal portal)
        {
            var id = guid.ToString().Replace("-", "");

            var conteudo = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                var lista = CRUD.Obter(new MLListaConfig { CodigoPortal = portal.Codigo, Codigo = conteudo.CodigoLista }, portal.ConnectionString);
                if (lista == null || !lista.Codigo.HasValue) return false;

                if (!new BLListaConfig().ValidarPermissaoLista(lista.Codigo.Value)) return false;
            }

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + id + "/video").Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            caminho = Path.Combine(pasta, Arquivo.FileName);

            int copia = 1;
            while (System.IO.File.Exists(caminho))
            {
                caminho = Path.Combine(pasta, Path.GetFileNameWithoutExtension(Arquivo.FileName) + "_" + copia + Path.GetExtension(Arquivo.FileName));
                copia += 1;
            }

            Arquivo.SaveAs(caminho);
            BLReplicar.Arquivo(caminho);

            return true;
        }
        #endregion

        #region IncluirVideoUrl
        public MLListaConteudoVideo IncluirVideoUrl(Guid guid, MLPortal portal)
        {
            decimal? id = null;

            var model = CRUD.Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            MLListaConteudoVideo video = null;

            if (model != null && model.Codigo.HasValue && model.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(model.CodigoLista.Value)) return null;

                id = model.Codigo;
            }
            video = new MLListaConteudoVideo
            {
                CodigoConteudo = id,
                GuidConteudo = guid,
                Guid = Guid.NewGuid(),
                Ordem = 9999,
                Video = string.Empty
            };

            return video;
        }
        #endregion

        #region IncluirArquivoVideo
        public MLListaConteudoVideo IncluirArquivoVideo(Guid guid, string file, MLPortal portal)
        {
            decimal? id = null;
            var nome = Path.GetFileName(file);

            var model = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            MLListaConteudoVideo video = null;

            if (model != null && model.Codigo.HasValue && model.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(model.CodigoLista.Value)) return null;

                id = model.Codigo;
                video = base.Obter(new MLListaConteudoVideo { CodigoConteudo = model.Codigo, Video = nome }, portal.ConnectionString);
            }

            if (video != null && video.Codigo.HasValue)
            {
                video.Video = nome;
            }
            else
            {
                video = new MLListaConteudoVideo
                {
                    CodigoConteudo = id,
                    GuidConteudo = guid,
                    Guid = Guid.NewGuid(),
                    Video = nome,
                    Ordem = 9999
                };

                base.Salvar(video, portal.ConnectionString);
            }

            return video;
        }
        #endregion

        #region RemoverArquivoVideo
        public bool RemoverArquivoVideo(Guid guid, string file, MLPortal obter)
        {
            var portal = PortalAtual.Obter;

            var conteudo = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var model = base.Obter(new MLListaConteudoVideo { CodigoConteudo = conteudo.Codigo, Video = file }, portal.ConnectionString);
                if (model != null && model.Codigo.HasValue && model.CodigoConteudo.HasValue)
                {
                    base.Excluir(new MLListaConteudoVideo
                    {
                        Codigo = model.Codigo,
                        CodigoConteudo = model.CodigoConteudo
                    }, portal.ConnectionString);
                }
            }

            return true;
        }
        #endregion

        #region ExcluirArquivoVideo
        public bool ExcluirArquivoVideo(Guid guid, string file, MLPortal portal)
        {
            var conteudo = CRUD.Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/video").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                if (!Directory.Exists(pasta)) return false;
                if (!File.Exists(Path.Combine(pasta, file))) return false;

                File.Delete(Path.Combine(pasta, file));
            }
            return true;
        }
        #endregion

        #region ListarConteudoVideoMaisArquivosEmDisco
        public List<MLListaConteudoVideo> ListarConteudoVideoMaisArquivosEmDisco(MLListaConteudo model)
        {

            var portal = PortalAtual.Obter;
            var listaVideo = new BLListaConteudoVideo().Listar(new MLListaConteudoVideo { CodigoConteudo = model.Codigo }, "Ordem", "ASC", portal.ConnectionString);

            var diretorioVideo = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/video").Replace("//", "/");

            var pastaVideo = HttpContextFactory.Current.Server.MapPath(diretorioVideo);
            if (!Directory.Exists(pastaVideo)) Directory.CreateDirectory(pastaVideo);

            var arquivosVideo = new List<string>(Directory.GetFiles(pastaVideo));
            for (var i = 0; i < arquivosVideo.Count; i++)
            {
                var nome = Path.GetFileName(arquivosVideo[i]);
                if (listaVideo.Find(o => o.Video == nome) == null)
                {
                    listaVideo.Add(new MLListaConteudoVideo { CodigoConteudo = model.Codigo, Video = nome });
                }
            }

            return listaVideo;
        }
        #endregion

        #region ListarVideo
        public List<MLListaConteudoVideo> ListarVideo(decimal? id, string guid)
        {
            MLListaConteudoVideo criterios = new MLListaConteudoVideo();

            if (id.HasValue)
                criterios.CodigoConteudo = id;
            else
                criterios.GuidConteudo = new Guid(guid);

            return new BLListaConteudoVideo().Listar(criterios, "Ordem", "ASC", PortalAtual.ConnectionString);
        }
        #endregion
    }

    #endregion

    #region BLListaConteudoAudio

    public class BLListaConteudoAudio : BLCRUD<MLListaConteudoAudio>
    {
        #region UploadAudio
        public bool UploadAudio(Guid guid, HttpPostedFileBase file, ref string caminho, MLPortal portal)
        {
            var id = guid.ToString().Replace("-", "");

            var conteudo = CRUD.Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                var lista = CRUD.Obter(new MLListaConfig { CodigoPortal = portal.Codigo, Codigo = conteudo.CodigoLista }, portal.ConnectionString);
                if (lista == null || !lista.Codigo.HasValue) return false;

                if (!new BLListaConfig().ValidarPermissaoLista(lista.Codigo.Value)) return false;
            }

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + id + "/Audio").Replace("//", "/");
            var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            var caminhoArquivo = Path.Combine(pasta, file.FileName);

            int copia = 1;
            while (System.IO.File.Exists(caminhoArquivo))
            {
                caminhoArquivo = Path.Combine(pasta, Path.GetFileNameWithoutExtension(file.FileName) + "_" + copia + Path.GetExtension(file.FileName));
                copia += 1;
            }

            file.SaveAs(caminhoArquivo);
            BLReplicar.Arquivo(caminhoArquivo);

            return true;
        }
        #endregion

        #region IncluirArquivoAudio

        public MLListaConteudoAudio IncluirArquivoAudio(Guid guid, string file, MLPortal portal)
        {

            decimal? id = null;
            var nome = Path.GetFileName(file);

            var model = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            MLListaConteudoAudio Audio = null;

            if (model != null && model.Codigo.HasValue && model.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(model.CodigoLista.Value)) return null;

                id = model.Codigo;
                Audio = base.Obter(new MLListaConteudoAudio { CodigoConteudo = model.Codigo, Audio = nome }, portal.ConnectionString);
            }

            if (Audio != null && Audio.Codigo.HasValue)
            {
                Audio.Audio = nome;
            }
            else
            {
                Audio = new MLListaConteudoAudio
                {
                    CodigoConteudo = id,
                    GuidConteudo = guid,
                    Guid = Guid.NewGuid(),
                    Audio = nome,
                    Ordem = 9999
                };
            }
            return Audio;
        }
        #endregion

        #region RemoverArquivoAudio
        public bool RemoverArquivoAudio(Guid guid, string file, MLPortal portal)
        {
            var conteudo = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var model = CRUD.Obter(new MLListaConteudoAudio { CodigoConteudo = conteudo.Codigo, Audio = file }, portal.ConnectionString);
                if (model != null && model.Codigo.HasValue && model.CodigoConteudo.HasValue)
                {
                    CRUD.Excluir(new MLListaConteudoAudio
                    {
                        Codigo = model.Codigo,
                        CodigoConteudo = model.CodigoConteudo
                    }, portal.ConnectionString);
                }
            }
            return true;
        }
        #endregion

        #region ExcluirArquivoAudio
        public bool ExcluirArquivoAudio(Guid guid, string file, MLPortal obter)
        {
            var portal = PortalAtual.Obter;

            var conteudo = new BLListaConteudo().Obter(new MLListaConteudo { GUID = guid }, portal.ConnectionString);
            if (conteudo != null && conteudo.Codigo.HasValue && conteudo.CodigoLista.HasValue)
            {
                if (!new BLListaConfig().ValidarPermissaoLista(conteudo.CodigoLista.Value)) return false;

                var diretorio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/Audio").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                if (!Directory.Exists(pasta)) return false;
                if (!File.Exists(Path.Combine(pasta, file))) return false;

                File.Delete(Path.Combine(pasta, file));
            }

            return true;
        }
        #endregion

        #region ListarConteudoAudioMaisArquivosEmDisco
        public List<MLListaConteudoAudio> ListarConteudoAudioMaisArquivosEmDisco(MLListaConteudo model)
        {
            var portal = PortalAtual.Obter;
            var listaAudio = base.Listar(new MLListaConteudoAudio { CodigoConteudo = model.Codigo }, "Ordem", "ASC", portal.ConnectionString);

            var diretorioAudio = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/Audio").Replace("//", "/");

            var pastaAudio = HttpContextFactory.Current.Server.MapPath(diretorioAudio);
            if (!Directory.Exists(pastaAudio)) Directory.CreateDirectory(pastaAudio);

            var arquivosAudio = new List<string>(Directory.GetFiles(pastaAudio));
            for (var i = 0; i < arquivosAudio.Count; i++)
            {
                var nome = Path.GetFileName(arquivosAudio[i]);
                if (listaAudio.Find(o => o.Audio == nome) == null)
                {
                    listaAudio.Add(new MLListaConteudoAudio { CodigoConteudo = model.Codigo, Audio = nome });
                }
            }

            return listaAudio;
        }
        #endregion

        #region ListarAudio
        public List<MLListaConteudoAudio> ListarAudio(decimal? id, string guid)
        {
            MLListaConteudoAudio criterios = new MLListaConteudoAudio();

            if (id.HasValue)
                criterios.CodigoConteudo = id;
            else
                criterios.GuidConteudo = new Guid(guid);

            return new BLListaConteudoAudio().Listar(criterios, "Ordem", "ASC", PortalAtual.ConnectionString);
        }
        #endregion
    }

    #endregion

    public class BLListaConteudoSEO : BLCRUD<MLListaConteudoSEO> { }

    public class BLListaConteudoPublicado : BLCRUD<MLListaConteudoPublicado> { }
}