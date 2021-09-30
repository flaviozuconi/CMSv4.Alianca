using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Framework.Utilities;
using Framework.DataLayer;
using System.Data.SqlClient;

namespace Framework.Utilities
{
    /// <summary> 
    /// Classe de Conexao do Portal
    /// </summary> 
    /// <user>Gerador [1.0.0.0]</user> 
    public static class DatabasePortal
    {
        #region NewConnection

        /// <summary>
        /// Nova conexão
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewConnection()
        {
            return Database.NewConnection(BLPortal.Atual.ConnectionString);
        }

        #endregion

        #region NewTransactionalConnection

        /// <summary>
        /// Nova conexão em ambiente transacional
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewTransactionalConnection()
        {
            return Database.NewTransactionalConnection(BLPortal.Atual.ConnectionString);
        }

        #endregion

        #region NewCommand

        /// <summary>
        /// Retorna um novo comando preparado para execução de uma procedure especificada
        /// </summary>
        /// <param name="storedProcedureName">nome da stored procedure</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand NewCommand(string storedProcedureName)
        {
            return Database.NewCommand(storedProcedureName, BLPortal.Atual.ConnectionString);
        }

        #endregion


    }
}
