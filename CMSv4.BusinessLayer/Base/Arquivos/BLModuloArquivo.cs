using System;
using System.Data;
using Framework.DataLayer;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLModuloArquivo
    {
        #region Obter

        /// <summary>
        /// Obter
        /// </summary>
        public static decimal? Obter(string nome, decimal? id)
        {
            using (var command = Database.NewCommand("USP_MOD_ARQ_S_CATEGORIAS_ARQUIVO", BLPortal.Atual.ConnectionString))
            {
                // Parametros   

                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@ACA_C_NOME", SqlDbType.VarChar, 250, nome);
                command.NewCriteriaParameter("@ACA_N_CODIGO", SqlDbType.Decimal, 18, id);

                return Convert.ToDecimal(Database.ExecuteScalar(command));
            }
        }

        #endregion

        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        //public static List<MLArquivo> ListarPublico(MLArquivo model, bool? IsDestaque, bool? IsOrdemData)
        //{
        //    using (var command = Database.NewCommand("USP_MOD_ARQ_L_ARQUIVOS_PUBLICO", BLPortal.Atual.ConnectionString))
        //    {
        //        // Parametros
        //        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoPortal);
        //        command.NewCriteriaParameter("@PASTA", SqlDbType.VarChar, -1, model.PastaRelativa);
        //        command.NewCriteriaParameter("@ARQ_B_DESTAQUE", SqlDbType.Bit, model.Destaque);
        //        command.NewCriteriaParameter("@ARQ_C_ORDEM_DATA", SqlDbType.Bit, IsOrdemData);
        //        command.NewCriteriaParameter("@ARQ_B_DESTAQUE", SqlDbType.Bit, IsDestaque);
                
        //        // Execucao
        //        return Database.ExecuteReader<MLArquivo>(command);
        //    }
        //}

        #endregion
    }
}
