using Framework.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities.Utilitario
{
    public class BLAuditoria
    {
        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLAuditoriaRelatorio> Listar(decimal? codigoPortal)
        {
            try
            {
                using (var command = Database.NewCommand("USP_AUD_L_LISTAR"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@AUD_POR_N_CODIGO", SqlDbType.Decimal, codigoPortal);

                    // Execucao
                    return Database.ExecuteReader<MLAuditoriaRelatorio>(command);
                }
            }
            catch
            {
                return new List<MLAuditoriaRelatorio>();
            }
        }

        #endregion
    }
}
