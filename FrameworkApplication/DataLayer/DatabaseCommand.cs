using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Framework.Utilities;

namespace Framework.DataLayer
{
    /// <summary>
    /// Métodos para execução dos comandos de banco de dados
    /// </summary>
    public static partial class Database
    {
        // OPEN

        #region OpenConnection
        
        /// <summary>
        /// Abre uma conexão para execução
        /// </summary>
        /// <param name="command"></param>
        public static void OpenConnection(SqlCommand command)
        {
            if (command.Connection == null) command.Connection = NewConnection();
            if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
        }

        #endregion

        // EXECUTE

        #region ExecuteScalar

        /// <summary>
        /// Executa um comando e retorna apenas um valor
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(SqlCommand command)
        {
            OpenConnection(command);

            if (Transaction.Current != null)
                return command.ExecuteScalar();
            else
                using (command.Connection)
                {
                    return command.ExecuteScalar();
                }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executa um comando e retorna a quantidade de registros afetados
        /// </summary>
        /// <param name="command">Comando</param>
        /// <returns>Quantidade de registros afetados</returns>
        public static int ExecuteNonQuery(SqlCommand command)
        {
            OpenConnection(command);

            if (Transaction.Current != null)
                return command.ExecuteNonQuery();
            else
                using (command.Connection)
                {
                    return command.ExecuteNonQuery();
                }
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// Executa um leitor de dados de um resultado e retorna uma lista 
        /// da entidade informada
        /// </summary>
        /// <remarks>
        /// Este método cria uma lista genérica conforme o tipo da model informada
        /// </remarks>
        /// <param name="command">Comando</param>
        /// <param name="model">Entidade para retorno</param>
        /// <returns>Lista da entidade informada</returns>
        public static List<TipoModel> ExecuteReader<TipoModel>(SqlCommand command)
        {
            //if (command.CommandType == CommandType.StoredProcedure)
            //    Profiler.Save(command, command.Connection.ConnectionString);

            OpenConnection(command);
            
            // Cria uma lista genérica conforme o tipo informado
            var lstRetorno = new List<TipoModel>();

            if (Transaction.Current != null)
                using (var idrRetorno = command.ExecuteReader())
                {
                    while (idrRetorno.Read())
                        lstRetorno.Add( Database.FillModel<TipoModel>(idrRetorno) );
                }
            else
            {
                using (command.Connection)
                using (var idrRetorno = command.ExecuteReader())
                {
                    while (idrRetorno.Read())
                        lstRetorno.Add( Database.FillModel<TipoModel>(idrRetorno) );
                }
            }

            return lstRetorno;
        }

        /// <summary>
        /// Retorno de reader padrão
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(SqlCommand command, CommandBehavior behavior)
        {
            OpenConnection(command);

            if (Transaction.Current != null)
                return command.ExecuteReader(CommandBehavior.Default);
            else
                return command.ExecuteReader(behavior);
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// Executa um comando e retorna um DataSet de resultado
        /// </summary>
        /// <param name="command">comando</param>
        /// <returns><DataSet/returns>
        public static DataSet ExecuteDataSet(SqlCommand command)
        {
            OpenConnection(command);

            var ds = new DataSet();

            if (Transaction.Current != null)
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                    return ds;
                }
            }
            else
                using (command.Connection)
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(ds);
                        return ds;
                    }
                }
        }

        #endregion

    }
}
