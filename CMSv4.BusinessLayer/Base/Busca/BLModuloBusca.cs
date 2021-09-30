using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Modulo de Busca
    /// </summary>
    public class BLModuloBusca
    {
        #region ListarPublico
        public static List<MLBuscaResultado> ListarPublico(int intQtde, int decPagina, string criterio)
        {
            var portal = BLPortal.Atual;
            List<MLBuscaResultado> retorno = new List<MLBuscaResultado>();

            using (var command = Database.NewCommand("USP_MOD_BUS_L_BUSCAR", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, 500, criterio);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, decPagina);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intQtde);

                var dataset = Database.ExecuteDataSet(command);

                if (dataset != null)
                {
                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        //Resultado de busca
                        retorno = Database.FillList<MLBuscaResultado>(dataset.Tables[0]);
                    }
                    if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    {
                        //Assuntos relacionados
                        var lstassuntos = Database.FillList<MLAssuntosBusca>(dataset.Tables[1]);

                        foreach (var item in retorno)
                        {
                            var assuntos = new List<MLAssuntosBusca>();

                            assuntos = lstassuntos.FindAll(o => o.CodigoPagina == item.Codigo);

                            foreach (var item2 in assuntos)
                            {
                                item.LstAssuntos.Add(item2);
                            }
                        }
                    }
                }
            }

            return retorno;
        }

        public static List<string> ListarPublico(string termo)
        {
            var lista = new List<string>();
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_BUS_L_AUTOCOMPLETE", portal.ConnectionString))
            {
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@TERMO", SqlDbType.VarChar, 200, termo);

                using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        lista.Add(Convert.ToString(reader.GetValue(0)));
                    }
                }

                Database.CloseConnection(command);

                return lista;
            }
        }
        #endregion

        #region ListarPublico
        public static List<MLBuscaResultado> ListarResumo(int intQtde, string criterio)
        {
            var portal = BLPortal.Atual;
            List<MLBuscaResultado> retorno = new List<MLBuscaResultado>();

            using (var command = Database.NewCommand("USP_MOD_BUS_L_BUSCAR_RESUMO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);
                command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, 500, criterio);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intQtde);

                var dataset = Database.ExecuteDataSet(command);

                if (dataset != null)
                {
                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        //Resultado de busca
                        retorno = Database.FillList<MLBuscaResultado>(dataset.Tables[0]);
                    }
                    if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    {
                        //Assuntos relacionados
                        var lstassuntos = Database.FillList<MLAssuntosBusca>(dataset.Tables[1]);

                        foreach (var item in retorno)
                        {
                            var assuntos = new List<MLAssuntosBusca>();

                            assuntos = lstassuntos.FindAll(o => o.CodigoPagina == item.Codigo);

                            foreach (var item2 in assuntos)
                            {
                                item.LstAssuntos.Add(item2);
                            }
                        }
                    }
                }
                //// Execucao
                //return Database.ExecuteReader<MLBuscaResultado>(command);
            }
            return retorno;
        }
        #endregion

        #region SanitizeInput
        private static string SanitizeInput(string busca)
        {
            try
            {
                if (string.IsNullOrEmpty(busca))
                    return busca;

                busca = busca.ToLower().Replace("and", "").Replace("between", "").Replace("or", "");

                string newValue = "";

                foreach (var item in busca.Split(' '))
                {
                    if (item.Length > 2)
                    {
                        if (string.IsNullOrEmpty(newValue))
                            newValue = "\"" + item;
                        else
                        {
                            newValue = newValue + "*\" AND \"" + item;

                        }
                    }
                }

                if (!string.IsNullOrEmpty(newValue))
                    newValue = newValue + "*\"";

                return newValue;
            }
            catch
            {
                return "\"" + busca + "*\"";
            }
        }
        #endregion

        #region ObterUrlResultadoBusca
        public static string ObterUrlResultadoBusca()
        {
            var portal = BLPortal.Atual;
            var idioma = BLIdioma.CodigoAtual;

            var cacheKey = string.Format("portal_{0}_resultado_busca_{1}", portal.Codigo, idioma);
            var cachedValue = BLCachePortal.Get<string>(cacheKey);

            if (string.IsNullOrEmpty(cachedValue))
            {
                using (var command = Database.NewCommand("USP_MOD_BUS_S_URL_RESULTADO_BUSCA", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                    command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, idioma);

                    // Execucao
                    cachedValue = Convert.ToString(Database.ExecuteScalar(command));
                }

                BLCachePortal.Add(portal.Codigo.GetValueOrDefault(), cacheKey, cachedValue);
            }

            return cachedValue;
        }
        #endregion

        #region ObterUrl
        public static string ObterUrl(decimal idModulo)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_BUS_S_URL_MODULO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@MOD_N_CODIGO", SqlDbType.Decimal, 18, idModulo);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, 18, BLIdioma.CodigoAtual);

                // Execucao
                return Convert.ToString(Database.ExecuteScalar(command));
            }
        }
        #endregion
    }
}
