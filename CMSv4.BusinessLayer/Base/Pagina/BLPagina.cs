using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using VM2.PageSpeed;
using VM2.PageSpeed.BusinessLayer;
using VM2.PageSpeed.Model;

namespace CMSv4.BusinessLayer
{
    public class BLPagina
    {
        private string CONNECTION_STRING = "";

        public BLPagina(string connectionSting)
        {
            CONNECTION_STRING = connectionSting;
        }

        #region Excluir

        public static void Excluir(decimal CodigoPagina)
        {
            Excluir(new List<string>() { CodigoPagina.ToString() });
        }

        public static void Excluir(List<string> ids)
        {
            foreach (var codigoPagina in ids)
            {
                var model = CRUD.Obter<MLPagina>(Convert.ToDecimal(codigoPagina), PortalAtual.ConnectionString);
             
                CRUD.SalvarParcial(new MLPagina
                {
                    Codigo = model.Codigo,
                    Excluida = true,
                    LogDataAlteracao = DateTime.Now,
                    LogUsuarioAlteracao = BLUsuario.ObterLogado().Codigo
                }, PortalAtual.ConnectionString);
            }
        }

        #endregion

        #region ListarAdmin

        /// <summary>
        /// Listar páginas na área administrativa
        /// </summary>
        /// <returns></returns>
        public List<MLPagina> ListarAdmin(string buscaGenerica, decimal? codigoPortal, decimal? codigoSecao, string gruposUsuario, bool? ativo, string orderBy, string sortOrder, int start, int length, decimal? codigoIdioma)
        {
            try
            {
                using (var command = Database.NewCommand("USP_CMS_L_PAGINA_ADMIN", CONNECTION_STRING))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, -1, buscaGenerica);
                    command.NewCriteriaParameter("@SEC_N_CODIGO", SqlDbType.Decimal, 18, codigoSecao);
                    command.NewCriteriaParameter("@GRUPOS_USUARIO", SqlDbType.VarChar, -1, gruposUsuario);
                    command.NewCriteriaParameter("@PAG_B_ATIVO", SqlDbType.Bit, ativo);
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, codigoPortal);
                    command.NewCriteriaParameter("@PAG_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);

                    command.NewCriteriaParameter("@ORDERBY", SqlDbType.VarChar, 100, orderBy);
                    command.NewCriteriaParameter("@ASC", SqlDbType.Bit, (sortOrder == "asc") );
                    command.NewCriteriaParameter("@START", SqlDbType.Int, start);
                    command.NewCriteriaParameter("@LENGTH", SqlDbType.Int, length);

                    // Execucao
                    return Database.ExecuteReader<MLPagina>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Obter Pagina Admin

        /// <summary>
        /// Obtem a página completa para ser utilizada na área administrativa
        /// </summary>
        public MLPaginaAdmin ObterPaginaAdmin(decimal id)
        {
            var model = new MLPaginaAdmin();

            using (var command = Database.NewCommand("USP_CMS_S_PAGINA_ADMIN", CONNECTION_STRING))
            {
                // Parametros
                command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, id);

                // Execucao
                var dataSet = Database.ExecuteDataSet(command);

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    // Pagina
                    model = Database.FillModel<MLPaginaAdmin>(dataSet.Tables[0].Rows[0]);

                    // Pagina em Edicao
                    if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                        model.PaginaEdicao = Database.FillModel<MLPaginaEdicao>(dataSet.Tables[1].Rows[0]);

                    // Modulos da Pagina em Edicao
                    if (dataSet.Tables.Count > 2 && dataSet.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[2].Rows)
                        {
                            model.PaginaEdicao.Modulos.Add(Database.FillModel<MLPaginaModuloEdicao>(row));
                        }
                    }

                    // Pagina Publicada
                    if (dataSet.Tables.Count > 3 && dataSet.Tables[3].Rows.Count > 0)
                    {
                        model.PaginaPublicada = Database.FillModel<MLPaginaPublicada>(dataSet.Tables[3].Rows[0]);
                        model.PaginaPublicada.PageSpeed = new MLPaginaPageSpeed()
                        {
                            CodigoPagina = model.PaginaPublicada.Codigo
                        };
                    }

                    // Modulos da Pagina Publicada
                    if (dataSet.Tables.Count > 4 && dataSet.Tables[4].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[4].Rows)
                        {
                            model.PaginaPublicada.Modulos.Add(Database.FillModel<MLPaginaModuloPublicado>(row));
                        }
                    }

                    //Dados de SEO
                    if (dataSet.Tables.Count > 5 && dataSet.Tables[5].Rows.Count > 0)
                        model.Seo = Database.FillModel<MLPaginaSeo>(dataSet.Tables[5].Rows[0]);

