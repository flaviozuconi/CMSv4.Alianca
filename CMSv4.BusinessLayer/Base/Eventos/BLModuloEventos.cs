using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Módulo de Eventos
    /// </summary>
    public class BLModuloEventos : BLCRUD<MLEvento>
    {
        #region Salvar

        public decimal Salvar(MLEvento model)
        {
            try
            {
                var Request = HttpContextFactory.Current.Request;
                var portal = PortalAtual.Obter;

                bool? RemoverCapa = Request.Form["RemoverCapa"] != null ? Convert.ToBoolean(Request.Form["RemoverCapa"]) : false;

                model.CodigoPortal = portal.Codigo;
                model.Conteudo = model.Conteudo == null ? string.Empty : model.Conteudo;
                model.Conteudo.Escape();
                model.Ativo = model.Ativo.GetValueOrDefault(false);
                model.Destaque = model.Destaque.GetValueOrDefault(false);

                #region Remover Imagem
                if (RemoverCapa.GetValueOrDefault() && !string.IsNullOrEmpty(model.Imagem))
                {
                    var diretorio = string.Concat(BLConfiguracao.Pastas.ModuloEvento(portal.Diretorio), "/", model.Codigo, "/");
                    var nomeImagem = string.Concat(diretorio, "capa", model.Imagem);

                    var caminhoArquivo = HttpContextFactory.Current.Server.MapPath(nomeImagem);

                    FileInfo fi = new FileInfo(caminhoArquivo);
                    if (fi.Exists)
                    {
                        try
                        {
                            fi.Delete();
                        }
                        catch { }
                    }

                    model.Imagem = String.Empty;
                }
                #endregion

                model.Codigo = this.Salvar(model, portal.ConnectionString);


                model.Seo.Codigo = model.Codigo;
                CRUD.Salvar<MLEventoSEO>(model.Seo, portal.ConnectionString);

                #region Salvar Imagem

                if (HttpContextFactory.Current.Request.Files.Count > 0)
                {
                    var imagem = HttpContextFactory.Current.Request.Files[0];

                    model.Imagem = ".png";

                    var diretorio = string.Concat(BLConfiguracao.Pastas.ModuloEvento(portal.Diretorio), "/", model.Codigo, "/");
                    var nomeImagem = string.Concat(diretorio, "capa", model.Imagem);
                    var di = new DirectoryInfo(HttpContextFactory.Current.Server.MapPath(diretorio));
                    var di_thumb = new DirectoryInfo(HttpContextFactory.Current.Server.MapPath(string.Concat(diretorio, "/_thumb")));
                    var fi = new FileInfo(HttpContextFactory.Current.Server.MapPath(nomeImagem));

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
                    imagem.SaveAs(HttpContextFactory.Current.Server.MapPath(nomeImagem));

                    this.SalvarParcial(model, portal.ConnectionString);
                }
                #endregion

                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }  
        #endregion

        #region Obter

        /// <summary>
        /// Obter
        /// </summary>
        public static decimal? Obter(string url, decimal? id)
        {
            using (var command = Database.NewCommand("USP_MOD_EVE_S_EVENTO", BLPortal.Atual.ConnectionString))
            {
                // Parametros                                
                command.NewCriteriaParameter("@EVE_C_URL", SqlDbType.VarChar, 250, url);
                command.NewCriteriaParameter("@EVE_N_CODIGO", SqlDbType.Decimal, 18, id);

                return Convert.ToDecimal(Database.ExecuteScalar(command));
            }
        }

        public MLEvento Obter(decimal? id, string CodigoIdioma, string CodigoBase)
        {
            MLEvento model = null;

            if (id.HasValue)
            {
                model = Obter(new MLEvento { Codigo = id }, PortalAtual.ConnectionString);
                model.Seo = CRUD.Obter<MLEventoSEO>(id.Value, PortalAtual.ConnectionString);
                model.Conteudo = model.Conteudo.Unescape();
            }

            if (model == null)
            {
                model = new MLEvento();
                model.CodigoIdioma = PortalAtual.Obter.CodigoIdioma;
            }

            if (!id.HasValue)
            {
                if (!string.IsNullOrEmpty(CodigoIdioma))
                    model.CodigoIdioma = Convert.ToDecimal(CodigoIdioma);

                if (!string.IsNullOrEmpty(CodigoBase))
                    model.CodigoBase = Convert.ToDecimal(CodigoBase);
            }

            return model;
        }

        #endregion

        #region ListarCalendario

        /// <summary>
        /// LISTAR PARA POPULAR CALENDÁRIO DA ÁREA ADMIN
        /// </summary>
        public static List<MLEventoLista> ListarCalendario(MLEvento model)
        {
            using (var command = Database.NewCommand("USP_MOD_EVE_L_EVENTOS_CALENDARIO_ADM", BLPortal.Atual.ConnectionString))
            {
                // Parametros                                
                command.NewCriteriaParameter("@EVE_D_INICIO", SqlDbType.DateTime, model.DataInicio);
                command.NewCriteriaParameter("@EVE_D_TERMINO", SqlDbType.DateTime, model.DataTermino);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);

                if (model.CodigoIdioma.HasValue)
                {
                    command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoIdioma);
                }
                
                // Execucao
                return Database.ExecuteReader<MLEventoLista>(command);
            }
        }

        #endregion
     
    }
}