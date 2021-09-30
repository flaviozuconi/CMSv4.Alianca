using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Transactions
{
    /// <summary>
    /// Esta classe facilita o uso do contexto de requisição para armazenar informações
    /// durante a chamada. É utilizada principalmente para controle de transação SQL
    /// </summary>
    public class CallContext
    {
        #region Get e Set

        /// <summary>
        /// Define uma informação no contexto
        /// </summary>
        public static void SetData(string key, object obj)
        {
            if (!HttpContextFactory.Current.Items.Contains(key))
                HttpContextFactory.Current.Items.Add(key, obj);
        }

        /// <summary>
        /// Obtem uma informação do contexto
        /// </summary>
        public static object GetData(string key)
        {
            if (HttpContextFactory.Current.Items[key] != null)
            {
                return HttpContextFactory.Current.Items[key];
            }

            return null;
        }

        #endregion
    }


    /// <summary>
    /// Esta classe substitui o controle de transações nativo, para evitar que transações entre classes sejam
    /// elevadas para nível de segurança que impeça de serem executadas em ambientes de hospedagens web comuns
    /// </summary>
    public static class Transaction
    {
        #region Current

        /// <summary>
        /// Obtem ou define uma transação SQL para a requisição atual
        /// </summary>
        public static System.Data.SqlClient.SqlTransaction Current
        {
            set
            {
                if (HttpContextFactory.Current.Session == null) return;

                string key = HttpContextFactory.Current.Session.SessionID + "current-transaction";
                HttpContextFactory.Current.Items.Add(key, value);
            }

            get
            {
                if (HttpContextFactory.Current == null) return null;
                if (HttpContextFactory.Current.Session == null) return null;

                string key = HttpContextFactory.Current.Session.SessionID + "current-transaction";

                if (HttpContextFactory.Current.Items[key] != null)
                    return (System.Data.SqlClient.SqlTransaction)HttpContextFactory.Current.Items[key];

                return null;
            }
        }

        #endregion

        #region CurrentAuditing

        /// <summary>
        /// Obtem ou define uma transação SQL para a requisição atual
        /// </summary>
        public static Framework.Utilities.MLAuditoria CurrentAuditing
        {
            set
            {
                if (HttpContextFactory.Current.Session == null) return;

                string key = HttpContextFactory.Current.Session.SessionID + "current-transaction-auditing";
                HttpContextFactory.Current.Items.Add(key, value);
            }

            get
            {
                if (HttpContextFactory.Current.Session == null) return null;

                string key = HttpContextFactory.Current.Session.SessionID + "current-transaction-auditing";

                if (HttpContextFactory.Current.Items[key] != null)
                    return (Framework.Utilities.MLAuditoria)HttpContextFactory.Current.Items[key];

                return null;
            }
        }

        #endregion
    }


    /// <summary>
    /// Garante que chamadas SQL realizadas dentro do escopo estejam na mesma transação
    /// </summary>
    public class TransactionScope : IDisposable
    {
        #region Construtor

        /// <summary>
        /// Define uma transação ao ser iniciado
        /// </summary>
        public TransactionScope()
        {
            var connection = Framework.DataLayer.Database.NewTransactionalConnection();
            Transaction.Current = connection.BeginTransaction();
        }

        #endregion

        #region Construtor (connectionString)

        /// <summary>
        /// Contrutor (connectionstring)
        /// </summary>
        public TransactionScope(string connectionString)
        {
            var connection = Framework.DataLayer.Database.NewTransactionalConnection(connectionString);

            Transaction.Current = connection.BeginTransaction();
        }

        #endregion

        #region Complete

        /// <summary>
        /// Persiste as alterações no banco de dados e remove a conexão do contexto
        /// </summary>
        public void Complete()
        {
            var modelauditoria = Transaction.CurrentAuditing;
            Transaction.Current.Commit();
            
            string key = HttpContextFactory.Current.Session.SessionID + "current-transaction";
            HttpContextFactory.Current.Items.Remove(key);
            if (modelauditoria != null)
            {                
                string keyaud = HttpContextFactory.Current.Session.SessionID + "current-transaction-auditing";
                HttpContextFactory.Current.Items.Remove(keyaud);
                Framework.Utilities.CRUD.Salvar(modelauditoria, Framework.Utilities.ApplicationSettings.ConnectionStrings.Default);
            }

        }

        #endregion

        #region Dispose

        /// <summary>
        /// Fecha a conexão
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (Transaction.Current != null
                    && Transaction.Current.Connection != null
                    && Transaction.Current.Connection.State == Data.ConnectionState.Open)
                {
                    Transaction.Current.Rollback();

                    string key = HttpContextFactory.Current.Session.SessionID + "current-transaction";
                    HttpContextFactory.Current.Items.Remove(key);
                }
            }
            catch { }
        }

        #endregion
    }
}
