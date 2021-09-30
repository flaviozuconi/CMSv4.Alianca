using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Framework.DataLayer;

namespace Framework.Utilities
{
    /// <summary>
    /// Classe MENU
    /// </summary>
    public class BLMenu
    {
        #region Excluir

        /// <summary>
        /// Deletar item, itens filho e reoordenar.
        /// </summary>
        /// <param name="decCodigo"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static decimal Excluir(decimal decCodigo, string connectionString)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_D_MENU_ITEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_N_CODIGO", SqlDbType.Decimal, 18, decCodigo);

                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Inserir

        /// <summary>
        /// Criar um novo item no menu especificado.
        /// </summary>
        /// <param name="objML"></param>
        /// <param name="connectionString"></param>
        /// <returns>decimal</returns>
        public static decimal Inserir(MLMenuItem objML, string connectionString)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_I_MENU_ITEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_MEN_N_CODIGO", SqlDbType.Decimal, 18, objML.CodigoMenu);
                    command.NewCriteriaParameter("@MNI_N_CODIGO_PAI", SqlDbType.Decimal, 18, objML.CodigoPai);
                    command.NewCriteriaParameter("@MNI_C_NOME", SqlDbType.VarChar, 100, objML.Nome);
                    command.NewCriteriaParameter("@MNI_B_STATUS", SqlDbType.Bit, objML.Ativo);
                    command.NewCriteriaParameter("@MNI_B_NOVA_PAGINA", SqlDbType.Bit, objML.AbrirNovaPagina);

                    // Execucao
                    return (decimal)Database.ExecuteScalar(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Obter Completo

        public static MLMenuCompleto ObterCompleto(decimal pdecCodigoMenu, decimal CodigoPortal, decimal CodigoUsuario)
        {
            return ObterCompleto(pdecCodigoMenu, true, true, true, CodigoPortal, CodigoUsuario);
        }

        /// <summary>
        /// Obtem um objeto completo do MENU e de seus itens
        /// </summary>
        /// <param name="useCache">Indica se o menu deve ser buscado primeiro no cache</param>
        public static MLMenuCompleto ObterCompleto(decimal pdecCodigoMenu, bool useCache, bool bItensPai, bool? Status, decimal? CodigoPortal, decimal? CodigoUsuario)
        {
            MLMenuCompleto retorno = null;
            var objItem = new MLMenuItem(); ;

            try
            {
                // Procura no cache caso esteja configurado, e escolhido pelo parametro
                if (useCache && HttpContext.Current.Request.QueryString["reset"] == null) retorno = BLCachePortal.Get<MLMenuCompleto>(MLMenu.Config.CacheKey + pdecCodigoMenu + "_" + CodigoPortal);

                if (retorno == null)
                {
                    // Senao encontrou, buscar na base de dados

                    using (var command = Database.NewCommand("USP_FWK_S_MENU_COMPLETO"))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@MEN_N_CODIGO", SqlDbType.Decimal, 18, pdecCodigoMenu);
                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, CodigoPortal);
                        command.NewCriteriaParameter("@USU_N_CODIGO", SqlDbType.Decimal, 18, CodigoUsuario);
                        command.NewCriteriaParameter("@MNI_B_STATUS", SqlDbType.Bit, Status);

                        // Execucao
                        var dataset = Database.ExecuteDataSet(command);
                        retorno = new MLMenuCompleto();

                        if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                        {
                            // Preenche dados do menu
                            retorno = Database.FillModel<MLMenuCompleto>(dataset.Tables[0].Rows[0]);

                            if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                            {
                                // Preenche itens do menu
                                List<MLMenuItem> ItensMenuAux = Database.FillList<MLMenuItem>(dataset.Tables[1]);
                                List<MLMenuItem> ItensMenu = new List<MLMenuItem>();

                                foreach (var item in ItensMenuAux)
                                    ItensMenu.Add(item);

                                if (bItensPai) ItensMenu.RemoveAll(item => item.CodigoPai != 0);

                                foreach (var item in ItensMenu)
                                    EncontrarFilhos(item, ItensMenuAux);

                                retorno.ItensMenu = ItensMenu;
                            }
                        }
                    }

                    if (useCache)
                        BLCachePortal.Add(MLMenu.Config.CacheKey + pdecCodigoMenu, retorno);
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

        #region EncontrarFilhos

        private static void EncontrarFilhos(MLMenuItem model, List<MLMenuItem> itens)
        {
            List<MLMenuItem> lstFilhos = itens.FindAll(m => m.CodigoPai == model.Codigo);

            if (lstFilhos.Count > 0)
            {
                foreach (var item in lstFilhos)
                {
                    EncontrarFilhos(item, itens);
                    model.Filhos.Add(item);
                }
            }
        }

        #endregion

        #region Ordenar

        public static bool Ordenar(decimal decCodigoSource, decimal decCodigoTarger, int intIndex, string connectionString)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_U_MENU_ORDEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_N_CODIGO_SOURCE", SqlDbType.Decimal, 18, decCodigoSource);
                    command.NewCriteriaParameter("@MNI_N_CODIGO_TARGET", SqlDbType.Decimal, 18, decCodigoTarger);
                    command.NewCriteriaParameter("@MNI_N_INDEX", SqlDbType.Int, intIndex);

                    // Execucao
                    Database.ExecuteNonQuery(command);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }

        #endregion
    }
}
