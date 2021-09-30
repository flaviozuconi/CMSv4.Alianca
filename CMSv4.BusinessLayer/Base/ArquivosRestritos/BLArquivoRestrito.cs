using System;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLArquivoRestrito
    {
        //Publico
        #region Obter arquivo restrito
        /// <summary>
        /// Obter arquivo restrito
        /// </summary>
        public static MLArquivoRestrito Obter(string pstrCategoria)
        {
            MLArquivoRestrito retorno = null;

            try
            {
                var portal = BLPortal.Atual;

                using (var command = Database.NewCommand("USP_MOD_ARE_S_ARQUIVO_RESTRITO", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@ARE_C_CATEGORIA", SqlDbType.VarChar, 500, pstrCategoria);
                    
                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLArquivoRestrito();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLArquivoRestrito>(dataset.Tables[0].Rows[0]);
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
    }
}
