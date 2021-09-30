using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using Framework.Model;
using Framework.Utilities;

namespace Framework.DataLayer
{
    /// <summary>
    /// Classe básica para conexão com banco de dados
    /// </summary>
    public static partial class Database
    {
        // NEW

        #region NewConnection

        /// <summary>
        /// Nova conexão
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewConnection()
        {
            return NewConnection(ApplicationSettings.ConnectionStrings.Default);
        }

        /// <summary>
        /// Nova conexão
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewConnection(string connectionString)
        {
            if (Transaction.Current != null)
            {
                var localConnection = (SqlConnection)CallContext.GetData(connectionString);

                if (localConnection != null
                    && !string.IsNullOrEmpty(localConnection.ConnectionString)
                    && localConnection.State == ConnectionState.Open)
                {
                    return localConnection;
                }
            }

            return new SqlConnection(connectionString);
        }

        #endregion

        #region NewTransactionalConnection

        /// <summary>
        /// Nova conexão em ambiente transacional
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewTransactionalConnection()
        {
            return NewTransactionalConnection(ApplicationSettings.ConnectionStrings.Default);
        }

        /// <summary>
        /// Nova conexão em ambiente transacional
        /// </summary>
        /// <returns>conexão</returns>
        public static SqlConnection NewTransactionalConnection(string connectionString)
        {
            var conExistente = (SqlConnection)CallContext.GetData(connectionString);
            if (conExistente != null) return conExistente;

            var conexao = new SqlConnection(connectionString);

            conexao.Open();

            CallContext.SetData(connectionString, conexao);

            return conexao;
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
            return NewCommand(storedProcedureName, string.Empty);
        }

        /// <summary>
        /// Retorna um novo comando preparado para execução de uma procedure especificada
        /// </summary>
        /// <param name="storedProcedureName">nome da stored procedure</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand NewCommand(string storedProcedureName, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ApplicationSettings.ConnectionStrings.Default;

            if (Transaction.Current != null)
            {
                return new SqlCommand
                {
                    CommandType = string.IsNullOrEmpty(storedProcedureName) ? CommandType.Text : CommandType.StoredProcedure,
                    CommandText = storedProcedureName,
                    CommandTimeout = ApplicationSettings.SqlSettings.CommandTimeOut,
                    Transaction = System.Transactions.Transaction.Current,
                    Connection = System.Transactions.Transaction.Current.Connection
                };
            }

            return new SqlCommand
            {
                CommandType = string.IsNullOrEmpty(storedProcedureName) ? CommandType.Text : CommandType.StoredProcedure,
                CommandText = storedProcedureName,
                CommandTimeout = ApplicationSettings.SqlSettings.CommandTimeOut,
                Connection = NewConnection(connectionString),
                Transaction = System.Transactions.Transaction.Current
            };
        }

        #endregion

        #region FillModel from DataReader

        /// <summary>
        /// Popula a model com um reader recebido
        /// </summary>
        /// <remarks>
        /// Preenche os atributos da model conforme as colunas 
        /// recebidas no item do datareader.
        /// </remarks>
        /// <param name="model">model</param>
        /// <param name="row">row</param>
        /// <returns>model preenchida</returns>
        public static object FillModel( object model, IDataReader row )
        {
            if( model == null ) return null;
            if( row == null ) return null;

            string fieldName = "";

            try
            {
                var propriedades = model.GetType( ).GetProperties( );
                var modelPreenchida = Activator.CreateInstance( model.GetType( ) );
                var colunas = new List<string>( );

                for( int i = 0 ; i < ( row.FieldCount ) ; i++ )
                {
                    colunas.Add( row.GetName( i ) );
                }

                // Procura as informações em [row] conforme as propriedades da [model]
                foreach( var item in propriedades )
                {
                    var dbField = item.GetCustomAttributes( typeof( DataField ), false );

                    if( dbField == null || dbField.Length <= 0 ) continue;
                    fieldName = ( (DataField)dbField[ 0 ] ).Name;

                    if( colunas.Contains( fieldName ) )
                        if( row[ fieldName ] != System.DBNull.Value )
                            item.SetValue( modelPreenchida, row[ fieldName ], null );
                }

                return modelPreenchida;
            }
            catch( ArgumentException ex )
            {
                if( !string.IsNullOrEmpty( fieldName ) )
                    throw new ArgumentException(
                        string.Format( "Erro no tipo de dados de '{0}'. {1}", fieldName, ex.Message ) );
                else
                    throw;
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
        public static object ExecuteReader( SqlCommand command, object model )
        {
            OpenConnection( command );

            // Cria uma lista genérica conforme o tipo informado
            Type objType = model.GetType( );
            Type listType = typeof( List<> );
            Type creatableList = listType.MakeGenericType( objType );
            object lstRetorno = Activator.CreateInstance( creatableList );
            MethodInfo mi = creatableList.GetMethod( "Add" );

            var modelPreenchida = Activator.CreateInstance( model.GetType( ) );

            if( Transaction.Current != null )
                using( var idrRetorno = command.ExecuteReader( ) )
                {
                    while( idrRetorno.Read( ) )
                        mi.Invoke( lstRetorno, new object[ ] { Database.FillModel( modelPreenchida, idrRetorno ) } );
                }
            else
            {
                using( command.Connection )
                using( var idrRetorno = command.ExecuteReader( ) )
                {
                    while( idrRetorno.Read( ) )
                        mi.Invoke( lstRetorno, new object[ ] { Database.FillModel( modelPreenchida, idrRetorno ) } );
                }
            }

            return lstRetorno;
        }
        #endregion

        // CLOSE

        #region CloseConnection

        /// <summary>
        /// Fecha uma conexão e a remove da memória
        /// </summary>
        /// <param name="conexao">SqlConnection</param>
        public static void CloseConnection(SqlConnection conexao)
        {
            if (conexao == null) return;

            try { conexao.Close(); }
            catch { }
            try { conexao.Dispose(); }
            catch { }
            try { conexao = null; }
            catch { }
        }

        /// <summary>
        /// Fecha uma conexão e a remove da memória
        /// </summary>
        /// <param name="conexao">SqlConnection</param>
        public static void CloseConnection(SqlCommand command)
        {
            CloseConnection(command.Connection);
        }

        #endregion

        #region CloseReader

        /// <summary>
        /// Fecha uma datareader e o remove da memória
        /// </summary>

        public static void CloseReader(IDataReader reader)
        {
            if (reader == null) return;

            try { reader.Close(); }
            catch { }
            try { reader.Dispose(); }
            catch { }
            try { reader = null; }
            catch { }
        }

        #endregion
    }
}
