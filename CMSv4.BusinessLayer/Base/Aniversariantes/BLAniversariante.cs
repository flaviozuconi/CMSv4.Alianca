using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Aniversariantes
    /// </summary>
    public class BLAniversariante
    {
        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLColaboradorPublico> Listar(MLModuloAniversariante filtro, int? mes = null, decimal? codigoPagina = null)
        {
            using (var command = Database.NewCommand("USP_MOD_ANI_L_LISTAR", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@ANI_N_QTDE", SqlDbType.Int, filtro.QuantidadeDestaque ?? 0);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, BLPortal.Atual.Codigo);

                if (codigoPagina.HasValue)
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, codigoPagina.Value);

                if (filtro.ListarMes.HasValue)
                {
                    command.NewCriteriaParameter("@ANI_B_LISTAR_MES", SqlDbType.Bit, filtro.ListarMes.Value);

                    if (filtro.ListarMes.Value)
                        command.NewCriteriaParameter("@ANI_N_MES", SqlDbType.Int, mes);
                }

                if (filtro.Restrito.HasValue)
                {
                    command.NewCriteriaParameter("@ANI_B_RESTRITO", SqlDbType.Bit, filtro.Restrito.Value);
                    command.NewCriteriaParameter("@ANI_C_GRUPOS", SqlDbType.VarChar, -1, filtro.Grupos);
                }

                if (filtro.DataInicio.HasValue)
                    command.NewCriteriaParameter("@ANI_D_INICIO", SqlDbType.DateTime, filtro.DataInicio.Value);

                if (filtro.DataTermino.HasValue)
                    command.NewCriteriaParameter("@ANI_D_TERMINO", SqlDbType.DateTime, filtro.DataTermino.Value);

                // Execucao
                return Database.ExecuteReader<MLColaboradorPublico>(command);
            }
        }

        #endregion
    }
}
