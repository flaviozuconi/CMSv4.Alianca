using System;
using System.Data;
using System.Data.SqlClient;

namespace Framework.DataLayer
{
    /// <summary>
    /// Parâmetros utilizados no preenchimentos dos SqlCommands
    /// </summary>
    public static partial class Database
    {
        #region NewParameter

        /// <summary>
        /// Cria um novo parâmetro para execução do SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <param name="value">Valor</param>
        public static void NewParameter(this SqlCommand command, string name, SqlDbType type, object value)
        {
            var parametro = new SqlParameter(name, type);
            parametro.Value = value;

            command.Parameters.Add(parametro);
        }

        /// <summary>
        /// Cria um novo parâmetro para execução do SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <param name="size">Tamanho</param>
        /// <param name="value">Valor</param>
        public static void NewParameter(this SqlCommand command, string name, SqlDbType type, int size, object value)
        {
            var parametro = new SqlParameter(name, type, size);
            parametro.Value = value;

            command.Parameters.Add(parametro);
        }

        /// <summary>
        /// Cria um novo parâmetro para execução do SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <param name="size">Tamanho</param>
        /// <param name="scale">Quantidade casas decimais</param>
        /// <param name="value">Valor</param>
        public static void NewParameter(this SqlCommand command, string name, SqlDbType type, int size, byte scale, object value)
        {
            var parametro = new SqlParameter(name, type, size);
            parametro.Precision = Convert.ToByte(size);
            parametro.Scale = scale;
            parametro.Value = value;

            command.Parameters.Add(parametro);
        }

        #endregion

        #region NewOutputParameter

        /// <summary>
        /// Cria um novo parâmetro de retorno para um SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <returns>SqlParameter</returns>
        public static void NewOutputParameter(this SqlCommand command, string name, SqlDbType type)
        {
            var parametro = new SqlParameter(name, type);
            parametro.Direction = ParameterDirection.Output;

            command.Parameters.Add(parametro);
        }

        /// <summary>
        /// Cria um novo parâmetro de retorno para um SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <param name="size">Tamanho</param>
        /// <returns>SqlParameter</returns>
        public static void NewOutputParameter(this SqlCommand command, string name, SqlDbType type, int size)
        {
            var parametro = new SqlParameter(name, type, size);
            parametro.Direction = ParameterDirection.Output;

            command.Parameters.Add(parametro);
        }

        /// <summary>
        /// Cria um novo parâmetro de retorno para um SqlCommand
        /// </summary>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="type">Tipo</param>
        /// <param name="size">Tamanho</param>
        /// <param name="scale">Quantidade de casas decimais</param>
        /// <returns>SqlParameter</returns>
        public static void NewOutputParameter(this SqlCommand command, string name, SqlDbType type, int size, byte scale)
        {
            var parametro = new SqlParameter(name, type, size);
            parametro.Scale = scale;
            parametro.Direction = ParameterDirection.Output;

            command.Parameters.Add(parametro);
        }

        #endregion

        #region NewCriteriaParameter

        /// <summary>
        /// Cria um novo parâmetro para critério de busca, caso o valor nãos seja nulo
        /// </summary>
        /// <param name="command">Comando</param>
        /// <param name="name">Nome do parâemtro</param>
        /// <param name="value">Valor</param>
        public static void NewCriteriaParameter(this SqlCommand command, string name, object value)
        {
            if (value != null)
                command.Parameters.AddWithValue(name, value);
        }

        /// <summary>
        /// Cria um novo parâmetro para critério de busca, mesmo que o valor seja nulo
        /// </summary>
        /// <param name="command">Comando</param>
        /// <param name="name">Nome do parâemtro</param>
        /// <param name="value">Valor</param>
        public static void NewParameter(this SqlCommand command, string name, object value)
        {
            if (value != null)
                command.Parameters.AddWithValue(name, value);
            else
                command.Parameters.AddWithValue(name, DBNull.Value);
        }

        /// <summary>
        /// Cria um novo parâmetro para critério de busca, caso o valor nãos seja nulo
        /// </summary>
        /// <param name="command">Comando</param>
        /// <param name="name">Nome do parâemtro</param>
        /// <param name="type">Tipo</param>
        /// <param name="value">Valor</param>
        public static void NewCriteriaParameter(this SqlCommand command, string name, SqlDbType type, object value)
        {
            if (value != null)
                command.NewParameter(name, type, value);
        }

        /// <summary>
        /// Cria um novo parâmetro para critério de busca, caso o valor nãos seja nulo
        /// </summary>
        /// <param name="command">Comando</param>
        /// <param name="name">Nome do parâemtro</param>
        /// <param name="type">Tipo</param>
        /// <param name="value">Valor</param>
        public static void NewCriteriaParameter(this SqlCommand command, string name, SqlDbType type, int size, object value)
        {
            if (value != null && !string.IsNullOrEmpty(Convert.ToString(value)))
                command.NewParameter(name, type, size, value);
        }

        /// <summary>
        /// Cria um novo parâmetro para critério de busca, caso o valor nãos seja nulo
        /// </summary>
        /// <param name="command">Comando</param>
        /// <param name="name">Nome do parâemtro</param>
        /// <param name="type">Tipo</param>
        /// <param name="value">Valor</param>
        public static void NewCriteriaParameter(this SqlCommand command, string name, SqlDbType type, int size, byte scale, object value)
        {
            if (value != null && !string.IsNullOrEmpty(Convert.ToString(value)))
                    command.NewParameter(name, type, size, scale, value);
        }

        
        #endregion
    }
}
