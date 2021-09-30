using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Modulo de Enquete
    /// </summary>
    public class BLModuloEnquete : BLCRUD<MLEnquete>
    {
        #region Salvar
        /// <summary>
        /// Salvar Completo com validações
        /// </summary>
        /// <param name="model"></param>
        /// <param name="OpcaoCodigo"></param>
        /// <param name="OpcaoTitulo"></param>
        /// <param name="portal"></param>
        /// <returns></returns>
        public MLEnquete Salvar(MLEnquete model, string OpcaoCodigo, string OpcaoTitulo, MLPortal portal)
        {
            var lstOpcaoCodigo = OpcaoCodigo.Split(',');
            var lstOpcaoTitulo = OpcaoTitulo.Split(',');

            if (model.Codigo.HasValue)
            {
                var modelcad = this.Obter(new MLEnquete { Codigo = model.Codigo.GetValueOrDefault(), CodigoPortal = portal.Codigo }, portal.ConnectionString);

                if (modelcad != null)
                {
                    model.CodigoPortal = modelcad.CodigoPortal;
                }
                else
                {
                    model.Codigo = null;
                    return model;
                }
                    
            }
            else
            {
                model.CodigoPortal = portal.Codigo;
            }

            model.Ativo = model.Ativo.GetValueOrDefault(false);

            var codigo = Salvar(model, portal.ConnectionString);

            if (lstOpcaoTitulo.Length > 0)
            {
                decimal? codigoOpcao = null;
                for (var i = 0; i < OpcaoTitulo.Length; i++)
                {
                    var opcao = new MLEnqueteOpcao();
                    opcao.Codigo = model.Codigo.HasValue ? Convert.ToDecimal(OpcaoCodigo[i]) : codigoOpcao;
                    opcao.CodigoEnquete = codigo;
                    opcao.Titulo = lstOpcaoTitulo[i];
                    opcao.Ordem = i;
                    new BLModuloEnqueteOpcao().Salvar(opcao, portal.ConnectionString);
                }
            }
            model.Codigo = codigo;
            return model;
        }

        #endregion

        #region Listar

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLEnquetePublico> Listar(decimal?codigoenquete,bool abertas)
        {
            using (var command = Database.NewCommand("USP_MOD_ENQ_L_ENQUETE_PUBLICO", BLPortal.Atual.ConnectionString))
            {
                // Parametros                                
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@ENQ_N_CODIGO", SqlDbType.Decimal, 18, codigoenquete);
                command.NewCriteriaParameter("@ABERTAS", SqlDbType.Bit, abertas);
                // Execucao
                return Database.ExecuteReader<MLEnquetePublico>(command);
            }
        }

        #endregion

        #region ListarResultado

        /// <summary>
        /// LISTAR RESULTADO
        /// </summary>
        public static MLEnqueteResultado ListarResultado(decimal codigoenquete, decimal? decCodigoUsuario, string strIP, bool IsCache)
        {
            var retorno = new MLEnqueteResultado();

            string strChaveCache = String.Format("modEnquete_codEnquete-{0}-codCli-{1}-ip-{2}", codigoenquete, decCodigoUsuario, strIP);

            if (IsCache)
            {
                var cacheRetorno = BLCachePortal.Get<MLEnqueteResultado>(strChaveCache);
                if (cacheRetorno != null) return cacheRetorno;
            }
            else
                BLCachePortal.Remove(strChaveCache);

            using (var command = Database.NewCommand("USP_MOD_ENQ_L_ENQUETE_RESULTADO", BLPortal.Atual.ConnectionString))
            {                
                // Parametros                                
                command.NewCriteriaParameter("@ENQ_N_CODIGO", SqlDbType.Decimal,18, codigoenquete);
                command.NewCriteriaParameter("@CLI_N_CODIGO", SqlDbType.Decimal, 18, decCodigoUsuario);
                command.NewCriteriaParameter("@IP", SqlDbType.VarChar, 100, strIP);

                // Execucao
                var dataset = Database.ExecuteDataSet(command);

                if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    retorno = Database.FillModel<MLEnqueteResultado>(dataset.Tables[0].Rows[0]);

                if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    retorno.Resultados = Database.FillList<MLEnqueteOpcaoResultado>(dataset.Tables[1]);

                if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0 && dataset.Tables[2].Rows[0].ItemArray.Length > 0)
                    retorno.IsVotou = (dataset.Tables[2].Rows[0] != null && Convert.ToDecimal(dataset.Tables[2].Rows[0].ItemArray[0]) > 0);

                if (retorno.Resultados.Count > 0)
                    retorno.Total = retorno.Resultados[0].TotalEnquete;

                BLCachePortal.Add(strChaveCache, retorno);
                return retorno;
            }
        }

        #endregion

        #region ListarRelatorio

        /// <summary>
        /// LISTAR RELATORIO
        /// </summary>
        public static List<MLEnqueteVotoRelatorio> ListarRelatorio(decimal codigoenquete)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_ENQ_L_ENQUETE_RELATORIO", BLPortal.Atual.ConnectionString))
                {
                    // Parametros                                
                    command.NewCriteriaParameter("@ENQ_N_CODIGO", SqlDbType.Decimal, 18, codigoenquete);
                    // Execucao
                    return Database.ExecuteReader<MLEnqueteVotoRelatorio>(command);
                    
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }            
            
        }

        #endregion

        #region ObterMaisRecente

        /// <summary>
        /// OBTER MAIS RECENTE
        /// </summary>
        public static MLEnqueteResultado ObterMaisRecente(decimal codigoPortal)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_ENQ_S_ENQUETE_MAIS_RECENTE", BLPortal.Atual.ConnectionString))
                {
                    // Parametros                                
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, codigoPortal);
                    
                    // Execucao
                    DataSet ds = Database.ExecuteDataSet(command);

                    if(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        return Database.FillModel<MLEnqueteResultado>(ds.Tables[0].Rows[0]);
                    return new MLEnqueteResultado();

                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }

        #endregion

        #region UltimaPublicada

        /// <summary>
        /// ULTIMA PUBLICADA
        /// </summary>
        public static MLEnqueteResultado UltimaPublicada(decimal codigoPortal)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_ENQ_S_ENQUETE_ULTIMA_PUBLICADA", BLPortal.Atual.ConnectionString))
                {
                    // Parametros                                
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, codigoPortal);

                    // Execucao
                    DataSet ds = Database.ExecuteDataSet(command);
                    return Database.FillModel<MLEnqueteResultado>(ds.Tables[0].Rows[0]);

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

    public class BLModuloEnqueteOpcao : BLCRUD<MLEnqueteOpcao>
    {

        public override decimal Salvar(MLEnqueteOpcao model, string connectionString = "")
        {

            model.Ordem = 999;
            return base.Salvar(model, connectionString);
        }

    }
}