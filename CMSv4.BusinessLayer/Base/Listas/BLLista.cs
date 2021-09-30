using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// LISTAS
    /// </summary>
    public class BLLista : BLCRUD<MLListaConteudo>
    {
        #region InserirAvaliacao
        /// <summary>
        /// InserirAvaliacao
        /// </summary>
        /// <param name="objML"></param>
        /// <returns></returns>
        public static MLAvaliacaoRetorno InserirAvaliacao(MLListaConteudoAvaliacao objML)
        {
            MLAvaliacaoRetorno retorno = new MLAvaliacaoRetorno();

            try
            {
                using (var command = Database.NewCommand("USP_MOD_MEN_I_LISTA_CONTEUDO_AVALIACAO", BLPortal.Atual.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@LCA_LIT_N_CODIGO", SqlDbType.Decimal, 18, objML.CodigoConteudo);
                    command.NewCriteriaParameter("@LCA_CLI_N_CODIGO", SqlDbType.Decimal, 18, objML.CodigoCliente);
                    command.NewCriteriaParameter("@LCA_N_NOTA", SqlDbType.Int, objML.Nota);
                    command.NewCriteriaParameter("@LCA_C_IP", SqlDbType.VarChar, 50, objML.IP);
                    command.NewCriteriaParameter("@LCA_D_CADASTRO", SqlDbType.DateTime, objML.DataCadastro);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    return (MLAvaliacaoRetorno)Database.FillModel<MLAvaliacaoRetorno>(dataset.Tables[0].Rows[0]);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Obter
        /// <summary>
        /// Obter Lista Via URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="codigoLista"></param>
        /// <returns></returns>
        public static MLListaConteudo Obter(string url, decimal codigoLista)
        {
            MLListaConteudo retorno = null;

            try
            {
                var portal = BLPortal.Atual;

                using (var command = Database.NewCommand("USP_MOD_LIS_S_LISTA_CONTEUDO", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@LIT_C_URL", SqlDbType.VarChar, 500, url);
                    command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, codigoLista);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLListaConteudo();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLListaConteudo>(dataset.Tables[0].Rows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }

        #endregion

        #region ObterCompleto

        /// <summary>
        /// Obter todas as informações da matéria (Galeria, Videos, Audios, SEO)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="codigoLista"></param>
        /// <returns></returns>
        public static MLListaConteudoCompleto ObterCompleto(string url, decimal codigoLista, decimal? CodigoCliente = null, string IP = "")
        {
            MLListaConteudoCompleto retorno = null;

            try
            {
                var portal = BLPortal.Atual;
                var cacheKey = string.Format("USP_MOD_LIS_S_LISTA_CONTEUDO_COMPLETO_{0}_{1}", url, codigoLista);
                var cachedValue = BLCachePortal.Get<MLListaConteudoCompleto>(cacheKey);

                //Procura no cache caso esteja configurado, e escolhido pelo parametro
                if (HttpContext.Current.Request.QueryString["reset"] == null && cachedValue != null)
                    return cachedValue;

                using (var command = Database.NewCommand("USP_MOD_LIS_S_LISTA_CONTEUDO_COMPLETO", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@LIT_C_URL", SqlDbType.VarChar, 500, url);
                    command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, codigoLista);
                    command.NewCriteriaParameter("@LCA_C_IP", SqlDbType.VarChar, 50, IP);
                    command.NewCriteriaParameter("@LCA_CLI_N_CODIGO", SqlDbType.Decimal, 18, CodigoCliente);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLListaConteudoCompleto();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        retorno = Database.FillModel<MLListaConteudoCompleto>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                            retorno.Imagens = Database.FillList<MLListaConteudoImagem>(dataset.Tables[1]);

                        if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                            retorno.Videos = Database.FillList<MLListaConteudoVideo>(dataset.Tables[2]);

                        if (dataset.Tables.Count > 3 && dataset.Tables[3].Rows.Count > 0)
                            retorno.Audios = Database.FillList<MLListaConteudoAudio>(dataset.Tables[3]);

                        if (dataset.Tables.Count > 4 && dataset.Tables[4].Rows.Count > 0)
                            retorno.Seo = Database.FillModel<MLListaConteudoSEOPublicado>(dataset.Tables[4].Rows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }

        #endregion

        #region ObterUrlValidacao

        /// <summary>
        /// Obter
        /// </summary>
        public static decimal? ObterUrlValidacao(string url, decimal? id, decimal CodigoLista)
        {
            using (var command = Database.NewCommand("USP_MOD_S_LIS_LISTA_CONTEUDO_VALIDAR_URL", BLPortal.Atual.ConnectionString))
            {
                // Parametros                                
                command.NewCriteriaParameter("@LIT_C_URL", SqlDbType.VarChar, 250, url);
                command.NewCriteriaParameter("@LIT_N_CODIGO", SqlDbType.Decimal, 18, id);
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, CodigoLista);

                return Convert.ToDecimal(Database.ExecuteScalar(command));
            }
        }

        #endregion

        #region ListarRelacionados

        /// <summary>
        /// LISTAR NOTÍCIAS RELACIONADAS POR TAG
        /// </summary>
        public static List<MLListaConteudoRelacionados> ListarRelacionados
        (
            decimal decCodigoLista, 
            decimal decCodigoConteudo,
            decimal decCodigoPortal, 
            decimal decCodigoIdioma, 
            decimal? decQuandade,
            string strTags
        )
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_LIS_S_LISTACONTEUDO_RELACIONADOS", BLPortal.Atual.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, decCodigoPortal);
                    command.NewCriteriaParameter("@LIT_N_CODIGO", SqlDbType.Decimal, 18, decCodigoConteudo);
                    command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoIdioma);
                    command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, decQuandade);
                    command.NewCriteriaParameter("@TAGS", SqlDbType.VarChar, -1, strTags);

                    // Execucao
                    return Database.ExecuteReader<MLListaConteudoRelacionados>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Publicar
        /// <summary>
        /// Publicar um item da lista
        /// </summary>
        public static void Publicar(MLPortal portal, decimal id)
        {
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_PUBLICAR", portal.ConnectionString))
            {
                command.NewCriteriaParameter("@LIT_N_CODIGO", SqlDbType.Decimal, 18, id);

                Database.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Publicar um item da lista
        /// </summary>
        public static void PublicarTodos()
        {
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_PUBLICAR_TODOS"))
            {
                Database.ExecuteNonQuery(command);
            }
        }

        #endregion

        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLListaConteudoPublicoListagem> ListarPublico(MLModuloLista model, int? paginaAtual, bool? destaque = null, decimal? codigoIdioma = null)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_PUBLICO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, paginaAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@DESTAQUE", SqlDbType.Bit, destaque);
                command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);

                // Execucao
                return Database.ExecuteReader<MLListaConteudoPublicoListagem>(command);
            }
        }

         public static List<MLListaConteudoPublicoListagem> ListarPublicoLista(MLModuloLista model, int? paginaAtual, bool? destaque = null, decimal? codigoIdioma = null)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_PUBLICO_LISTA", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, paginaAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@DESTAQUE", SqlDbType.Bit, destaque);
                command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);
                command.NewCriteriaParameter("@MOD_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);

                // Execucao
                return Database.ExecuteReader<MLListaConteudoPublicoListagem>(command);
            }
        }

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLListaConteudoPublicoListagem> ListarPublicoAdmin(MLModuloLista model, int? paginaAtual, bool? destaque = null, decimal? codigoIdioma = null)
        {
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_PUBLICO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, model.Categorias);                
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, paginaAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@DESTAQUE", SqlDbType.Bit, destaque);
                command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);

                if (!string.IsNullOrWhiteSpace(model.Titulo))
                {
                    command.NewCriteriaParameter("@TITULO", SqlDbType.VarChar, 250, model.Titulo);
                }

                // Execucao
                return Database.ExecuteReader<MLListaConteudoPublicoListagem>(command);
            }
        }

        #endregion

        #region ListarPorAno

        /// <summary>
        /// LISTAR PUBLICO POR ANO
        /// </summary>
        public static List<MLListaConteudoPublicoListagem> ListarPorAno(MLModuloLista model, int ano)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_ANO_PUBLICO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@PORTAL", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, ano);

                // Execucao
                return Database.ExecuteReader<MLListaConteudoPublicoListagem>(command);
            }
        }

        #endregion

        #region Listagem Comunicação

        #region ListarComunicacaoAno

        /// <summary>
        /// Utilizado na página de Comunicação, filtro para ano
        /// </summary>
        public static List<MLFiltroAno> ListarComunicacaoAno(MLModuloLista model, decimal decIdioma)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_FILTRO_ANO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@PORTAL", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, decIdioma);    

                // Execucao
                return Database.ExecuteReader<MLFiltroAno>(command);
            }
        }

        #endregion

        #region ListarComunicacaoMes

        /// <summary>
        /// Utilizado na página de Comunicação, filtro para ano
        /// </summary>
        public static List<MLFiltroMes> ListarComunicacaoMes(decimal decCodigoLista, string strCategorias, decimal? decIdioma, int intAno)
        {
            var portal = BLPortal.Atual;
            
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_FILTRO_MES", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, strCategorias);
                command.NewCriteriaParameter("@PORTAL", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, decIdioma);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, intAno);

                // Execucao
                return Database.ExecuteReader<MLFiltroMes>(command);
            }
        }

        #endregion

        #region ListaFiltroBuscar

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLListaConteudoPublicoListagem> ListaFiltroBuscar
        (
            decimal decCodigoLista,
            string strCategorias,
            int intPaginaAtual, 
            int intQuantidade,
            decimal? codigoIdioma,
            int? intAno,
            int? intMes,
            string busca
        )
        {
            var portal = BLPortal.Atual;
            
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_FILTRO_BUSCAR", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                command.NewCriteriaParameter("@LIC_N_CODIGOS", SqlDbType.VarChar, 100, strCategorias);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, intPaginaAtual);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intQuantidade);
                command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, intAno);
                command.NewCriteriaParameter("@MES", SqlDbType.Int, intMes);
                command.NewCriteriaParameter("@BUSCA", SqlDbType.VarChar, 200, busca);

                // Execucao
                var retorno = Database.ExecuteReader<MLListaConteudoPublicoListagem>(command);

                return retorno;
            }
        }

        #endregion

        #region ListarComunicacaoTemas

        /// <summary>
        /// Utilizado na página de Comunicação, filtro para ano
        /// </summary>
        public static List<MLListaConfigCategoria> ListarComunicacaoTemas(MLModuloLista model)
        {
            var portal = BLPortal.Atual;
            
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_FILTRO_CATEGORIAS", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoLista);
                command.NewCriteriaParameter("@PORTAL", SqlDbType.Decimal, 18, portal.Codigo);

                if (!string.IsNullOrWhiteSpace(model.Categorias))
                {
                    command.NewCriteriaParameter("@CATEGORIA", SqlDbType.VarChar, -1, model.Categorias);
                }

                // Execucao
                return Database.ExecuteReader<MLListaConfigCategoria>(command);
            }
        }

        #endregion

        #endregion

        #region ListarAnos

        /// <summary>
        /// LISTAR ANOS
        /// </summary>
        public static List<string> ListarAnos()
        {
            var portal = BLPortal.Atual;
            var lista = new List<string>();
            var cacheKey = string.Format("portal_{0}_lista_anos_{1}", portal.Codigo, BLIdioma.CodigoAtual);
            var cachedValue = BLCachePortal.Get<List<string>>(cacheKey);

            // Procura no cache caso esteja configurado, e escolhido pelo parametro
            if (HttpContext.Current.Request.QueryString["reset"] == null && cachedValue != null)
                return cachedValue;

            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTACONTEUDO_ANOS", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, BLIdioma.CodigoAtual);

                // Execucao
                using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        lista.Add(Convert.ToString(reader.GetValue(0)));
                    }

                    Database.CloseReader(reader);
                    Database.CloseConnection(command);
                    BLCachePortal.Add(cacheKey, lista);
                }

                return lista;
            }
        }

        #endregion

        #region Eventos

        public class Eventos
        {
            #region Listar

            /// <summary>
            /// Listar os eventos por ano e semestre com paginação.
            /// </summary>
            /// <user>rvissontai</user>
            public static List<MLEventosLTP> Listar
            (
                decimal decCodigoLista,
                decimal decCodigoPortal,
                decimal decCodigoIdioma,
                int intPagina,
                int? intAno,
                int? intSemestre,
                int? intQuantidade
            )
            {
                try
                {
                    using (var command = Database.NewCommand("USP_LTP_MOD_LIS_L_LISTACONTEUDO_EVENTOS", BLPortal.Atual.ConnectionString))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                        command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoIdioma);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, decCodigoPortal);
                        command.NewCriteriaParameter("@ANO", SqlDbType.Int, intAno);
                        command.NewCriteriaParameter("@SEMESTRE", SqlDbType.Int, intSemestre);
                        command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, intPagina);
                        command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intQuantidade);


                        // Execucao
                        return Database.ExecuteReader<MLEventosLTP>(command);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
            }

            #endregion

            #region ListarDestaquePopup

            public static MLEventosLTP ListarDestaquePopup
            (
                decimal decCodigoLista,
                decimal decCodigoPortal,
                decimal decCodigoIdioma
            )
            {
                var cachedKey = string.Format("USP_LTP_MOD_LIS_S_LISTACONTEUDO_EVENTO_POPUP_{0}_{1}_{2}", decCodigoLista, decCodigoPortal, decCodigoIdioma);
                var retorno = BLCachePortal.Get<MLEventosLTP>(cachedKey);


                if (retorno != null)
                    return retorno;

                try
                {
                    using (var command = Database.NewCommand("USP_LTP_MOD_LIS_S_LISTACONTEUDO_EVENTO_POPUP", BLPortal.Atual.ConnectionString))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                        command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoIdioma);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, decCodigoPortal);

                        var dataset = Database.ExecuteDataSet(command);

                        if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                        {
                            retorno = Database.FillModel<MLEventosLTP>(dataset.Tables[0].Rows[0]);
                            BLCachePortal.Add(cachedKey, retorno);
                        }

                        // Execucao
                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
            }

            #endregion

            #region ListarFiltros

            public static List<int> ListarFiltros
            (
                decimal decCodigoLista,
                decimal decCodigoPortal,
                decimal decCodigoIdioma
            )
            {
                try
                {
                    List<int> retorno = new List<int>();

                    using (var command = Database.NewCommand("USP_LTP_MOD_LIS_L_LISTACONTEUDO_EVENTOS_FILTROS", BLPortal.Atual.ConnectionString))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                        command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoIdioma);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, decCodigoPortal);

                        // Execucao
                        var dataset = Database.ExecuteDataSet(command);

                        if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0].ItemArray.Count() > 1)
                        {
                            int anoTermino = Convert.ToInt16(dataset.Tables[0].Rows[0].ItemArray[0]);
                            int anoInicio = Convert.ToInt16(dataset.Tables[0].Rows[0].ItemArray[1]);

                            for(int i = anoTermino; i >= anoInicio; i--)
                                retorno.Add(i);
                        }

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
            }

            #endregion

            #region ListarUltimos

            public static List<MLEventosLTP> ListarUltimos
            (
                decimal decCodigoLista,
                decimal decCodigoPortal,
                decimal decCodigoIdioma,
                int intQuantidade,
                bool? Destaque = null
            )
            {
                try
                {
                    using (var command = Database.NewCommand("USP_LTP_MOD_LIS_L_LISTACONTEUDO_ULTIMOS_EVENTOS", BLPortal.Atual.ConnectionString))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18, decCodigoLista);
                        command.NewCriteriaParameter("@LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoIdioma);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, decCodigoPortal);
                        command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intQuantidade);
                        command.NewCriteriaParameter("@DESTAQUE", SqlDbType.Bit, Destaque);

                        // Execucao
                        return Database.ExecuteReader<MLEventosLTP>(command);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
            }

            #endregion
        }

        #endregion
    }
}
