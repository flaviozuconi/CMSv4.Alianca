using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Framework.Model;
using System.Linq;
using System.Reflection;

namespace Framework.DataLayer
{
    /// <summary>
    /// Métodos para preenchimento de models
    /// </summary>
    public static partial class Database
    {
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
        public static TipoModel FillModel<TipoModel>(IDataReader row)
        {
            if (row == null) return Activator.CreateInstance<TipoModel>();

            string fieldName = "";

            try
            {
                var propriedades = (from e in typeof(TipoModel).GetProperties()
                                    where e.IsDefined(typeof(DataField), false) && e.GetCustomAttribute<DataField>(false).FillModel
                                    select new { key = e.GetCustomAttribute<DataField>(false).Name, prop = e }
                ).ToList();

                var modelPreenchida = Activator.CreateInstance<TipoModel>();
                var colunas = new List<string>();

                for (int i = 0; i < (row.FieldCount); i++)
                {
                    colunas.Add(row.GetName(i));

                }

                // Procura as informações em [row] conforme as propriedades da [model]
                for (int i = 0; i < propriedades.Count; i++)
                {
                    fieldName = propriedades[i].key;

                    if (colunas.Contains(fieldName) && row[fieldName] != System.DBNull.Value)
                    {
                        propriedades[i].prop.SetValue(modelPreenchida, row[fieldName], null);
                    }
                }

                return modelPreenchida;
            }
            catch (ArgumentException ex)
            {
                if (!string.IsNullOrEmpty(fieldName))
                    throw new ArgumentException(
                        string.Format("Erro no tipo de dados de '{0}'. {1}", fieldName, ex.Message));
                else
                    throw;
            }

        }

        #endregion

        #region FillModel from DataRow

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
        public static TipoModel FillModel<TipoModel>(DataRow row, Dictionary<string, PropertyInfo> parPropriedades = null)
        {
            if (row == null) return Activator.CreateInstance<TipoModel>();

            string fieldName = "";

            try
            {

                var propriedades = parPropriedades ?? (from e in typeof(TipoModel).GetProperties()
                                    where e.IsDefined(typeof(DataField), false) && e.GetCustomAttribute<DataField>(false).FillModel
                                    select new { key = e.GetCustomAttribute<DataField>(false).Name, prop = e }
                ).ToList().ToDictionary(k => k.key, v => v.prop);

                var modelPreenchida = Activator.CreateInstance<TipoModel>();

                // Procura as informações em [row] conforme as propriedades da [model]
                foreach (var propriedade in propriedades)
                {
                    fieldName = propriedade.Key;

                    if (row.Table.Columns.Contains(fieldName) && row[fieldName] != System.DBNull.Value)
                    {
                        propriedade.Value.SetValue(modelPreenchida, row[fieldName], null);
                    }
                }

                return modelPreenchida;
            }
            catch (ArgumentException ex)
            {
                if (!string.IsNullOrEmpty(fieldName))
                    throw new ArgumentException(
                        string.Format("Erro no tipo de dados de '{0}'. {1}", fieldName, ex.Message));
                else
                    throw;
            }
        }

        #endregion

        #region FillModel from Command

        /// <summary>
        /// Popula a model com um command recebido, pelos parametros de output
        /// </summary>
        /// <remarks>
        /// O formato do parâmetro de output deve ser: @OUT_[Nome do campo]
        /// </remarks>
        /// <param name="model">model</param>
        /// <param name="row">row</param>
        /// <returns>model preenchida</returns>
        public static TipoModel FillModel<TipoModel>(SqlCommand row)
        {

            if (row == null) return Activator.CreateInstance<TipoModel>();

            const string nameFormat = "@OUT_{0}";
            string fieldName = "";

            try
            {
                var propriedades = (from e in typeof(TipoModel).GetProperties()
                                    where e.IsDefined(typeof(DataField), false) && e.GetCustomAttribute<DataField>(false).FillModel
                                    select new { key = e.GetCustomAttribute<DataField>(false).Name, prop = e }
                ).ToList();

                var modelPreenchida = Activator.CreateInstance<TipoModel>();

                // Procura as informações em [row] conforme as propriedades da [model]
                for (int i = 0; i < propriedades.Count; i++)
                {
                    fieldName = string.Format(nameFormat, propriedades[i].key);

                    if (row.Parameters.Contains(fieldName)
                       && row.Parameters[fieldName] != null
                       && row.Parameters[fieldName].Value != null
                       && row.Parameters[fieldName].Value != System.DBNull.Value)
                    {
                        propriedades[i].prop.SetValue(modelPreenchida, row.Parameters[fieldName].Value, null);
                    }
                }

                return modelPreenchida;
            }
            catch (ArgumentException ex)
            {
                if (!string.IsNullOrEmpty(fieldName))
                    throw new ArgumentException(
                        string.Format("Erro no tipo de dados de '{0}'. {1}", fieldName, ex.Message));
                else
                    throw;
            }
        }

        #endregion

        #region FillList from DataTable

        /// <summary>
        /// Popula uma lista de model com um reader recebido
        /// </summary>
        /// <remarks>
        /// Preenche os atributos da model conforme as colunas 
        /// recebidas no item do datareader.
        /// </remarks>
        /// <param name="model">model</param>
        /// <param name="row">row</param>
        /// <returns>model preenchida</returns>
        public static List<TipoModel> FillList<TipoModel>(DataTable table)
        {
            var model = Activator.CreateInstance<TipoModel>();
            return FillList<TipoModel>(model, table);
        }

        public static List<TipoModel> FillList<TipoModel>(TipoModel model, DataTable table)
        {
            if (table == null) return new List<TipoModel>();


            var propriedades = (from e in model.GetType().GetProperties()
                                where e.IsDefined(typeof(DataField), false) && e.GetCustomAttribute<DataField>(false).FillModel
                                select new { key = e.GetCustomAttribute<DataField>(false).Name, prop = e }
            ).ToList().ToDictionary(k => k.key, v => v.prop);

            // Cria uma lista genérica conforme o tipo informado
            var lstRetorno = new List<TipoModel>();

            foreach (DataRow row in table.Rows)
                lstRetorno.Add(Database.FillModel<TipoModel>(row, propriedades));
            
            return lstRetorno;
        }

        #endregion
    }
}
