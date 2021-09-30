using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.DataLayer;
using System.Data;
using Framework.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.Utilities
{
    /// <summary>
    /// TODO LIST:
    /// -Criar um CustomSelect onde o próprio usuário informa a query sql
    /// -Metodo de retorno ToList<T> e/ou ToDictionary<T1, T2> e/ou retorno do tipo Anônimo
    /// -Método para adicionar alias na tabela principal e nas tabelas com join (editado: é inútil)
    /// </summary>
    public partial class CRUD
    {
        public class Select<TipoModel>
        {
            protected List<string> ignoreList = new List<string>();
            protected List<string> whereList = new List<string>();
            protected Dictionary<string, string> joinList = new Dictionary<string, string>();
            protected Dictionary<string, object> comandList = new Dictionary<string, object>();
            protected Dictionary<string, string> columnList = new Dictionary<string, string>();
            protected String orderByCommand = string.Empty;
            protected String tableName = string.Empty;
            protected String aliasTable = "tabela";
            protected int topSelect = 0;
            protected int paginationFrom = 0;
            protected int paginationTo = 0;
            protected bool paginationEnabled = false;
            protected bool distinctSelect = false;

            #region Select
            /// <summary>
            /// Seleciona a propriedade da Model presente na base de dados
            /// </summary>
            public Select()
            {
                SetTableName();

                AddColumns(typeof(TipoModel), aliasTable);
            }

            /// <summary>
            /// Seleciona a propriedade da Model presente na base de dados
            /// </summary>
            public Select(string colunaSql)
            {
                SetTableName();

                if (string.IsNullOrEmpty(colunaSql))
                    throw new ApplicationException("Não foi encontrado referência da coluna na model");

                columnList[colunaSql] = string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", colunaSql);
            }

            /// <summary>
            /// Seleciona a propriedade da Model presente na base de dados
            /// </summary>
            public Select(params string[] colunasSql)
            {
                SetTableName();

                if (colunasSql.Length == 0 || string.IsNullOrEmpty(colunasSql[0]))
                    throw new ApplicationException("Não foi encontrado referência da coluna na model");

                for (int i = 0; i < colunasSql.Length; i++)
                    columnList[colunasSql[i]] = string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", colunasSql[i]);
            }

            /// <summary>
            /// Seleciona a propriedade da Model presente na base de dados
            /// </summary>
            public Select(Expression<Func<TipoModel, object>> propriedade)
            {
                SetTableName();

                AddColumnSelect(propriedade);
            }

            /// <summary>
            /// Seleciona a propriedade da Model presente na base de dados
            /// </summary>
            public Select(params Expression<Func<TipoModel, object>>[] propriedades)
            {
                SetTableName();

                for (int i = 0; i < propriedades.Length; i++)
                    AddColumnSelect(propriedades[i]);
            }
            #endregion

            #region Ignore
            public Select<TipoModel> Ignore(Expression<Func<TipoModel, object>> propriedade)
            {
                var expression = GetExpression(propriedade);

                if (expression.HasValue)
                {
                    ignoreList.Add(expression.Value.Value);
                }

                return this;
            }
            #endregion

            #region OrderBy

            public Select<TipoModel> OrderBy(Expression<Func<TipoModel, object>> propriedade)
            {
                AddOrderBy(propriedade, true);

                return this;
            }

            public Select<TipoModel> OrderBy(params Expression<Func<TipoModel, object>>[] propriedades)
            {
                for (int i = 0; i < propriedades.Length; i++)
                {
                    AddOrderBy(propriedades[i], true);
                }

                return this;
            }

            public Select<TipoModel> OrderByDesc(Expression<Func<TipoModel, object>> propriedade)
            {
                AddOrderBy(propriedade, false);

                return this;
            }

            public Select<TipoModel> OrderByDesc(params Expression<Func<TipoModel, object>>[] propriedades)
            {
                for (int i = 0; i < propriedades.Length; i++)
                {
                    AddOrderBy(propriedades[i], false);
                }

                return this;
            }

            private void AddOrderBy(Expression<Func<TipoModel, object>> propriedade, bool isAsc)
            {
                var expression = GetExpression(propriedade);

                if (expression.HasValue)
                {
                    if (!String.IsNullOrEmpty(orderByCommand))
                        orderByCommand = string.Concat(orderByCommand, ", ", expression.Value.Value, isAsc ? "" : " DESC");
                    else
                        orderByCommand = string.Concat("ORDER BY ", expression.Value.Value, isAsc ? "" : " DESC");
                }
            }

            #endregion

            #region ToList/First
            /// <summary>
            /// Retorna o total de registors que atinjam a cláusula experada
            /// </summary>
            /// <param name="distinct">Distinguir registros duplicados</param>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            public int Count(string connectionString = "")
            {
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = ApplicationSettings.ConnectionStrings.Default;

                if (columnList.Count > 1)
                    throw new ApplicationException("Para realizar o Count, informe apenas uma coluna");

                int retorno = 0;

                try
                {
                    using (var command = Database.NewCommand(string.Empty, connectionString))
                    {
                        foreach (var item in comandList)
                            command.NewCriteriaParameter(item.Key, item.Value);

                        command.CommandText = GerarQuerySql(count: true);




                        using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                        {
                            if (reader.Read())
                            {
                                retorno = Convert.ToInt32(reader.GetValue(0));
                            }

                            Database.CloseReader(reader);
                            Database.CloseConnection(command);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
                finally
                {
                    Reset();
                }

                return retorno;
            }

            /// <summary>
            /// Retorna lista de registros que atinjam a cláusula experada
            /// </summary>
            public Select<TipoModel> Distinct()
            {
                distinctSelect = true;

                return this;
            }

            /// <summary>
            /// Retorna lista de registros que atinjam a cláusula experada
            /// </summary>
            /// <param name="top">Seleciona os primeiros N registros</param>
            public Select<TipoModel> Top(int top)
            {
                topSelect = top;

                return this;
            }

            /// <summary>
            /// Retorna lista de registros que atinjam a cláusula experada
            /// </summary>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            public List<TipoModel> ToList(string connectionString = "")
            {
                return Listar<TipoModel>(connectionString);
            }

            /// <summary>
            /// Retorna lista de registros que atinjam a cláusula experada
            /// </summary>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            public List<T> ToList<T>(string connectionString = "")
            {
                return Listar<T>(connectionString);
            }

            /// <summary>
            /// Retorna lista de registros que atinjam a cláusula experada
            /// </summary>
            /// <param name="top">Caso informado, seleciona os primeiros N registros</param>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            private List<T> Listar<T>(string connectionString = "")
            {
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = ApplicationSettings.ConnectionStrings.Default;

                var listaRetorno = new List<T>();
                var propriedades = ListarPropriedades(typeof(TipoModel), aliasTable);

                try
                {
                    using (var command = Database.NewCommand(string.Empty, connectionString))
                    {
                        foreach (var item in comandList)
                            command.NewCriteriaParameter(item.Key, item.Value);

                        command.CommandText = GerarQuerySql();


                        #region FillModel Manual
                        //TODO: não está implementado o controle de transactions que existe em Database.ExecuteReader
                        using (var reader = Database.ExecuteReader(command, CommandBehavior.Default))
                        {
                            while (reader.Read())
                            {
                                object obj = null;

                                if (typeof(T).IsClass)
                                {
                                    obj = FillModel(typeof(T), propriedades, reader);
                                }
                                else
                                {
                                    obj = reader.GetValue(0);
                                }

                                listaRetorno.Add((T)obj);
                            }

                            Database.CloseReader(reader);
                            Database.CloseConnection(command);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }
                finally
                {
                    Reset();
                }

                return listaRetorno;
            }

            /// <summary>
            /// Retorna apenas um registro que atinja a cláusula experada
            /// </summary>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            public TipoModel First(string connectionString = "")
            {
                return Obter<TipoModel>(connectionString);
            }

            /// <summary>
            /// Retorna apenas um registro que atinja a cláusula experada
            /// </summary>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            public T First<T>(string connectionString = "")
            {
                return Obter<T>(connectionString);
            }

            /// <summary>
            /// Retorna apenas um registro que atinja a cláusula experada
            /// </summary>
            /// <param name="connectionString">ConnectionString da base de dados</param>
            private T Obter<T>(string connectionString = "")
            {
                var lista = Listar<T>(connectionString);
                if (lista.Count > 0)
                {
                    return lista[0];
                }

                return Activator.CreateInstance<T>();
            }

            public string GerarQuerySql(bool count = false)
            {
                StringBuilder sb = new StringBuilder();

                //PAGINATION
                if (paginationEnabled && !count)
                {
                    sb.AppendLine("WITH tabela AS");
                    sb.AppendLine("(");
                }

                //SELECT
                sb.Append("SELECT");

                if (count)
                {
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("COUNT({0}{1}){2}", distinctSelect ? "DISTINCT " : "", columnList.ElementAt(0).Value, Environment.NewLine);
                }
                else
                {
                    if (distinctSelect)
                        sb.Append(" DISTINCT");

                    if (topSelect > 0)
                        sb.AppendFormat(" TOP {0}", topSelect);

                    sb.Append(Environment.NewLine);

                    for (int i = 0; i < ignoreList.Count; i++)
                    {
                        var dict = columnList.FirstOrDefault(a => a.Value.Contains(ignoreList[i]));
                        if (!string.IsNullOrEmpty(dict.Value))
                        {
                            columnList.Remove(dict.Key);
                        }
                    }

                    sb.AppendLine(String.Join(", ", columnList.Select(a => a.Value)));

                    if (paginationEnabled)
                    {
                        if (!string.IsNullOrEmpty(orderByCommand))
                        {
                            sb.AppendLine(string.Format("sql_indice = ROW_NUMBER() OVER ({0}),", orderByCommand));
                        }
                        else
                        {
                            sb.AppendLine("sql_indice = ROW_NUMBER() OVER (ORDER BY 1 ASC),");
                        }

                        sb.AppendLine("sql_total = COUNT(1) OVER ()");
                    }
                }

                //FROM
                sb.AppendFormat("FROM {0} {1}{2}", tableName, aliasTable, Environment.NewLine);

                //JOIN
                foreach (var item in joinList)
                {
                    sb.AppendLine(String.Join(Environment.NewLine, item.Value));
                }

                //WHERE
                if (whereList.Count > 0)
                {
                    sb.AppendLine(String.Join(" ", whereList));
                }

                //PAGINATION
                if (paginationEnabled && !count)
                {
                    sb.AppendLine(")");
                    sb.AppendLine("SELECT *");
                    sb.AppendLine("FROM tabela");
                    sb.AppendLine(string.Format("WHERE sql_indice BETWEEN {0} AND {1}", paginationFrom, paginationTo));
                }

                //ORDER BY
                if (!string.IsNullOrEmpty(orderByCommand))
                    sb.AppendLine(orderByCommand);

                return sb.ToString();
            }
            #endregion

            #region Paginate
            public Select<TipoModel> Paginate(int from, int to)
            {
                paginationFrom = from;
                paginationTo = to;
                paginationEnabled = true;

                return this;
            }
            #endregion

            #region Internal
            private void Reset()
            {
                whereList = new List<string>();
                joinList = new Dictionary<string, string>();
                comandList = new Dictionary<string, object>();
                columnList = new Dictionary<string, string>();
                ignoreList = new List<string>();

                topSelect = 0;
                paginationFrom = 0;
                paginationTo = 0;
                paginationEnabled = false;
                distinctSelect = false;
                orderByCommand = string.Empty;
                tableName = string.Empty;
            }

            private object FillModel(Type tipo, List<PropriedadeModel> propriedadesModel, IDataReader row)
            {
                var propriedades = propriedadesModel ?? ListarPropriedades(tipo, "");
                var modelPreenchida = Activator.CreateInstance(tipo);
                var colunas = new List<string>();

                for (int i = 0; i < row.FieldCount; i++)
                {
                    colunas.Add(row.GetName(i));
                }

                for (int i = 0; i < propriedades.Count; i++)
                {
                    var fieldName = string.Empty;

                    if (propriedades[i].dataField != null)
                    {
                        fieldName = propriedades[i].dataField.Name;
                    }
                    else if (propriedades[i].subSelectField != null)
                    {
                        fieldName = propriedades[i].subSelectField.Coluna;
                    }
                    else if (propriedades[i].joinField != null)
                    {
                        fieldName = propriedades[i].joinField.DestField;
                    }

                    if (joinList.ContainsKey(propriedades[i].propName))
                    {
                        propriedades[i].info.SetValue(modelPreenchida, FillModel(propriedades[i].info.PropertyType, null, row), null);
                    }
                    else
                    {
                        if (colunas.Contains(fieldName) && row[fieldName] != System.DBNull.Value)
                        {
                            propriedades[i].info.SetValue(modelPreenchida, row[fieldName], null);
                        }
                    }
                }

                return modelPreenchida;
            }

            private List<PropriedadeModel> ListarPropriedades(Type tipo, string alias)
            {
                return (from e in tipo.GetProperties()
                        where e.IsDefined(typeof(DataField), false) ||
                               e.IsDefined(typeof(SubSelectField), false) ||
                               e.IsDefined(typeof(JoinField), false) ||
                               e.IsDefined(typeof(JoinModel), false)
                        select new PropriedadeModel
                        {
                            propName = e.Name,
                            nameAttr = string.Concat(alias, "_", e.Name),
                            dataField = e.GetCustomAttribute<DataField>(false),
                            subSelectField = e.GetCustomAttribute<SubSelectField>(false),
                            joinField = e.GetCustomAttribute<JoinField>(false),
                            info = e
                        }).ToList();
            }
            #endregion

            #region Protected
            /// <summary>
            /// Método privado para obtenção de member expression da propriedade informada
            /// </summary>
            protected KeyValuePair<string, string>? GetExpression<T>(Expression<Func<T, object>> propriedade)
            {
                var member = propriedade.Body as MemberExpression;
                var unary = propriedade.Body as UnaryExpression;
                var memberExpression = member ?? (unary != null ? unary.Operand as MemberExpression : null);

                if (memberExpression != null)
                {
                    string key = string.Empty, value = string.Empty;

                    key = memberExpression.Member.Name;

                    if (memberExpression.Member.IsDefined(typeof(DataField), false))
                    {
                        value = memberExpression.Member.GetCustomAttribute<DataField>(false).Name;
                    }
                    else
                    {
                        value = memberExpression.Member.Name;
                    }

                    if (String.IsNullOrEmpty(value))
                        throw new ApplicationException("Não foi encontrado referência da coluna na model");

                    return new KeyValuePair<string, string>(key, value);
                }

                return null;
            }

            protected void JoinColumn(string joinType, string propertyName, string sourceKey, string destTable, string destKey, string destField, string aliasField = "")
            {
                var joinAlias = string.Concat("j", joinList.Count);

                if (!joinList.ContainsKey(string.Format("{0}_{1}", joinAlias, destTable)))
                    joinList.Add(string.Format("{0}_{1}", joinAlias, destTable), string.Format("{0} JOIN {1} AS {2} ON {3} = {4}", joinType, destTable, joinAlias, string.Format("{0}.{1}", joinAlias, destKey), sourceKey));

                if (!columnList.ContainsKey(string.Concat(joinAlias, "_", propertyName)))
                    columnList.Add(string.Concat(joinAlias, "_", propertyName), string.Format("{0}.{1} {2}", joinAlias, destField, !string.IsNullOrEmpty(aliasField) ? aliasField : ""));
            }

            protected void AddColumns(Type tipo, string alias)
            {
                var aliasTabelaSelect = string.Empty;
                var propriedades = ListarPropriedades(tipo, alias);

                if (!string.IsNullOrEmpty(alias))
                {
                    aliasTabelaSelect = string.Concat(alias, ".");
                }

                foreach (var item in propriedades)
                {
                    if (item.subSelectField != null && !columnList.ContainsKey(item.nameAttr))
                    {
                        columnList.Add(item.nameAttr, string.Format("({0}) {1}", item.subSelectField.QuerySql, item.subSelectField.Coluna));
                    }
                    else if (item.joinField != null && !columnList.ContainsKey(item.nameAttr))
                    {
                        var fieldName = string.Empty;

                        if (item.dataField != null && !item.dataField.Name.Equals(item.joinField.DestField))
                        {
                            fieldName = string.Concat(item.joinField.DestField, " ", item.dataField.Name);
                        }
                        else
                        {
                            fieldName = item.joinField.DestField;
                        }

                        JoinColumn("LEFT", item.nameAttr, string.Concat(aliasTabelaSelect, item.joinField.SourceKey), item.joinField.DestTable, item.joinField.DestKey, fieldName);
                    }
                    else if (item.dataField != null && !columnList.ContainsKey(item.nameAttr))
                    {
                        columnList.Add(item.nameAttr, string.Concat(aliasTabelaSelect, item.dataField.Name));
                    }
                }
            }
            #endregion

            #region Join

            private Select<TipoModel> Join(String joinType, Expression<Func<TipoModel, object>> propriedade)
            {
                var joinCount = joinList.Count;
                var joinAlias = string.Concat("j", joinCount);
                var expression = GetExpression(propriedade);
                if (expression.HasValue)
                {
                    var joinModel = (from e in typeof(TipoModel).GetProperties()
                                     where e.IsDefined(typeof(JoinModel)) && e.Name == expression.Value.Key
                                     select new { attr = e.GetCustomAttribute<JoinModel>(false), prop = e }).First();

                    var nomeTabela = string.Empty;

                    if (joinModel.attr != null && !string.IsNullOrEmpty(joinModel.attr.DestTable))
                    {
                        nomeTabela = joinModel.attr.DestTable;
                    }
                    else
                    {
                        nomeTabela = ObterNomeTabela(joinModel.prop.PropertyType);
                    }

                    if (!string.IsNullOrEmpty(nomeTabela))
                    {
                        if (string.IsNullOrEmpty(joinModel.attr.SourceKey) || string.IsNullOrEmpty(joinModel.attr.DestKey))
                            throw new ApplicationException("As chaves de origem e/ou destino não possuem valores válidos para a realização do Join");

                        if (!joinList.ContainsKey(string.Format("{0}_{1}", joinAlias, nomeTabela)))
                        {
                            joinList.Add(string.Format("{0}_{1}", joinAlias, nomeTabela),
                                string.Format("{0} JOIN {1} AS {2} ON j{2}.{3} = {4}.{5}", joinType, nomeTabela, joinAlias, joinModel.attr.DestKey, aliasTable, joinModel.attr.SourceKey));

                            AddColumns(joinModel.prop.PropertyType, joinAlias);
                        }
                    }
                }

                return this;
            }

            private Select<TipoModel> Join<T>(String joinType, Expression<Func<T, object>> joinKey, Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> column = null, string aliasColumn = "")
            {
                var expJoinKey = GetExpression(joinKey);
                var expPrimaryTableKey = GetExpression(primaryTableKey);
                var expColumn = column != null ? GetExpression(column) : null;
                var nomeTabela = ObterNomeTabela(typeof(T));

                if (!string.IsNullOrEmpty(nomeTabela))
                {
                    if (!expPrimaryTableKey.HasValue || !expJoinKey.HasValue)
                        throw new ApplicationException("As chaves de origem e/ou destino não possuem valores válidos para a realização do Join");

                    var joinCount = joinList.Count;
                    var joinAlias = string.Concat("j", joinCount);

                    if (expColumn != null && expColumn.HasValue && !columnList.ContainsKey(string.Format("{0}_{1}", joinAlias, expColumn.Value.Key)))
                    {
                        JoinColumn(joinType, expColumn.Value.Key, string.Concat(aliasTable, ".", expPrimaryTableKey.Value.Value), nomeTabela, expJoinKey.Value.Value, expColumn.Value.Value, aliasColumn);
                    }
                    else
                    {
                        if (!joinList.ContainsKey(string.Format("{0}_{1}", joinAlias, nomeTabela)))
                        {
                            joinList.Add(string.Format("{0}_{1}", joinAlias, nomeTabela), string.Format("{0} JOIN {1} AS {2} ON j{2}.{3} = {4}.{5}", joinType, nomeTabela, joinAlias, expJoinKey.Value.Value, aliasTable, expPrimaryTableKey.Value.Value));

                            AddColumns(typeof(T), joinAlias);
                        }
                    }
                }

                return this;
            }

            public Select<TipoModel> LeftJoin(Expression<Func<TipoModel, object>> propriedade)
            {
                return Join("LEFT", propriedade);
            }

            public Select<TipoModel> LeftJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey, Expression<Func<T, object>> column, string aliasColumn = "")
            {
                return Join<T>("LEFT", joinKey, primaryTableKey, column, aliasColumn);
            }

            public Select<TipoModel> LeftJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey)
            {
                return Join<T>("LEFT", joinKey, primaryTableKey);
            }

            public Select<TipoModel> RightJoin(Expression<Func<TipoModel, object>> propriedade)
            {
                return Join("RIGHT", propriedade);
            }

            public Select<TipoModel> RightJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey, Expression<Func<T, object>> column, string aliasColumn = "")
            {
                return Join<T>("RIGHT", joinKey, primaryTableKey, column, aliasColumn);
            }

            public Select<TipoModel> RightJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey)
            {
                return Join<T>("RIGHT", joinKey, primaryTableKey);
            }

            public Select<TipoModel> InnerJoin(Expression<Func<TipoModel, object>> propriedade)
            {
                return Join("INNER", propriedade);
            }

            public Select<TipoModel> InnerJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey, Expression<Func<T, object>> column, string aliasColumn = "")
            {
                return Join<T>("INNER", joinKey, primaryTableKey, column, aliasColumn);
            }

            public Select<TipoModel> InnerJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey)
            {
                return Join<T>("INNER", joinKey, primaryTableKey);
            }

            public Select<TipoModel> OuterJoin(Expression<Func<TipoModel, object>> propriedade)
            {
                return Join("OUTER", propriedade);
            }

            public Select<TipoModel> OuterJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey, Expression<Func<T, object>> column, string aliasColumn = "")
            {
                return Join<T>("OUTER", joinKey, primaryTableKey, column, aliasColumn);
            }

            public Select<TipoModel> OuterJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey)
            {
                return Join<T>("OUTER", joinKey, primaryTableKey);
            }

            public Select<TipoModel> FullJoin(Expression<Func<TipoModel, object>> propriedade)
            {
                return Join("FULL", propriedade);
            }

            public Select<TipoModel> FullJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey, Expression<Func<T, object>> column, string aliasColumn = "")
            {
                return Join<T>("FULL", joinKey, primaryTableKey, column, aliasColumn);
            }

            public Select<TipoModel> FullJoin<T>(Expression<Func<TipoModel, object>> primaryTableKey, Expression<Func<T, object>> joinKey)
            {
                return Join<T>("FULL", joinKey, primaryTableKey);
            }

            #endregion

            #region Internal
            private void SetTableName()
            {
                tableName = ObterNomeTabela<TipoModel>();

                if (string.IsNullOrEmpty(tableName))
                    throw new ApplicationException("Nome da tabela não definida na model");
            }

            private void AddColumnSelect(Expression<Func<TipoModel, object>> propriedade)
            {
                var expression = GetExpression(propriedade);

                if (expression.HasValue)
                {
                    columnList[expression.Value.Key] = string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", expression.Value.Value);
                }
            }
            #endregion

            #region Class/Enums
            protected class PropriedadeModel
            {
                public string propName { get; set; }
                public string nameAttr { get; set; }
                public DataField dataField { get; set; }
                public SubSelectField subSelectField { get; set; }
                public JoinField joinField { get; set; }
                public PropertyInfo info { get; set; }
            }

            private enum TipoWhereClause
            {
                Equals,
                NotEquals,
                Like,
                GreaterThan,
                GreaterThanEquals,
                LessThan,
                LessThanEquals,
                Between
            }

            #endregion

            #region Where
            #region WhereClause
            /// <summary>
            /// Método privado para geração da cláusula em Where
            /// </summary>
            private Select<TipoModel> WhereClause<T>(Expression<Func<T, object>> propriedade, object valor, bool condicaoOr = false, TipoWhereClause tipo = TipoWhereClause.Equals, bool? isNotNull = null)
            {
                return WhereClause<T>(propriedade, tipo: tipo, condicaoOr: condicaoOr, isNotNull: isNotNull, valores: new object[] { valor });
            }

            /// <summary>
            /// Método privado para geração da cláusula em Where
            /// </summary>
            private Select<TipoModel> WhereClause<T>(Expression<Func<T, object>> propriedade, bool condicaoOr = false, TipoWhereClause tipo = TipoWhereClause.Equals, bool? isNotNull = null, params object[] valores)
            {
                var expression = GetExpression(propriedade);

                if (expression.HasValue)
                {
                    if (comandList.Count == 0)
                    {
                        whereList.Add("WHERE");
                    }
                    else
                    {
                        if (condicaoOr)
                        {
                            whereList.Add("OR");
                        }
                        else
                        {
                            whereList.Add("AND");
                        }
                    }

                    for (int i = 0; i < valores.Length; i++)
                    {
                        int countParametros = comandList.Count;

                        if (valores.Length > 1 && i == 0)
                        {
                            whereList.Add("(");
                        }

                        if (isNotNull.HasValue || valores[i] == null)
                        {
                            if (isNotNull.GetValueOrDefault())
                                whereList.Add(string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", expression.Value.Value, " IS NOT NULL"));
                            else
                                whereList.Add(string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", expression.Value.Value, " IS NULL"));
                        }
                        else
                        {
                            string operatorSql = "="; //valor padrão

                            if (tipo == TipoWhereClause.NotEquals)
                                operatorSql = "!=";
                            else if (tipo == TipoWhereClause.Like)
                                operatorSql = "LIKE";
                            else if (tipo == TipoWhereClause.GreaterThan)
                                operatorSql = ">";
                            else if (tipo == TipoWhereClause.GreaterThanEquals)
                                operatorSql = ">=";
                            else if (tipo == TipoWhereClause.LessThan)
                                operatorSql = "<";
                            else if (tipo == TipoWhereClause.LessThanEquals)
                                operatorSql = "<=";

                            whereList.Add(string.Concat(aliasTable, !string.IsNullOrEmpty(aliasTable) ? "." : "", expression.Value.Value, " ", operatorSql, " @C", countParametros));

                            if (tipo == TipoWhereClause.Like)
                                comandList.Add("@C" + countParametros, "%" + valores[i] + "%");
                            else
                                comandList.Add("@C" + countParametros, valores[i]);
                        }

                        if (valores.Length > 1)
                        {
                            if (i == valores.Length - 1)
                            {
                                whereList.Add(")");
                            }
                            else
                            {
                                whereList.Add("OR");
                            }
                        }
                    }
                }

                return this;
            }
            #endregion

            #region Equals
            /// <summary>
            /// Cláusula onde a propriedade possui valor igual ao informado
            /// </summary>
            public Select<TipoModel> Equals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor igual ao informado
            /// </summary>
            public Select<TipoModel> EqualsTo<T>(Expression<Func<T, object>> propriedade, object valor)
            {
                return WhereClause<T>(propriedade, valor);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor igual aos informados
            /// </summary>
            public Select<TipoModel> Equals(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.Equals);
            }
            #endregion

            #region NotEquals
            /// <summary>
            /// Cláusula onde a propriedade possui valor igual ao informado
            /// </summary>
            public Select<TipoModel> NotEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.NotEquals);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor igual ao informado
            /// </summary>
            public Select<TipoModel> NotEqualsTo<T>(Expression<Func<T, object>> propriedade, object valor)
            {
                return WhereClause<T>(propriedade, valor, tipo: TipoWhereClause.NotEquals);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor igual aos informados
            /// </summary>
            public Select<TipoModel> NotEquals(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.NotEquals);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrNotEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.NotEquals);
            }
            #endregion

            #region Like
            /// <summary>
            /// Cláusula onde a propriedade possui valor equivalente ao informado
            /// </summary>
            public Select<TipoModel> Like(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.Like);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor equivalente aos informados
            /// </summary>
            public Select<TipoModel> Like(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.Like);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrLike(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.Like);
            }
            #endregion

            #region GreaterThan
            /// <summary>
            /// Cláusula onde a propriedade possui valor maior que o informado
            /// </summary>
            public Select<TipoModel> GreaterThan(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.GreaterThan);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor maior que aos informados
            /// </summary>
            public Select<TipoModel> GreaterThan(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.GreaterThan);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrGreaterThan(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.GreaterThan);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor maior ou igual ao informado
            /// </summary>
            public Select<TipoModel> GreaterThanEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.GreaterThanEquals);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor maior ou igual aos informados
            /// </summary>
            public Select<TipoModel> GreaterThanEquals(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.GreaterThanEquals);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrGreaterThanEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.GreaterThanEquals);
            }
            #endregion

            #region LessThan
            /// <summary>
            /// Cláusula onde a propriedade possui valor menor que o informado
            /// </summary>
            public Select<TipoModel> LessThan(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.LessThan);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor menor que aos informados
            /// </summary>
            public Select<TipoModel> LessThan(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.LessThan);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrLessThan(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.LessThan);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor menor ou igual ao informado
            /// </summary>
            public Select<TipoModel> LessThanEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, tipo: TipoWhereClause.LessThanEquals);
            }

            /// <summary>
            /// Cláusula onde a propriedade possui valor menor ou igual aos informados
            /// </summary>
            public Select<TipoModel> LessThanEquals(Expression<Func<TipoModel, object>> propriedade, params object[] valores)
            {
                return WhereClause(propriedade, valores: valores, tipo: TipoWhereClause.LessThanEquals);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrLessThanEquals(Expression<Func<TipoModel, object>> propriedade, object valor)
            {
                return WhereClause(propriedade, valor, condicaoOr: true, tipo: TipoWhereClause.LessThanEquals);
            }
            #endregion

            #region IsNotNull
            /// <summary>
            /// Cláusula onde a propriedade não deverá ser nula
            /// </summary>
            public Select<TipoModel> IsNotNull(Expression<Func<TipoModel, object>> propriedade)
            {
                return WhereClause(propriedade, null, isNotNull: true);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrIsNotNull(Expression<Func<TipoModel, object>> propriedade)
            {
                return WhereClause(propriedade, null, condicaoOr: true, isNotNull: true);
            }
            #endregion

            #region IsNull
            /// <summary>
            /// Cláusula onde a propriedade deverá ser nula
            /// </summary>
            public Select<TipoModel> IsNull(Expression<Func<TipoModel, object>> propriedade)
            {
                return WhereClause(propriedade, null, isNotNull: false);
            }

            /// <summary>
            /// Cláusula condicional onde a propriedade tem que atingir a condição determinada via parametro
            /// </summary>
            public Select<TipoModel> OrIsNull(Expression<Func<TipoModel, object>> propriedade)
            {
                return WhereClause(propriedade, null, condicaoOr: true, isNotNull: false);
            }
            #endregion

            #region In
            public Select<TipoModel> In<TReference>(Expression<Func<TipoModel, object>> propriedade, Expression<Func<TReference, object>> propriedadeIn)
            {
                return In<TReference>(propriedade, propriedadeIn, false, false);
            }

            public Select<TipoModel> OrIn<TReference>(Expression<Func<TipoModel, object>> propriedade, Expression<Func<TReference, object>> propriedadeIn)
            {
                return In<TReference>(propriedade, propriedadeIn, true, false);
            }

            private Select<TipoModel> In<TReference>(Expression<Func<TipoModel, object>> propriedade, Expression<Func<TReference, object>> propriedadeIn, bool condicaoOr, bool notIn)
            {
                var expression = GetExpression(propriedade);
                var expressionIn = GetExpression(propriedadeIn);

                if (expression.HasValue && expressionIn.HasValue)
                {
                    var nomeTabela = ObterNomeTabela(typeof(TReference));

                    if (comandList.Count == 0)
                    {
                        whereList.Add("WHERE");
                    }
                    else
                    {
                        if (condicaoOr)
                        {
                            whereList.Add("OR");
                        }
                        else
                        {
                            whereList.Add("AND");
                        }
                    }

                    whereList.Add(string.Format("{0} {1} (SELECT {2} FROM {3})", expression.Value.Value, (notIn ? "NOT IN" : "IN"), expressionIn.Value.Value, nomeTabela));
                }

                return this;
            }

            public Select<TipoModel> In<TReference>(Expression<Func<TipoModel, object>> propriedade, string values)
            {
                var expression = GetExpression(propriedade);

                if (expression.HasValue)
                {
                    if (comandList.Count == 0)
                    {
                        whereList.Add("WHERE");
                    }
                    else
                    {
                        whereList.Add("AND");   
                    }

                    whereList.Add(string.Format("{0} {1} ({2})", expression.Value.Value, "IN", values));
                }

                return this;
            }
            #endregion

            #region NotIn
            public Select<TipoModel> NotIn<TReference>(Expression<Func<TipoModel, object>> propriedade, Expression<Func<TReference, object>> propriedadeIn)
            {
                return In<TReference>(propriedade, propriedadeIn, false, true);
            }

            public Select<TipoModel> OrNotIn<TReference>(Expression<Func<TipoModel, object>> propriedade, Expression<Func<TReference, object>> propriedadeIn)
            {
                return In<TReference>(propriedade, propriedadeIn, true, true);
            }
            #endregion
            #endregion
        }
    }
}