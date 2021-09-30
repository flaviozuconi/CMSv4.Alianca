using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Web;
using System.Transactions;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLArquivos : BLCRUD<MLArquivo>
    {
        #region ListarDestaque

        /// <summary>
        /// LISTAR DESTAQUE ATIVOS
        /// </summary>
        /// <param name="intTop">Quantidade de Registros</param>
        /// <param name="IsCache">Define se deve utilizar o cache</param>
        public static List<MLArquivoPublico> ListarDestaque(MLModuloArquivos model, bool IsCache)
        {
            var portal = BLPortal.Atual;
            var cacheKey = string.Format("portal_{0}_arquivo_listagem_quantidade_{1}_categorias_{2}_idioma_{3}",
                 portal.Codigo, model.Quantidade, model.Categorias, BLPortal.Atual.Codigo);

            var cachedValue = BLCachePortal.Get<List<MLArquivoPublico>>(cacheKey);

            if (cachedValue != null && IsCache)
                return cachedValue;

            using (var command = Database.NewCommand("USP_MOD_ARQ_L_ARQUIVOS_DESTAQUE", portal.ConnectionString))
            {
                // Parametros                        
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@CATEGORIAS", SqlDbType.VarChar, model.Categorias);


                // Execucao

                var lstRetorno = Database.ExecuteReader<MLArquivoPublico>(command);
                BLCachePortal.Add(portal.Codigo.Value, cacheKey, lstRetorno, 1);

                return lstRetorno;
            }
        }

        #endregion

        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLArquivoPublico> ListarPublico(MLModuloArquivos model, bool IsCache)
        {
            var portal = BLPortal.Atual;
            var cacheKey = string.Format("portal_{0}_arquivo_listagem_quantidade_{1}_pagina_{2}_categorias_{3}_ordenardata_{4}_ordenardesc_{5}",
                portal.Codigo, model.Quantidade, model.Pagina, model.Categorias, model.OrdenarData.Value, model.OrdenarDesc.Value);

            var cachedValue = BLCachePortal.Get<List<MLArquivoPublico>>(cacheKey);

            if (cachedValue != null && IsCache)
                return cachedValue;

            using (var command = Database.NewCommand("USP_MOD_ARQ_L_ARQUIVOS_PUBLICO_II", BLPortal.Atual.ConnectionString))
            {
                // Parametros                                
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, model.Pagina);
                command.NewCriteriaParameter("@CATEGORIAS", SqlDbType.VarChar, model.Categorias);
                command.NewCriteriaParameter("@ORDERBY", SqlDbType.Bit, model.OrdenarData);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, model.OrdenarDesc);

                // Execucao
                var lstRetorno = Database.ExecuteReader<MLArquivoPublico>(command);
                BLCachePortal.Add(portal.Codigo.Value, cacheKey, lstRetorno, 1);

                return lstRetorno;
            }
        }

        #endregion

        #region ListarAno

        /// <summary>
        /// LISTAR PUBLICO POR ANO
        /// </summary>
        public static List<MLArquivoPublico> ListarAno(string categorias, int ano)
        {
            using (var command = Database.NewCommand("USP_MOD_ARQ_L_ARQUIVOS_PUBLICO_ANO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@CATEGORIAS", SqlDbType.VarChar, categorias);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, ano);

                // Execucao
                return Database.ExecuteReader<MLArquivoPublico>(command);
            }
        }

        #endregion


        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;

                foreach (var item in ids)
                {
                    //Obter para model com as informações para excluir os arquivos.
                    var modelArquivo = CRUD.Obter<MLArquivo>(Convert.ToDecimal(item));
                    var modelCategoria = CRUD.Obter<MLArquivoCategoria>(modelArquivo.CodigoCategoria.Value, portal.ConnectionString);
                    var diretorioVirtualImagem = BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, modelArquivo.Codigo.Value);
                    var diretorioFisicoImagem = HttpContextFactory.Current.Server.MapPath(diretorioVirtualImagem);

                    if (modelCategoria != null && !string.IsNullOrWhiteSpace(modelCategoria.Nome))
                    {
                        var arquivo = HttpContextFactory.Current.Server.MapPath(string.Concat("~/Portal/", portal.Diretorio, "/arquivos/", modelCategoria.Nome.Replace("/", ""), "/", modelArquivo.Nome));

                        if (File.Exists(arquivo))
                        {
                            try
                            {
                                File.Delete(arquivo);
                                BLReplicar.ExcluirArquivosReplicados(arquivo);
                            }
                            catch { }
                        }
                    }                    

                    if(Directory.Exists(diretorioFisicoImagem))
                    {
                        try
                        {
                            var imagem = Path.Combine(diretorioFisicoImagem, modelArquivo.Nome);

                            if (File.Exists(imagem))
                            {
                                File.Delete(imagem);
                                BLReplicar.ExcluirArquivosReplicados(imagem);
                            }
                                

                            Directory.Delete(diretorioFisicoImagem, true);
                            BLReplicar.ExcluirDiretoriosReplicados(diretorioVirtualImagem);
                        }
                        catch {}
                    }

                    base.Excluir(Convert.ToDecimal(item), PortalAtual.ConnectionString);
                }

                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Obter

        public MLArquivo Obter(decimal Codigo, decimal CodigoCategoria, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = new MLArquivo()
                {
                    CodigoIdioma = PortalAtual.Obter.CodigoIdioma,
                    CodigoCategoria = CodigoCategoria
                    
                };

                if (Codigo > 0)
                {
                    model = base.Obter(Codigo, portal.ConnectionString);

                    if (!string.IsNullOrEmpty(model.Descricao))
                        model.Descricao = model.Descricao.Unescape();
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Salvar

        public decimal Salvar(MLArquivo model, HttpPostedFileBase imgThumb, bool? RemoverImg)
        {
            var portal = PortalAtual.Obter;

            try
            {
                using (var scope = new TransactionScope(portal.ConnectionString))
                {
                    if (string.IsNullOrEmpty(model.Nome))
                        throw new Exception("Nenhum arquivo selecionado!");

                    var modelCategoria = CRUD.Obter<MLArquivoCategoria>(model.CodigoCategoria.Value, portal.ConnectionString);

                    if (!File.Exists(Path.Combine(modelCategoria.PastaFisica(portal.Diretorio), model.Nome)))
                        throw new Exception("Falha ao realizar upload do arquivo!");

                    model.Ativo = model.Ativo.GetValueOrDefault(false);
                    model.Destaque = model.Destaque.GetValueOrDefault(false);
                    

                    model.CodigoPortal = portal.Codigo;
                    model.LogPreencher(model.Codigo.HasValue);
                    //model.PastaRelativa = modelCategoria.PastaRelativa(portal.Diretorio);

                    if ((imgThumb != null && imgThumb.ContentLength > 0) && (!(RemoverImg ?? false)))
                    {
                        if (!Directory.Exists(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(portal.Diretorio))))
                        {
                            Directory.CreateDirectory(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(portal.Diretorio)));
                            BLReplicar.Diretorio(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(portal.Diretorio)));
                        }


                        /// Excluir a imagem antiga
                        if (!string.IsNullOrEmpty(model.Imagem))
                        {
                            if (File.Exists(HttpContextFactory.Current.Server.MapPath(Path.Combine(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value), model.Imagem))))
                            {
                                File.Delete(HttpContextFactory.Current.Server.MapPath(Path.Combine(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value), model.Imagem)));
                                BLReplicar.ExcluirArquivosReplicados(HttpContextFactory.Current.Server.MapPath(Path.Combine(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value), model.Imagem)));
                            }

                        }

                        model.Imagem = imgThumb.FileName;
                    }
                    else if ((RemoverImg ?? false))
                    {
                        /// Excluir a imagem antiga
                        if (!string.IsNullOrEmpty(model.Imagem))
                        {
                            var file = HttpContextFactory.Current.Server.MapPath(Path.Combine(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value), model.Imagem));

                            if (File.Exists(file))
                            {
                                File.Delete(file);
                                BLReplicar.ExcluirArquivosReplicados(file);
                            }
                        }

                        model.Imagem = null;
                    }

                    if (!String.IsNullOrEmpty(model.Descricao))
                        model.Descricao = model.Descricao.Escape();

                    model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

                    if (!Directory.Exists(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value))))
                    {
                        Directory.CreateDirectory(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value)));
                        BLReplicar.Diretorio(HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value)));
                    }

                    if (imgThumb != null && !string.IsNullOrEmpty(model.Imagem))
                    {
                        var dir = BLConfiguracao.Pastas.ModuloArquivosImagens(portal.Diretorio, model.Codigo.Value);

                        imgThumb.SaveAs(HttpContextFactory.Current.Server.MapPath(Path.Combine(dir, imgThumb.FileName)));
                        BLReplicar.Arquivo(HttpContextFactory.Current.Server.MapPath(Path.Combine(dir, imgThumb.FileName)), dir);
                    }

                    scope.Complete();
                }

                return model.Codigo.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region UploadFile

        public bool UploadFile(decimal CodigoCategoria)
        {
            try
            {
                var portal = PortalAtual.Obter;
                string caminhoArquivo = string.Empty;

                var modelCategoria = CRUD.Obter<MLArquivoCategoria>(CodigoCategoria, portal.ConnectionString);
                var di = new DirectoryInfo(modelCategoria.PastaFisica(portal.Diretorio));

                if (!di.Exists)
                {
                    di.Create();
                    BLReplicar.Diretorio(modelCategoria.PastaFisica(portal.Diretorio));
                }

                if (HttpContextFactory.Current.Request.Files.Count > 0)
                {
                    var file = HttpContextFactory.Current.Request.Files[0];
                    caminhoArquivo = modelCategoria.PastaFisica(portal.Diretorio) + "/" + file.FileName;
                    file.SaveAs(caminhoArquivo);
                    BLReplicar.Arquivo(caminhoArquivo, modelCategoria.PastaRelativa(portal.Diretorio));
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
