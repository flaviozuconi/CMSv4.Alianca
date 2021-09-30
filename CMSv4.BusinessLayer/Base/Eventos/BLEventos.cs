using System;
using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLEventos
    {
        #region ListarDestaque

        /// <summary>
        /// LISTAR DESTAQUE ATIVOS
        /// </summary>
        /// <param name="intTop">Quantidade de Registros</param>
        /// <param name="IsCache">Define se deve utilizar o cache</param>
        public static List<MLEvento> ListarDestaque(int intTop, bool IsCache)
        {
            var portal = BLPortal.Atual;
            var idioma = BLIdioma.Atual;
            var cacheKey = string.Format("portal_{0}_evento_destaque_top_{1}_idioma_{2}", portal.Codigo, intTop, idioma.Codigo);
            var cachedValue = BLCachePortal.Get<List<MLEvento>>(cacheKey);

            if (cachedValue != null && IsCache)
                return cachedValue;
            
            using (var command = Database.NewCommand("USP_MOD_EVE_L_EVENTOS_DESTAQUE", portal.ConnectionString))
            {
                // Parametros                        
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, portal.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Int, idioma.Codigo);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, intTop);

                // Execucao

                var lstRetorno = Database.ExecuteReader<MLEvento>(command);
                BLCachePortal.Add(portal.Codigo.Value, cacheKey, lstRetorno, 1);

                return lstRetorno;
            }
        }

        #endregion

        #region ListarDiasCalendario

        public static List<MLEventoPublico> ListarDiasCalendario(int mes, int ano, int idioma)
        {
            using (var command = Database.NewCommand("USP_MOD_EVE_L_EVENTOS_MES", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Int, idioma);
                command.NewCriteriaParameter("@MES", SqlDbType.VarChar, 2, mes);
                command.NewCriteriaParameter("@ANO", SqlDbType.VarChar, 4, ano);

                // Execucao
                return Database.ExecuteReader<MLEventoPublico>(command);
            }
        }

        #endregion

        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLEventoPublico> ListarPublico(MLModuloEventos model,int idioma, bool eventosPassados = false, bool usarPaginacao = true)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_EVE_L_EVENTOS_PUBLICO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Int, idioma);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, model.Pagina);
                command.NewCriteriaParameter("@EVENTOS_PASSADOS", SqlDbType.Bit, eventosPassados);
                command.NewCriteriaParameter("@USAR_PAGINACAO", SqlDbType.Bit, usarPaginacao);

                // Execucao
                return Database.ExecuteReader<MLEventoPublico>(command);
            }
        }

        #endregion

        #region ListarPublicoComFiltro

        /// <summary>
        /// LISTAR PUBLICO COM FILTRO
        /// </summary>
        public static List<MLEventoPublico> ListarPublicoComFiltro(int? ano, int? semestre)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_EVE_L_EVENTOS_LISTAGEM_FILTRO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int,  BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Int, BLIdioma.Atual.Codigo);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, ano);
                command.NewCriteriaParameter("@SEMESTRE", SqlDbType.Int, semestre > 0 ? semestre : null);

                // Execucao
                return Database.ExecuteReader<MLEventoPublico>(command);
            }
        }

        #endregion

        #region ListarDiaPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLEventoPublico> ListarDiaPublico(DateTime dataEventoDia,int codigoidioma)
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_EVE_L_EVENTOS_DIA_PUBLICO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Int, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Int, codigoidioma);
                command.NewCriteriaParameter("@EVE_D_EVENTOS_DIA", SqlDbType.DateTime, dataEventoDia);

                // Execucao
                return Database.ExecuteReader<MLEventoPublico>(command);
            }
        }

        #endregion
    }
}
