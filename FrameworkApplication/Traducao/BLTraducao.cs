using Framework.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace Framework.Utilities
{
    public class BLTraducao
    {
        private string CONNECTION_STRING = "";
        private decimal? CODIGO_PORTAL = 0;

        public BLTraducao()
        {
            
        }

        public BLTraducao(MLPortal portal)
        {
            if (portal == null) return;

            CODIGO_PORTAL = portal.Codigo;
            CONNECTION_STRING = portal.ConnectionString;
        }

        /// <summary>
        /// Tradução de Termos
        /// </summary>
        /// <param name="texto">Texto original</param>
        /// <returns>Termo traduzido</returns>
        public static string T(string texto)        
        {            
            var T = new BLTraducao(BLPortal.Atual);
            return T.Obter(texto);
        }

   

        #region CarregarDicionario

        /// <summary>
        /// Carrega o dicionário de traduções
        /// </summary>
        /// <returns></returns>
        public List<MLDicionario> CarregarDicionario()
        {
            try
            {
                if (!CODIGO_PORTAL.HasValue) return null;
                //if (string.IsNullOrEmpty(CONNECTION_STRING)) return null;

                using (var command = Database.NewCommand("USP_CMS_L_DICIONARIO_TRADUCAO", CONNECTION_STRING))
                {
                    // Parametros
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, CODIGO_PORTAL);

                    // Execucao
                    return Database.ExecuteReader<MLDicionario>(command);
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
        /// Obter
        /// </summary>
        /// <returns></returns>
        public string Obter(string idioma, string texto)
        {
            try
            {
                /*if (string.IsNullOrEmpty(idioma) || !CODIGO_PORTAL.HasValue || string.IsNullOrEmpty(CONNECTION_STRING))
                {
                    return texto;
                }*/
                if (string.IsNullOrEmpty(idioma) || !CODIGO_PORTAL.HasValue)
                    return texto;

                var cacheKey = string.Format("portal_{0}_dicionario_traducao", CODIGO_PORTAL);
                List<MLDicionario> dicionario = null;

                if (BLConfiguracao.Obter<bool>("CMS.Traducao.Cache.Habilitar", false))
                {
                    dicionario = BLCachePortal.Get<List<MLDicionario>>(cacheKey);

                    if (dicionario == null)
                    {
                        dicionario = CarregarDicionario();
                        BLCachePortal.Add(CODIGO_PORTAL.Value, cacheKey, dicionario);
                    }
                }
                else
                {
                    dicionario = CarregarDicionario();
                }

                if (dicionario == null)
                    dicionario = new List<MLDicionario>();

                var traducao = dicionario.Find(o =>
                    o.Termo.Equals(texto, StringComparison.InvariantCultureIgnoreCase)
                    && !string.IsNullOrEmpty(o.Idioma) && o.Idioma.Equals(idioma, StringComparison.InvariantCultureIgnoreCase));

                if (traducao != null)
                    return (traducao.Traducao ?? texto); //Exibe o próprio texto caso a tradução retorne nulo
                else
                {
                    using (var command = Database.NewCommand("USP_CMS_I_TERMO_TRADUCAO", CONNECTION_STRING))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@TER_C_TERMO", SqlDbType.VarChar, -1, texto);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, CODIGO_PORTAL);

                        // Execucao
                        Database.ExecuteNonQuery(command);
                    }

                    return texto;
                }
            }
            catch 
            {
                return texto;
            }
        }

        public string Obter(string texto)
        {
            //return Obter(Thread.CurrentThread.CurrentCulture.Name, texto);
            return Obter( BLIdioma.Atual.Sigla, texto);
        }

        public string ObterAdm(string texto)
        {
            return Obter(BLIdioma.AtualAdm.Sigla, texto);
        }

        #endregion

        #region Listar
        public static List<Dictionary<string, object>> Listar(MLPortal portal, string filtro, int pagina, int quantidade, int orderBy, out double total,bool adm)
        {            
            var retorno = new List<Dictionary<string, object>>();

            total = 0;

            using (var command = Database.NewCommand("USP_CMS_L_TRADUCAO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, adm ? 0 : portal.Codigo);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, quantidade);
                command.NewCriteriaParameter("@FILTRO", SqlDbType.VarChar, -1, filtro);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, pagina);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, orderBy);

                using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        if (total == 0)
                        {
                            try
                            {
                                total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTAL_ROWS")));
                            }
                            catch { }
                        }

                        retorno.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(i => reader.GetName(i), i => reader.GetValue(i)));
                    }

                    Database.CloseReader(reader);
                    Database.CloseConnection(command);
                }

                // Execucao
                return retorno;
            }
        }
        #endregion

        #region Atualizar
        public static void Atualizar(decimal codigo, string sigla, string traducao)
        {
            using (var command = Database.NewCommand("USP_CMS_I_TRADUCAO"))
            {
                // Parametros
                command.NewCriteriaParameter("@CODIGO", SqlDbType.Decimal, 18, codigo);
                command.NewCriteriaParameter("@SIGLA", SqlDbType.VarChar, 5, sigla);
                command.NewCriteriaParameter("@TRADUCAO", SqlDbType.VarChar, -1, traducao);

                // Execucao
                Database.ExecuteScalar(command);
            }
        }
        #endregion

        #region ListarPendentes
        public static List<MLTermo> ListarPendentes(decimal portal,string idioma)
        {
            using (var command = Database.NewCommand("USP_CMS_L_TERMOS_PENDENTES"))
            {
                // Parametros
                command.NewCriteriaParameter("@PORTAL", SqlDbType.Decimal, 18, portal);
                command.NewCriteriaParameter("@IDIOMA", SqlDbType.VarChar, 5, idioma);                

                // Execucao
                return Database.ExecuteReader<MLTermo>(command);
            }
        }
        #endregion

        public string MSG_CODIGO_INVALIDO = "Código inválido.";
        public string MSG_PERMISSAO_NEGADA = "Você não tem permissão para acessar este recurso.";
    }
}