                    //Dados do page speed
                    if (model.PaginaPublicada.Codigo.HasValue && dataSet.Tables.Count > 6 && dataSet.Tables[6].Rows.Count > 0)
                    {
                        model.PaginaPublicada.PageSpeed = Database.FillModel<MLPaginaPageSpeed>(dataSet.Tables[6].Rows[0]);

                        if (!string.IsNullOrWhiteSpace(model.PaginaPublicada.PageSpeed.JsonResult))
                        {
                            model.PaginaPublicada.PageSpeed.Analise = JsonConvert.DeserializeObject<PageSpeedApiResponseV5>(model.PaginaPublicada.PageSpeed.JsonResult);

                            var categories = model.PaginaPublicada.PageSpeed.Analise.LighthouseResult.Categories;
                            var audits = model.PaginaPublicada.PageSpeed.Analise.LighthouseResult.Audits;

                            var accessibility = new BLPageSpeedTratarModelBase(categories.Accessibility, audits);
                            var bestPractices = new BLPageSpeedTratarModelBase(categories.BestPractices, audits);
                            var seo           = new BLPageSpeedTratarModelBase(categories.Seo, audits);

                            model.PaginaPublicada.PageSpeed.Performance   = new BLPageSpeedPerformanceTratarModel(model.PaginaPublicada.PageSpeed.Analise).BindScore();
                            model.PaginaPublicada.PageSpeed.Accessibility = accessibility.BindScore();
                            model.PaginaPublicada.PageSpeed.BestPractices = bestPractices.BindScore();
                            model.PaginaPublicada.PageSpeed.Seo           = seo.BindScore();
                        }
                    }
                        
                }
            }

            return model;
        }

        #endregion

        #region ObterPageSpeed
        
        public static MLPageSpeedViewModel ObterPageSpeed(decimal CodigoPagina)
        {
            var modelPaginaPageSpeed = CRUD.Obter(new MLPaginaPageSpeed() { CodigoPagina = CodigoPagina });

            var modelAnalise = JsonConvert.DeserializeObject<PageSpeedApiResponseV5>(modelPaginaPageSpeed.JsonResult);

            var modelRetorno = new PageSpeedBindViewModel(modelAnalise).Execute();

            return modelRetorno;
        }

        #endregion

        #region Obter Pagina Publica
        /// <summary>
        /// Obtém pagina de visualizacao publica
        /// </summary>
        /// <param name="url"></param>
        /// <param name="portal"></param>
        /// <returns></returns>
        public static MLPaginaPublico ObterPaginaPublica(string url, MLPortal portal)
        {
            var cacheKey = string.Format("portal_{0}_page_{1}", portal.Codigo, url);
            var cachedValue = BLCachePortal.Get<MLPaginaPublico>(cacheKey);

            if (cachedValue == null)
            {
                using (var command = Database.NewCommand("USP_CMS_S_PAGINA_PUBLICO", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PAG_C_URL", SqlDbType.VarChar, 100, url);
                    command.NewCriteriaParameter("@POR_N_CODIGo", SqlDbType.Decimal, 18, portal.Codigo);

                    command.NewOutputParameter("@OUT_PAG_N_CODIGO", SqlDbType.Decimal);
                    command.NewOutputParameter("@OUT_PAG_C_NOME", SqlDbType.VarChar, 100);
                    command.NewOutputParameter("@OUT_PAG_C_URL", SqlDbType.VarChar, 100);
                    command.NewOutputParameter("@OUT_PAG_SEC_N_CODIGO", SqlDbType.Decimal);

                    command.NewOutputParameter("@OUT_PAG_IDI_N_CODIGO", SqlDbType.Decimal);
                    command.NewOutputParameter("@OUT_IDI_C_SIGLA", SqlDbType.VarChar, 5);
                    command.NewOutputParameter("@OUT_IDI_C_NOME", SqlDbType.VarChar, 50);
                    command.NewOutputParameter("@OUT_IDI_B_ATIVO", SqlDbType.Bit);
                    command.NewOutputParameter("@OUT_PAG_B_HTTPS", SqlDbType.Bit);

                    command.NewOutputParameter("@OUT_PAG_B_RESTRITO", SqlDbType.Bit);
                    command.NewOutputParameter("@OUT_PAG_C_TITULO", SqlDbType.VarChar, 100);
                    command.NewOutputParameter("@OUT_PAG_C_DESCRICAO", SqlDbType.VarChar, 250);
                    command.NewOutputParameter("@OUT_PAG_C_TAGS", SqlDbType.VarChar, 250);
                    command.NewOutputParameter("@OUT_PAG_C_NOME_LAYOUT", SqlDbType.VarChar, 100);
                    command.NewOutputParameter("@OUT_PAG_C_NOME_TEMPLATE", SqlDbType.VarChar, 100);
                    command.NewOutputParameter("@OUT_PAG_C_TEMPLATE", SqlDbType.VarChar, -1);
                    command.NewOutputParameter("@OUT_PAG_C_SCRIPT", SqlDbType.VarChar, -1);
                    command.NewOutputParameter("@OUT_PAG_C_CSS", SqlDbType.VarChar, -1);
                    command.NewOutputParameter("@OUT_PAG_C_URL_LOGIN", SqlDbType.VarChar, 500);
                    command.NewOutputParameter("@OUT_IDIOMAS", SqlDbType.VarChar, -1);

                    //command.NewOutputParameter("@OUT_LAY_C_IDIOMA", SqlDbType.VarChar, 5);

                    // Execucao
                    Database.ExecuteNonQuery(command);

                    var pagina = Database.FillModel<MLPaginaPublico>(command);

                    BLCachePortal.Add(portal.Codigo.Value, cacheKey, pagina);

                    return pagina;
                }
            }

            return cachedValue;
        }
        #endregion

        #region Excluir Modulos

        /// <summary>
        /// Excluir todos as referências de módulos em uma página
        /// </summary>
        public bool ExcluirModulosPublicados(decimal id)
        {
            using (var command = Database.NewCommand("USP_CMS_D_PAGINA_PUB_MODULOS", CONNECTION_STRING))
            {
                // Parametros
                command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, id);

                command.ExecuteNonQuery();
            }

            return true;
        }

        public bool ExcluirModulosEdicao(decimal id)
        {
            using (var command = Database.NewCommand("USP_CMS_D_PAGINA_EDI_MODULOS", CONNECTION_STRING))
            {
                // Parametros
                command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, id);

                command.ExecuteNonQuery();
            }

            return true;
        }

        #endregion

        #region ListarPaginasPublicadas

        public static List<MLPagina> ListarPaginasPublicadas(string buscaGenerica, decimal? codigoPortal, string gruposUsuario, string connectionString, int? top)
        {
            return ListarPaginasPublicadas(buscaGenerica, codigoPortal, null, gruposUsuario, connectionString, top);
        }

        public static List<MLPagina> ListarPaginasPublicadas(string buscaGenerica, decimal? codigoPortal, decimal? codigoIdioma, string gruposUsuario, string connectionString, int? top)
        {
            try
            {
                using (var command = Database.NewCommand("USP_CMS_L_PAGINAS_PUBLICADAS", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, -1, buscaGenerica);
                    command.NewCriteriaParameter("@GRUPOS_USUARIO", SqlDbType.VarChar, -1, gruposUsuario);
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, codigoPortal);
                    command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);
                    command.NewCriteriaParameter("@TOP", SqlDbType.Int, top);

                    // Execucao
                    return Database.ExecuteReader<MLPagina>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        // PERMISSOES

        #region CheckPermissaoAdmin

        /// <summary>
        /// Verifica se o usuário tem permissões administrativas para gerenciar a página
        /// </summary>
        public static bool CheckPermissaoAdmin(Permissao permissao, decimal codigoUsuario, decimal codigoPagina)
        {

            

            return false;
        }


        #endregion

        #region ObterPermissoes

        /// <summary>
        /// Retorna um objeto com todas as permissoes definidas para a página
        /// </summary>
        /// <param name="codigoPagina"></param>
        /// <returns></returns>
        public MLPaginaPermissaoCompleta ObterPermissoes(decimal id)
        {
            try
            {
                var model = new MLPaginaPermissaoCompleta();

                using (var command = Database.NewCommand("USP_CMS_S_PAGINA_PERMISSAO", CONNECTION_STRING))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, id);

                    command.NewOutputParameter("@OUT_PAG_B_RESTRITO", SqlDbType.Bit);
                    command.NewOutputParameter("@OUT_SEC_B_RESTRITO", SqlDbType.Bit);
                    command.NewOutputParameter("@OUT_POR_B_RESTRITO", SqlDbType.Bit);

                    // Execucao
                    var dataSet = Database.ExecuteDataSet(command);

                    model.CodigoPagina = id;
                    if (command.Parameters["@OUT_PAG_B_RESTRITO"].Value != DBNull.Value) model.PaginaRestrita = Convert.ToBoolean(command.Parameters["@OUT_PAG_B_RESTRITO"].Value);
                    if (command.Parameters["@OUT_SEC_B_RESTRITO"].Value != DBNull.Value) model.SecaoRestrita = Convert.ToBoolean(command.Parameters["@OUT_SEC_B_RESTRITO"].Value);
                    if (command.Parameters["@OUT_POR_B_RESTRITO"].Value != DBNull.Value) model.PortalRestrito = Convert.ToBoolean(command.Parameters["@OUT_POR_B_RESTRITO"].Value);

                    // Grupos Pagina
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        model.GruposPagina  = Database.FillList<MLPaginaPermissao>(dataSet.Tables[0]);

                    if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                        model.GruposSecao = Database.FillList<MLSecaoPermissao>(dataSet.Tables[1]);
                    
                }

                return model;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion


        // HISTORICO

        #region GerarHistorico

        /// <summary>
        /// Excluir todos as referências de módulos em uma página
        /// </summary>
        public Guid? GerarHistorico(decimal id)
        {
            try
            {
                var paginaPublicada = CRUD.Obter<MLPaginaPublicada>(id, CONNECTION_STRING);
                var modulosPublicados = CRUD.Listar<MLPaginaModuloPublicado>(new MLPaginaModuloPublicado { CodigoPagina = id }, CONNECTION_STRING);

                if (paginaPublicada == null || !paginaPublicada.Codigo.HasValue) return null;

                var guid = Guid.NewGuid();

                var historico = new MLPaginaHistorico 
                {
                    CodigoHistorico = guid,
                    DataHistorico = DateTime.Now,

                    ApresentarNaBusca = paginaPublicada.ApresentarNaBusca,
                    Codigo = paginaPublicada.Codigo,                    
                    NomeLayout = paginaPublicada.NomeLayout,                    
                    NomeTemplate = paginaPublicada.NomeTemplate,
                    DataPublicacao = paginaPublicada.DataEdicao,
                    Descricao = paginaPublicada.Descricao,
                    Tags = paginaPublicada.Tags,
                    TemplateCustomizado = paginaPublicada.TemplateCustomizado,
                    Scripts = paginaPublicada.Scripts,
                    Css = paginaPublicada.Css,
                    Titulo = paginaPublicada.Titulo,
                    UsuarioPublicador = paginaPublicada.UsuarioEditor
                };

                CRUD.Salvar(historico, CONNECTION_STRING);

                foreach (var modulo in modulosPublicados)
                {
                    CRUD.Salvar(new MLPaginaModuloHistorico
                    {
                        CodigoHistorico = guid,

                        CodigoModulo = modulo.CodigoModulo,
                        CodigoPagina = modulo.CodigoPagina,
                        Repositorio = modulo.Repositorio
                    }, CONNECTION_STRING);
                }

                return guid;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Obter Pagina Histórico

        /// <summary>
        /// Obtem a página completa para ser utilizada na área administrativa
        /// </summary>
        public MLPaginaHistorico ObterPaginaHistorico(Guid codigoHistorico)
        {
            var model = new MLPaginaHistorico();

            using (var command = Database.NewCommand("USP_CMS_S_PAGINA_HISTORICO_ADMIN", CONNECTION_STRING))
            {
                // Parametros
                command.NewCriteriaParameter("@HIS_GUID", SqlDbType.UniqueIdentifier, 18, codigoHistorico);

                // Execucao
                var dataSet = Database.ExecuteDataSet(command);

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    // Pagina
                    model = Database.FillModel<MLPaginaHistorico>(dataSet.Tables[0].Rows[0]);

                   // Modulos da Pagina em Edicao
                    if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[1].Rows)
                        {
                            model.Modulos.Add(Database.FillModel<MLPaginaModuloHistorico>(row));
                        }
                    }
                }
            }

            return model;
        }

        #endregion

        // PUBLICO

        #region Obter Pagina Publico



        #endregion

        // IDIOMA

        #region ListarPaginasRelacionadas

        /// <summary>
        /// Listar páginas publicas, relacionadas pelo código pai
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ListarPaginasRelacionadas(decimal? codigoPagina, string urlDetalhe)
        {
            var retorno = new Dictionary<string, string>();

            try
            {
                var portal = BLPortal.Atual;

                using (var command = Database.NewCommand("USP_MOD_IDI_L_PAGINAS", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, codigoPagina);

                    if (!string.IsNullOrWhiteSpace(urlDetalhe))
                    {
                        command.NewCriteriaParameter("@URL", SqlDbType.VarChar, 250, urlDetalhe);
                    }
                    
                    // Execucao
                    using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                    {
                        while (reader.Read())
                        {
                            string key = null, value = null;

                            if (!reader.IsDBNull(0))
                            {
                                value = reader.GetString(0);
                            }

                            key = reader.GetString(1);

                            if (!retorno.ContainsKey(key))
                            {
                                retorno.Add(key, value);
                            }
                        }

                        Database.CloseReader(reader);
                        Database.CloseConnection(command);

                        return retorno;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        public static MLPaginaPublico Atual
        {
            get
            {
                var obj = CallContext.GetData("pagina-atual");

                if (obj != null)
                {
                    return obj as MLPaginaPublico;
                }

                return new MLPaginaPublico();
            }
            set
            {
                CallContext.SetData("pagina-atual", value);
            }
        }
    }
}
