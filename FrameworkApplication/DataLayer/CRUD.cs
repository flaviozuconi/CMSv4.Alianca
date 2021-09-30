using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.DataLayer;
using System.Data;
using Framework.Model;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Reflection;

namespace Framework.Utilities
{
    public partial class CRUD
    {
        #region Obter

        public static TipoModel Obter<TipoModel>(string codigo, string connectionString = "")
        {
            return Obter<TipoModel>(codigo, null, connectionString);
        }

        public static TipoModel Obter<TipoModel>(decimal codigo, string connectionString = "")
        {
            return Obter<TipoModel>("", codigo, connectionString);
        }

        /// <summary>
        /// Obter um registro da base de dados e retorna a model preenchida
        /// </summary>
        public static TipoModel Obter<TipoModel>(string codigostr, decimal? codigo, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedades);

            var nomeTabela = ObterNomeTabela<TipoModel>();
            var modelRetorno = Activator.CreateInstance<TipoModel>();
            var sb = new StringBuilder();
            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var joinList = new List<JoinTable>();
            //int joinCount = 0;

            if (chavePrimaria.Count != 1)
                throw new ApplicationException("Este método só aceita models com chave primária única, ou a chave não está definida");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    // Campos de Join
                    var lista = (from e in propriedades
                                 where e.IsDefined(typeof(JoinField), false)
                                 select e.GetCustomAttribute<JoinField>(false)
                    ).ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        var alreadyJoined = joinList.Find(o => o.TableName.Equals(lista[i].DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                               o.DestKey.Equals(lista[i].DestKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                               o.SourceKey.Equals(lista[i].SourceKey, StringComparison.InvariantCultureIgnoreCase));

                        sbJoinFields.Append(",");

                        if (alreadyJoined == null)
                        {
                            if (!string.IsNullOrEmpty(lista[i].AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", i, lista[i].DestField, lista[i].AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", i, lista[i].DestField);
                            }

                            sbJoinTables.Append(
                            string.Format(" LEFT JOIN {0} AS t{3} ON {4}.{1} = t{3}.{2} ",
                                lista[i].DestTable,
                                lista[i].SourceKey,
                                lista[i].DestKey,
                                i,
                                nomeTabela)
                            );

                            joinList.Add(new JoinTable(lista[i].DestTable, lista[i].DestField, lista[i].SourceKey, lista[i].DestKey, i));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(lista[i].AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, lista[i].DestField, lista[i].AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, lista[i].DestField);
                            }
                        }
                    }

                    command.CommandText = string.Format(
                        "SELECT {0}.* {2} FROM {0} {3} WHERE {0}.{1}=@CRITERIO",
                        nomeTabela,
                        chavePrimaria[0].Key,
                        sbJoinFields,
                        sbJoinTables
                    );

                    // Parametros
                    if (codigo.HasValue)
                        command.NewCriteriaParameter("@CRITERIO", SqlDbType.Decimal, 18, codigo);
                    else
                        command.NewCriteriaParameter("@CRITERIO", chavePrimaria[0].Type, chavePrimaria[0].Size, codigostr);
                    // Execucao

                    var retorno = Database.ExecuteReader<TipoModel>(command);

                    if (retorno != null)
                    {
                        if (retorno.Count == 0)
                            return modelRetorno;

                        if (retorno.Count > 1)
                            throw new ApplicationException("O resultado da busca retornou mais do que uma linha de resultado");

                        return retorno[0];
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return modelRetorno;
        }

        /// <summary>
        /// Obter o registro da base de dados, com preenchimento de propriedades do tipo lista<TipoModel>
        /// </summary>
        /// <typeparam name="TipoModel"></typeparam>
        /// <param name="codigo"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static TipoModel ObterCompleto<TipoModel>(decimal codigo, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedades);
            var nomeTabela = ObterNomeTabela<TipoModel>();
            var modelRetorno = Activator.CreateInstance<TipoModel>();
            var sb = new StringBuilder();
            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var sbDataList = new StringBuilder();
            var joinList = new List<JoinTable>();
            var dataList = new List<DataList>();

            //int joinCount = 0;

            if (chavePrimaria.Count != 1)
                throw new ApplicationException("Este método só aceita models com chave primária única, ou a chave não está definida");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    // Campos de Join
                    var lista = (from e in propriedades
                                 where e.IsDefined(typeof(JoinField), false)
                                 select e.GetCustomAttribute<JoinField>(false)
                    ).ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        var alreadyJoined = joinList.Find(o => o.TableName.Equals(lista[i].DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                               o.DestKey.Equals(lista[i].DestKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                               o.SourceKey.Equals(lista[i].SourceKey, StringComparison.InvariantCultureIgnoreCase));

                        sbJoinFields.Append(",");

                        if (alreadyJoined == null)
                        {
                            if (!string.IsNullOrEmpty(lista[i].AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", i, lista[i].DestField, lista[i].AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", i, lista[i].DestField);
                            }

                            sbJoinTables.Append(
                            string.Format(" LEFT JOIN {0} AS t{3} ON {4}.{1} = t{3}.{2} ",
                                lista[i].DestTable,
                                lista[i].SourceKey,
                                lista[i].DestKey,
                                i,
                                nomeTabela)
                            );

                            joinList.Add(new JoinTable(lista[i].DestTable, lista[i].DestField, lista[i].SourceKey, lista[i].DestKey, i));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(lista[i].AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, lista[i].DestField, lista[i].AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, lista[i].DestField);
                            }
                        }
                    }

                    #region Sub Selects de Listas

                    //Encontrar as propriedades que estão com o atributo DataList
                    List<PropertyInfo> subSelects = (from e in propriedades
                                 where e.IsDefined(typeof(DataList), false)
                                 select e
                    ).ToList();

                    if(subSelects != null && subSelects.Count > 0)
                    {
                        for (int i = 0; i < subSelects.Count; i++)
                        {
                            //Obter o tipo da model utilizada na lista para obter nome da tabela e PK
                            var model = Activator.CreateInstance(subSelects[i].PropertyType.GetGenericArguments().Single());

                            //Obter a chave estrangeira que será utilizada como condição do select
                            var FK = ObterChaveEstrangeira(model.GetType(), model.GetType().GetProperties());

                            //Nome da tabela para listar as informações
                            var tabela = ObterNomeTabela(model.GetType());

                            //Concatenar as consultas
                            sbDataList.Append(string.Format(" SELECT {0}.* FROM {0} WHERE {1} = @CRITERIO", tabela, FK.Key));
                        }
                    }

                    #endregion

                    command.CommandText = string.Format(
                        "SELECT {0}.* {2} FROM {0} {3} WHERE {0}.{1}=@CRITERIO {4}",
                        nomeTabela,
                        chavePrimaria[0].Key,
                        sbJoinFields,
                        sbJoinTables,
                        sbDataList
                    );

                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.Decimal, 18, codigo);

                    var dataset = Database.ExecuteDataSet(command);

                    if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        //a model sempre será a primeira consulta
                        var retornoTeste = Activator.CreateInstance<TipoModel>();

                        //Preencher as propriedades da model
                        retornoTeste = Database.FillModel<TipoModel>(dataset.Tables[0].Rows[0]);

                        //preencher as propriedades do tipo lista
                        for (int i=1; i < dataset.Tables.Count; i++)
                        {
                            //A ordem do select retornado no datase table é o mesmo da propriedade da model
                            var item = subSelects[(i-1)];
                            var modelItem = item.PropertyType.GetGenericArguments().Single();

                            Assembly assembly = Assembly.GetExecutingAssembly();
                            var obj = assembly.CreateInstance(modelItem.FullName);

                            var prop = (from e in obj.GetType().GetProperties()
                                        where e.IsDefined(typeof(DataField), false) && e.GetCustomAttribute<DataField>(false).FillModel
                                        select new { key = e.GetCustomAttribute<DataField>(false).Name, prop = e }
                                        ).ToList();

                            var listItem = assembly.CreateInstance(item.PropertyType.FullName);

                            var listRetorno = Database.FillList(obj, dataset.Tables[i]);

                            //retornoTeste.GetType().GetProperty(item.Name).SetValue(Database.FillList(obj, dataset.Tables[i]), null);
                        }
                    }

                    var retorno = Database.ExecuteReader<TipoModel>(command);

                    if (retorno != null)
                    {
                        if (retorno.Count == 0)
                            return modelRetorno;

                        if (retorno.Count > 1)
                            throw new ApplicationException("O resultado da busca retornou mais do que uma linha de resultado");

                        return retorno[0];
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return modelRetorno;
        }

        #endregion

        #region Listar

        public static TipoModel Obter<TipoModel>(TipoModel criterios, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var lista = Listar<TipoModel>(criterios, 1, string.Empty, string.Empty, connectionString, false);
            if (lista.Count > 0) return lista[0];

            return default(TipoModel);
        }

        public static TipoModel Obter<TipoModel>(TipoModel criterios, bool useSubSelect, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var lista = Listar<TipoModel>(criterios, 1, string.Empty, string.Empty, connectionString, false, useSubSelect);
            if (lista.Count > 0) return lista[0];

            return default(TipoModel);
        }

        public static List<TipoModel> Listar<TipoModel>(string connectionString = "")
        {
            return Listar<TipoModel>(Activator.CreateInstance<TipoModel>(), null, string.Empty, string.Empty, connectionString);
        }


        /// <summary>
        /// Listar da base de dados com cache, o nome do cache é baseado nos valores da propriedade da model.
        /// </summary>
        /// <typeparam name="TipoModel"></typeparam>
        /// <param name="criterios">Model com os criterios de busca</param>
        /// <param name="connectionString">Chave de conexão com a base de dados, se for nulo, utiliza Default</param>
        /// <param name="tempoHoras">Tempo em horas até a renovação do cache</param>
        /// <returns>List<T></returns>
        /// <user>rvissontai</user>
        public static List<TipoModel> ListarCache<TipoModel>(TipoModel criterios, string connectionString, int tempoHoras = 4)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                connectionString = ApplicationSettings.ConnectionStrings.Default;

            //Ler o nome da tabela na model
            var nomeTabela = ObterNomeTabela<TipoModel>();

            //Obter todas as propriedades do objeto
            var propriedades = typeof(TipoModel).GetProperties();

            //Nome da chave do cache
            var cachedKey = nomeTabela;

            //Utilizar os valores da propriedade da model como parametro para o cache
            foreach (var item in propriedades)
            {
                var value = item.GetValue(criterios, null);

                if (value != null)
                    cachedKey += string.Concat("_", value);
            }

            //Verificar se já existe cache
            var retorno = BLCachePortal.Get<List<TipoModel>>(cachedKey);

            if (retorno != null)
                return retorno;

            //Listar os itens de critério e adicionar no cache por 4 horas.
            retorno = Listar<TipoModel>(criterios, connectionString);

            BLCachePortal.Add(cachedKey, retorno, tempoHoras);

            return retorno;
        }

        public static List<TipoModel> ListarCache<TipoModel>(TipoModel criterios, int tempo = 4)
        {
            return ListarCache<TipoModel>(criterios, string.Empty, tempo);
        }

        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, string connectionString = "")
        {
            return Listar<TipoModel>(criterios, null, string.Empty, string.Empty, connectionString, true);
        }

        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, string orderBy, string sortOrder, string connectionString = "")
        {
            return Listar<TipoModel>(criterios, null, orderBy, sortOrder, connectionString);
        }

        /// <summary>
        /// Lista os elementos da base de dados e retorna model preenchida
        /// </summary>
        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, int? top, string orderBy, string sortOrder, string connectionString = "", bool useLikeOnSearch = true, bool useSubSelect = false)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var nomeTabela = ObterNomeTabela<TipoModel>();
            var tiposTexto = new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Text, SqlDbType.NVarChar, SqlDbType.Char };
            var listaRetorno = new List<TipoModel>();
            var sbWhere = new StringBuilder();
            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var joinList = new List<JoinTable>();
            int joinCount = 0;
            int criterioCount = 0;

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    // Procura as informações em [row] conforme as propriedades da [model]
                    var lista = (from e in propriedades
                                 where e.IsDefined(typeof(DataField), false)
                                 select new
                                 {
                                     dataField = e.GetCustomAttribute<DataField>(false),
                                     joinField = e.GetCustomAttribute<JoinField>(false),
                                     value = e.GetValue(criterios, null)
                                 }
                    ).ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        #region JoinField
                        if (lista[i].joinField != null)
                        {
                            var alreadyJoined = joinList.Find(o => o.TableName.Equals(lista[i].joinField.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.DestKey.Equals(lista[i].joinField.DestKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.SourceKey.Equals(lista[i].joinField.SourceKey, StringComparison.InvariantCultureIgnoreCase));

                            sbJoinFields.Append(",");

                            if (alreadyJoined == null)
                            {
                                joinCount++;

                                if (!string.IsNullOrEmpty(lista[i].joinField.AliasName))
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1} {2}", joinCount, lista[i].joinField.DestField, lista[i].joinField.AliasName);
                                }
                                else
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1}", joinCount, lista[i].joinField.DestField);
                                }

                                sbJoinTables.Append(
                                string.Format(" LEFT JOIN {0} AS t{3} ON {4}.{1} = t{3}.{2} ",
                                    lista[i].joinField.DestTable,
                                    lista[i].joinField.SourceKey,
                                    lista[i].joinField.DestKey,
                                    joinCount,
                                    nomeTabela)
                                );
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(lista[i].joinField.AliasName))
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, lista[i].joinField.DestField, lista[i].joinField.AliasName);
                                }
                                else
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, lista[i].joinField.DestField);
                                }
                            }

                            joinList.Add(new JoinTable(lista[i].joinField.DestTable, lista[i].joinField.DestField, lista[i].joinField.SourceKey, lista[i].joinField.DestKey, joinCount));
                        }
                        #endregion

                        #region DataField
                        if (lista[i].value != null)
                        {
                            if (criterioCount == 0) sbWhere.Append(" WHERE ");
                            else sbWhere.Append(" AND ");

                            criterioCount++;

                            var joinField = joinList.Find(a =>
                                lista[i].joinField != null &&
                                a.TableName.Equals(lista[i].joinField.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                a.ColumnName.Equals(lista[i].dataField.Name, StringComparison.InvariantCultureIgnoreCase));

                            // Parametros
                            if (useLikeOnSearch && Array.IndexOf(tiposTexto, lista[i].dataField.Type) > -1)
                            {
                                if (joinField != null)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", string.Concat("t", joinField.Alias), lista[i].dataField.Name, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", nomeTabela, lista[i].dataField.Name, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", lista[i].value));
                            }
                            else
                            {
                                if (joinField != null)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} = @C{2}", string.Concat("t", joinField.Alias), lista[i].dataField.Name, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("{0}.{1} = @C{2}", nomeTabela, lista[i].dataField.Name, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), lista[i].value);
                            }
                        }
                        #endregion
                    }

                    // Campos de Sub Select
                    var subSelectList = new List<string>();
                        
                    if(useSubSelect)
                    {
                        subSelectList = 
                            (from e in propriedades
                                    where e.IsDefined(typeof(SubSelectField), false)
                                    select string.Format(",({0}) {1}", e.GetCustomAttribute<SubSelectField>(false).QuerySql,
                                        e.GetCustomAttribute<SubSelectField>(false).Coluna)
                            ).ToList();
                    }
                        
                        

                    if (!string.IsNullOrEmpty(sortOrder) &&
                        sortOrder.IndexOf("ASC", StringComparison.InvariantCultureIgnoreCase) == -1 &&
                        sortOrder.IndexOf("DESC", StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        sortOrder = string.Empty;
                    }
                    else
                    {
                        sortOrder = string.Concat(" ", sortOrder);
                    }

                    command.CommandText = string.Format(
                        @"  SELECT {0} {1}.* {2} {7}
                            FROM {1}
                            {3}
                            {4}
                            {5} {6}
                        ",
                        top.HasValue ? string.Format(" TOP {0} ", top) : "",
                        nomeTabela,
                        sbJoinFields.ToString(),
                        sbJoinTables.ToString(),
                        sbWhere.ToString(),
                        !string.IsNullOrEmpty(orderBy) ? string.Format(" ORDER BY {0} ", ObterNomeCampo<TipoModel>(orderBy, propriedades)) : "",
                        !string.IsNullOrEmpty(sortOrder) ? sortOrder : "",
                        string.Join("", subSelectList)
                    );

                    var dataSet = Database.ExecuteDataSet(command);

                    if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        return Database.FillList<TipoModel>(dataSet.Tables[0]);
                    }
                    else
                    {
                        return listaRetorno;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
               throw;
            }
        }

        #endregion

        #region Listar com busca generica
        /// <summary>
        /// Lista os elementos da base de dados e retorna model preenchida
        /// </summary>
        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, NameValueCollection request, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedadesModel = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedadesModel);
            var nomeTabela = ObterNomeTabela<TipoModel>();

            // Criterios
            var buscaGenerica = request["search[value]"];
            var listColunasBuscaGenerica = new List<string>();

            // Busca Generica
            var getColuna = "";
            int getIndex = 0;

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            do
            {
                getColuna = request[string.Format("columns[{0}][data]", getIndex)];

                if (!string.IsNullOrWhiteSpace(getColuna) && getColuna != "0")
                {
                    if (request[string.Format("columns[{0}][searchable]", getIndex)] == "true") listColunasBuscaGenerica.Add(getColuna);
                }

                getIndex += 1;
            } while (!string.IsNullOrWhiteSpace(getColuna));

            // Ordenacao
            getColuna = "";
            getIndex = 0;
            var orderBy = "";

            do
            {
                getColuna = request[string.Format("order[{0}][column]", getIndex)];

                if (!string.IsNullOrWhiteSpace(getColuna))
                {
                    var ordering = request[string.Format("columns[{0}][data]", getColuna)];
                    var nomeCampo = ObterNomeCampo<TipoModel>(ordering, propriedadesModel);

                    if (!string.IsNullOrEmpty(ordering) && !string.IsNullOrEmpty(nomeCampo) && ordering != "0")
                    {
                        if (!string.IsNullOrEmpty(orderBy))
                        {
                            orderBy = string.Concat(orderBy, ", ", nomeCampo, " ", request[string.Format("order[{0}][dir]", getIndex)]);
                        }
                        else
                        {
                            orderBy = string.Concat(nomeCampo, " ", request[string.Format("order[{0}][dir]", getIndex)]);
                        }
                    }
                }

                getIndex += 1;

            } while (!string.IsNullOrWhiteSpace(getColuna));

            if (string.IsNullOrEmpty(orderBy))
            {
                foreach (var item in chavePrimaria)
                    orderBy = string.Concat(orderBy, " ", item.Key, " ASC,");

                orderBy = orderBy.Trim().TrimEnd(',');
            }

            // Montagem
            var sbWhere = new StringBuilder();
            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var joinList = new List<JoinTable>();
            var selectList = new List<String>();

            int joinCount = 0;

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    // Procura as informações em [row] conforme as propriedades da [model]
                    int criterioCount = 0;

                    var tiposTexto = new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Text, SqlDbType.NVarChar, SqlDbType.Char };

                    //Campos de Dados
                    var propriedadesFiltrada = (from e in propriedadesModel
                                                where e.IsDefined(typeof(DataField), false)
                                                select new
                                                {
                                                    dataField = e.GetCustomAttribute<DataField>(false),
                                                    joinField = e.GetCustomAttribute<JoinField>(false),
                                                    whereField = e.GetCustomAttribute<WhereField>(false),
                                                    prop = e
                                                }).ToList();

                    // Campos de Join
                    var propriedadesJoin = (from e in propriedadesModel
                                            where e.IsDefined(typeof(JoinField), false)
                                            select e.GetCustomAttribute<JoinField>(false)
                                        ).ToList();

                    // Campos de Sub Select
                    var subSelectList = (from e in propriedadesModel
                                         where e.IsDefined(typeof(SubSelectField), false)
                                         select string.Format(",({0}) {1}", e.GetCustomAttribute<SubSelectField>(false).QuerySql,
                                                e.GetCustomAttribute<SubSelectField>(false).Coluna)
                                    ).ToList();

                    #region propriedadesJoin
                    for (int i = 0; i < propriedadesJoin.Count; i++)
                    {
                        var joinItem = propriedadesJoin[i];
                        var alreadyJoined = joinList.Find(o => o.TableName.Equals(joinItem.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.SourceKey.Equals(joinItem.SourceKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.DestKey.Equals(joinItem.DestKey, StringComparison.InvariantCultureIgnoreCase));

                        sbJoinFields.Append(",");

                        if (alreadyJoined == null)
                        {
                            joinCount++;

                            if (!string.IsNullOrEmpty(joinItem.AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", joinCount, joinItem.DestField, joinItem.AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", joinCount, joinItem.DestField);
                            }

                            sbJoinTables.Append(
                                string.Format(" LEFT JOIN {0} AS t{3} ON principal.{1} = t{3}.{2} ",
                                    joinItem.DestTable,
                                    joinItem.SourceKey,
                                    joinItem.DestKey,
                                    joinCount
                                )
                            );

                            joinList.Add(new JoinTable(joinItem.DestTable, joinItem.DestField, joinItem.SourceKey, joinItem.DestKey, joinCount));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(joinItem.AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, joinItem.DestField, joinItem.AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, joinItem.DestField);
                            }

                            //A tabela já receber left join porém falta adicionar o campo à lista de joins para uso posterior
                            if (!alreadyJoined.ColumnName.Equals(joinItem.DestField, StringComparison.InvariantCultureIgnoreCase))
                            {
                                joinList.Add(new JoinTable(joinItem.DestTable, joinItem.DestField, joinItem.SourceKey, joinItem.DestKey, joinCount));
                            }
                        }
                    }
                    #endregion

                    #region buscaGenerica
                    for (int i = 0; i < propriedadesFiltrada.Count; i++)
                    {
                        var dataField = propriedadesFiltrada[i].dataField;

                        if (!string.IsNullOrEmpty(buscaGenerica) && listColunasBuscaGenerica.Count > 0)
                        {
                            if (listColunasBuscaGenerica.Exists(a => a.Equals(propriedadesFiltrada[i].prop.Name, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (criterioCount == 0) sbWhere.Append(" WHERE (");
                                else sbWhere.Append(" OR ");

                                criterioCount++;

                                var joinFind = joinList.Find(a => a.ColumnName.Equals(dataField.Name, StringComparison.InvariantCultureIgnoreCase));
                                string alias = joinFind != null ? string.Concat("t", joinFind.Alias) : "principal";
                                string nomeColuna = propriedadesFiltrada[i].joinField != null ? propriedadesFiltrada[i].joinField.DestField : dataField.Name;

                                // Parametros
                                if (Array.IndexOf(tiposTexto, dataField.Type) > -1)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", alias, nomeColuna, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("CONVERT(VARCHAR, {0}.{1}) LIKE @C{2}", alias, nomeColuna, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", buscaGenerica));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(buscaGenerica) && listColunasBuscaGenerica.Count > 0)
                    {
                        sbWhere.Append(" ) ");
                    }
                    #endregion

                    #region propriedadesFiltrada
                    for (int i = 0; i < propriedadesFiltrada.Count; i++)
                    {
                        var dataField = propriedadesFiltrada[i].dataField;
                        var valor = propriedadesFiltrada[i].prop.GetValue(criterios, null);

                        if (valor != null)
                        {
                            if (criterioCount == 0) sbWhere.Append(" WHERE ");
                            else sbWhere.Append(" AND ");

                            criterioCount++;

                            // Parametros
                            var condition = WhereCondition.EqualsOrLike;

                            if (propriedadesFiltrada[i].whereField != null)
                            {
                                condition = propriedadesFiltrada[i].whereField.Condition;
                            }

                            if (condition == WhereCondition.In ||
                                condition == WhereCondition.NotIn ||
                                condition == WhereCondition.ListInList)
                            {
                                JoinTable joinFind = joinList.Find(a => a.ColumnName.Equals(dataField.Name, StringComparison.InvariantCultureIgnoreCase));
                                string alias = joinFind != null ? string.Concat("t", joinFind.Alias) : "principal";
                                string nomeColuna = propriedadesFiltrada[i].joinField != null ? propriedadesFiltrada[i].joinField.DestField : dataField.Name;

                                var isIn = condition == WhereCondition.In || condition == WhereCondition.ListInList;

                                if (condition == WhereCondition.ListInList || condition == WhereCondition.ListNotInList)
                                    sbWhere.Append(string.Format("(EXISTS((SELECT ITEM FROM DBO.SPLITTABLE({2}.{3}) WHERE ITEM IS NOT NULL AND ITEM {0} (SELECT ITEM FROM DBO.SPLITTABLE(@C{1})))) OR @C{1} IS NULL)", (isIn ? "IN" : "NOT IN"), criterioCount, alias, nomeColuna));
                                else
                                    sbWhere.Append(string.Format("({2}.{3} {0} (SELECT ITEM FROM DBO.SPLITTABLE(@C{1})) OR @C{1} IS NULL)", (isIn ? "IN" : "NOT IN"), criterioCount, alias, nomeColuna));

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), valor);
                            }
                            else
                            {
                                if (Array.IndexOf(tiposTexto, dataField.Type) > -1)
                                {
                                    sbWhere.Append(string.Format("principal.{0} LIKE @C{1}", dataField.Name, criterioCount));
                                    command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", valor));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("principal.{0} = @C{1}", dataField.Name, criterioCount));
                                    command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), valor);
                                }
                            }
                        }

                        if (dataField.Busca &&
                            subSelectList.Find(a => a.EndsWith(dataField.Name)) == null)
                        {
                            selectList.Add(string.Concat("principal.", dataField.Name));
                        }
                    }
                    #endregion

                    #region definir alias em Order By
                    var joinOrderBy = joinList.Find(a => a.ColumnName.EndsWith(orderBy.Substring(0, orderBy.IndexOf(" ")), StringComparison.InvariantCultureIgnoreCase));

                    if (joinOrderBy != null)
                    {
                        orderBy = string.Concat("t", joinOrderBy.Alias, ".", orderBy);
                    }
                    else
                    {
                        orderBy = string.Concat("principal.", orderBy);
                    }
                    #endregion

                    //por questão de performance, foi substituido o sql_total anterior por select count da tabela via subselect
                    //antes:
                    //sql_total = COUNT(1) OVER ()
                    command.CommandText = string.Format(
                        @"SELECT {6} {5} {3}
                            FROM {0} AS principal
                            {4}
                            {2}
                            ORDER BY {1}",
                        nomeTabela, //0
                        orderBy, //1
                        sbWhere.ToString(), //2
                        sbJoinFields.ToString(), //3
                        sbJoinTables.ToString(), //4
                        string.Join("", subSelectList), //5
                        string.Join(",", selectList) //6
                    );

                    // Execucao
                    var dataSet = Database.ExecuteDataSet(command);



                    if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        return Database.FillList<TipoModel>(dataSet.Tables[0]);
                    }

                    return new List<TipoModel>();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Listar Paginado

        /// <summary>
        /// Lista os elementos da base de dados e retorna model preenchida
        /// </summary>
        /// <param name="pagina">O valor inicial de página é 1</param>
        /// <param name="retornoTotalRegistros">Retorna o total de registros com esses critérios</param>
        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, string orderBy, string sortOrder, int pagina, double quantidade, out double retornoTotalRegistros, string buscaGenerica, string[] camposBuscaGenerica, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedades);
            var nomeTabela = ObterNomeTabela<TipoModel>();
            var tiposTexto = new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Text, SqlDbType.NVarChar, SqlDbType.Char };
            var listaRetorno = new List<TipoModel>();
            var sbWhere = new StringBuilder();
            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var joinList = new List<JoinTable>();
            int joinCount = 0;

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            if (!string.IsNullOrEmpty(orderBy)) orderBy = ObterNomeCampo<TipoModel>(orderBy, propriedades);
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = string.Join(",", chavePrimaria.Select(a => a.Key));
            }

            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "ASC";

            if (pagina > 0) pagina = pagina - 1;
            else pagina = 0;

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    int criterioCount = 0;

                    // Procura as informações em [row] conforme as propriedades da [model]
                    var lista = (from e in propriedades
                                 where e.IsDefined(typeof(DataField), false)
                                 select new
                                 {
                                     dataField = e.GetCustomAttribute<DataField>(false),
                                     joinField = e.GetCustomAttribute<JoinField>(false),
                                     value = e.GetValue(criterios, null)
                                 }
                    ).ToList();

                    if (!string.IsNullOrEmpty(buscaGenerica) && camposBuscaGenerica != null && camposBuscaGenerica.Length > 0)
                    {
                        for (int i = 0; i < lista.Count; i++)
                        {
                            if (camposBuscaGenerica.Select(a => a.ToLower()).Contains(lista[i].dataField.Name.ToLower()))
                            {
                                if (criterioCount == 0) sbWhere.Append(" WHERE (");
                                else sbWhere.Append(" OR ");

                                criterioCount++;

                                // Parametros
                                if (Array.IndexOf(tiposTexto, lista[i].dataField.Type) > -1)
                                {
                                    sbWhere.Append(string.Format("{0} LIKE @C{1}", lista[i].dataField.Name, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("CONVERT(VARCHAR, {0}) LIKE @C{1}", lista[i].dataField.Name, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", buscaGenerica));
                            }
                        }

                        sbWhere.Append(" ) ");
                    }

                    for (int i = 0; i < lista.Count; i++)
                    {
                        #region JoinField
                        if (lista[i].joinField != null)
                        {
                            var alreadyJoined = joinList.Find(o => o.TableName.Equals(lista[i].joinField.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.SourceKey.Equals(lista[i].joinField.SourceKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.DestKey.Equals(lista[i].joinField.DestKey, StringComparison.InvariantCultureIgnoreCase));

                            sbJoinFields.Append(",");

                            if (alreadyJoined == null)
                            {
                                joinCount++;

                                if (!string.IsNullOrEmpty(lista[i].joinField.AliasName))
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1} {2}", joinCount, lista[i].joinField.DestField, lista[i].joinField.AliasName);
                                }
                                else
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1}", joinCount, lista[i].joinField.DestField);
                                }

                                sbJoinTables.Append(
                                    string.Format(" LEFT JOIN {0} AS t{3} ON {4}.{1} = t{3}.{2} ",
                                    lista[i].joinField.DestTable,
                                    lista[i].joinField.SourceKey,
                                    lista[i].joinField.DestKey,
                                    joinCount,
                                    "principal") //nomeTabela)
                                );
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(lista[i].joinField.AliasName))
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, lista[i].joinField.DestField, lista[i].joinField.AliasName);
                                }
                                else
                                {
                                    sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, lista[i].joinField.DestField);
                                }
                            }

                            joinList.Add(new JoinTable(lista[i].joinField.DestTable, lista[i].joinField.DestField, lista[i].joinField.SourceKey, lista[i].joinField.DestKey, joinCount));
                        }
                        #endregion

                        #region DataField
                        if (lista[i].value != null)
                        {
                            if (criterioCount == 0) sbWhere.Append(" WHERE ");
                            else sbWhere.Append(" AND ");

                            criterioCount++;

                            var joinField = joinList.Find(a =>
                                lista[i].joinField != null &&
                                a.TableName.Equals(lista[i].joinField.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                a.ColumnName.Equals(lista[i].dataField.Name, StringComparison.InvariantCultureIgnoreCase));

                            // Parametros
                            if (Array.IndexOf(tiposTexto, lista[i].dataField.Type) > -1)
                            {
                                if (joinField != null)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", string.Concat("t", joinField.Alias), lista[i].dataField.Name, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", "principal", lista[i].dataField.Name, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", lista[i].value));
                            }
                            else
                            {
                                if (joinField != null)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} = @C{2}", string.Concat("t", joinField.Alias), lista[i].dataField.Name, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("{0}.{1} = @C{2}", "principal", lista[i].dataField.Name, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), lista[i].value);
                            }
                        }
                        #endregion
                    }

                    //por questão de performance, foi substituido o sql_total anterior por select count da tabela via subselect
                    //antes:
                    //sql_total = COUNT(1) OVER ()
                    command.CommandText = string.Format(
                        @"WITH tabela AS
                        (
                            SELECT principal.* {6},
                            sql_indice = ROW_NUMBER() OVER (ORDER BY {1} {2})
                            FROM {0} AS principal
                            {7}
                            {5}
                        )
                        SELECT tabela.*, sql_total = (SELECT COUNT(1) FROM tabela)
                        FROM tabela
                        WHERE sql_indice BETWEEN {3} AND {4}
                        ORDER BY {1} {2}",
                        nomeTabela, //0
                        orderBy, //1
                        sortOrder, //2
                        pagina * quantidade + 1, //3
                        (pagina * quantidade) + quantidade, //4
                        sbWhere.ToString(), //5
                        sbJoinFields.ToString(), //6
                        sbJoinTables.ToString() //7
                    );

                    // Execucao

                    var dataSet = Database.ExecuteDataSet(command);


                    if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        retornoTotalRegistros = Convert.ToInt32(dataSet.Tables[0].Rows[0]["sql_total"]);

                        return Database.FillList<TipoModel>(dataSet.Tables[0]);
                    }
                    else
                    {
                        retornoTotalRegistros = 0;

                        return listaRetorno;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Listar Paginado com busca generica (GRID)

        /// <summary>
        /// Lista os elementos da base de dados e retorna model preenchida
        /// </summary>
        /// <param name="pagina">O valor inicial de página é 1</param>
        /// <param name="retornoTotalRegistros">Retorna o total de registros com esses critérios</param>
        public static List<TipoModel> Listar<TipoModel>(TipoModel criterios, NameValueCollection request, out double retornoTotalRegistros, string connectionString = "", Dictionary<string, string> criteriosAdicionais = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedadesModel = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedadesModel);
            var nomeTabela = ObterNomeTabela<TipoModel>();

            // Criterios
            var buscaGenerica = request["search[value]"];
            var listColunasBuscaGenerica = new List<string>();

            // Busca Generica
            var getColuna = "";
            int getIndex = 0;

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            do
            {
                getColuna = request[String.Format("columns[{0}][data]", getIndex)];

                if (!String.IsNullOrWhiteSpace(getColuna) && getColuna != "0")
                {
                    if (request[String.Format("columns[{0}][searchable]", getIndex)] == "true") listColunasBuscaGenerica.Add(getColuna);
                }

                getIndex += 1;
            } while (!String.IsNullOrWhiteSpace(getColuna));

            // Ordenacao
            getColuna = "";
            getIndex = 0;
            var orderBy = "";

            do
            {
                getColuna = request[string.Format("order[{0}][column]", getIndex)];

                if (!string.IsNullOrWhiteSpace(getColuna))
                {
                    var ordering = request[string.Format("columns[{0}][data]", getColuna)];
                    var nomeCampo = ObterNomeCampo<TipoModel>(ordering, propriedadesModel);

                    if (!string.IsNullOrEmpty(ordering) && !string.IsNullOrEmpty(nomeCampo) && ordering != "0")
                    {
                        if (!string.IsNullOrEmpty(orderBy))
                        {
                            orderBy = string.Concat(orderBy, ", ", nomeCampo, " ", request[string.Format("order[{0}][dir]", getIndex)]);
                        }
                        else
                        {
                            orderBy = string.Concat(nomeCampo, " ", request[string.Format("order[{0}][dir]", getIndex)]);
                        }
                    }
                }

                getIndex += 1;

            } while (!string.IsNullOrWhiteSpace(getColuna));

            if (string.IsNullOrEmpty(orderBy))
            {
                foreach (var item in chavePrimaria)
                    orderBy = string.Concat(orderBy, " ", item.Key, " ASC,");

                orderBy = orderBy.Trim().TrimEnd(',');
            }

            // Pagina
            int start;
            int length;
            int.TryParse(request["start"], out start);
            int.TryParse(request["length"], out length);

            if (length == 0) length = 10;

            // Montagem
            var listaRetorno = new List<TipoModel>();
            var sbWhere = new StringBuilder();

            var sbJoinFields = new StringBuilder();
            var sbJoinTables = new StringBuilder();
            var joinList = new List<JoinTable>();
            var selectList = new List<String>();

            int joinCount = 0;

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    // Procura as informações em [row] conforme as propriedades da [model]
                    int criterioCount = 0;

                    var tiposTexto = new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Text, SqlDbType.NVarChar, SqlDbType.Char };

                    //Campos de Dados
                    var propriedadesFiltrada = (from e in propriedadesModel
                                                where e.IsDefined(typeof(DataField), false)
                                                select new
                                                {
                                                    dataField = e.GetCustomAttribute<DataField>(false),
                                                    joinField = e.GetCustomAttribute<JoinField>(false),
                                                    whereField = e.GetCustomAttribute<WhereField>(false),
                                                    prop = e
                                                }).ToList();

                    // Campos de Join
                    var propriedadesJoin = (from e in propriedadesModel
                                            where e.IsDefined(typeof(JoinField), false)
                                            select e.GetCustomAttribute<JoinField>(false)
                                        ).ToList();

                    // Campos de Sub Select
                    var subSelectList = (from e in propriedadesModel
                                         where e.IsDefined(typeof(SubSelectField), false)
                                         select string.Format(",({0}) {1}", e.GetCustomAttribute<SubSelectField>(false).QuerySql,
                                                e.GetCustomAttribute<SubSelectField>(false).Coluna)
                                    ).ToList();

                    #region propriedadesJoin
                    for (int i = 0; i < propriedadesJoin.Count; i++)
                    {
                        var joinItem = propriedadesJoin[i];
                        var alreadyJoined = joinList.Find(o => o.TableName.Equals(joinItem.DestTable, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.SourceKey.Equals(joinItem.SourceKey, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    o.DestKey.Equals(joinItem.DestKey, StringComparison.InvariantCultureIgnoreCase));

                        sbJoinFields.Append(",");

                        if (alreadyJoined == null)
                        {
                            joinCount++;

                            if (!string.IsNullOrEmpty(joinItem.AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", joinCount, joinItem.DestField, joinItem.AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", joinCount, joinItem.DestField);
                            }

                            sbJoinTables.Append(
                                string.Format(" LEFT JOIN {0} AS t{3} ON principal.{1} = t{3}.{2} ",
                                    joinItem.DestTable,
                                    joinItem.SourceKey,
                                    joinItem.DestKey,
                                    joinCount,
                                    "principal" //nomeTabela)
                                )
                            );

                            joinList.Add(new JoinTable(joinItem.DestTable, joinItem.DestField, joinItem.SourceKey, joinItem.DestKey, joinCount));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(joinItem.AliasName))
                            {
                                sbJoinFields.AppendFormat("t{0}.{1} {2}", alreadyJoined.Alias, joinItem.DestField, joinItem.AliasName);
                            }
                            else
                            {
                                sbJoinFields.AppendFormat("t{0}.{1}", alreadyJoined.Alias, joinItem.DestField);
                            }

                            //A tabela já receber left join porém falta adicionar o campo à lista de joins para uso posterior
                            if (!alreadyJoined.ColumnName.Equals(joinItem.DestField, StringComparison.InvariantCultureIgnoreCase))
                            {
                                joinList.Add(new JoinTable(joinItem.DestTable, joinItem.DestField, joinItem.SourceKey, joinItem.DestKey, joinCount));
                            }
                        }
                    }
                    #endregion

                    #region buscaGenerica
                    for (int i = 0; i < propriedadesFiltrada.Count; i++)
                    {
                        var dataField = propriedadesFiltrada[i].dataField;

                        if (!string.IsNullOrEmpty(buscaGenerica) && listColunasBuscaGenerica.Count > 0)
                        {
                            if (listColunasBuscaGenerica.Exists(a => a.Equals(propriedadesFiltrada[i].prop.Name, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (criterioCount == 0) sbWhere.Append(" WHERE (");
                                else sbWhere.Append(" OR ");

                                criterioCount++;

                                var joinFind = joinList.Find(a => a.ColumnName.Equals(dataField.Name, StringComparison.InvariantCultureIgnoreCase));
                                string alias = joinFind != null ? string.Concat("t", joinFind.Alias) : "principal";
                                string nomeColuna = propriedadesFiltrada[i].joinField != null ? propriedadesFiltrada[i].joinField.DestField : dataField.Name;

                                // Parametros
                                if (Array.IndexOf(tiposTexto, dataField.Type) > -1)
                                {
                                    sbWhere.Append(string.Format("{0}.{1} LIKE @C{2}", alias, nomeColuna, criterioCount));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("CONVERT(VARCHAR, {0}.{1}) LIKE @C{2}", alias, nomeColuna, criterioCount));
                                }

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", buscaGenerica));
                            }
                        }
                    }

                    if(criteriosAdicionais != null && criteriosAdicionais.Count > 0)
                        foreach(var item in criteriosAdicionais)
                            command.NewCriteriaParameter(item.Key, item.Value);

                    if (!string.IsNullOrEmpty(buscaGenerica) && listColunasBuscaGenerica.Count > 0)
                    {
                        sbWhere.Append(" ) ");
                    }
                    #endregion

                    #region propriedadesFiltrada
                    for (int i = 0; i < propriedadesFiltrada.Count; i++)
                    {
                        var dataField = propriedadesFiltrada[i].dataField;
                        var valor = propriedadesFiltrada[i].prop.GetValue(criterios, null);

                        if (valor != null)
                        {
                            if (criterioCount == 0) sbWhere.Append(" WHERE ");
                            else sbWhere.Append(" AND ");

                            criterioCount++;

                            // Parametros
                            var condition = WhereCondition.EqualsOrLike;

                            if (propriedadesFiltrada[i].whereField != null)
                            {
                                condition = propriedadesFiltrada[i].whereField.Condition;
                            }

                            if (condition == WhereCondition.In ||
                                condition == WhereCondition.NotIn ||
                                condition == WhereCondition.ListInList)
                            {
                                JoinTable joinFind = joinList.Find(a => a.ColumnName.Equals(dataField.Name, StringComparison.InvariantCultureIgnoreCase));
                                string alias = joinFind != null ? string.Concat("t", joinFind.Alias) : "principal";
                                string nomeColuna = propriedadesFiltrada[i].joinField != null ? propriedadesFiltrada[i].joinField.DestField : dataField.Name;

                                var isIn = condition == WhereCondition.In || condition == WhereCondition.ListInList;

                                if (condition == WhereCondition.ListInList || condition == WhereCondition.ListNotInList)
                                    sbWhere.Append(string.Format("(EXISTS((SELECT ITEM FROM DBO.SPLITTABLE({2}.{3}) WHERE ITEM IS NOT NULL AND ITEM {0} (SELECT ITEM FROM DBO.SPLITTABLE(@C{1})))) OR @C{1} IS NULL)", (isIn ? "IN" : "NOT IN"), criterioCount, alias, nomeColuna));
                                else
                                    sbWhere.Append(string.Format("({2}.{3} {0} (SELECT ITEM FROM DBO.SPLITTABLE(@C{1})) OR @C{1} IS NULL)", (isIn ? "IN" : "NOT IN"), criterioCount, alias, nomeColuna));

                                command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), valor);
                            }
                            else
                            {
                                JoinTable joinFind = joinList.Find(a => a.ColumnName.Equals(dataField.Name, StringComparison.InvariantCultureIgnoreCase));

                                if (joinFind != null && string.Concat("t", joinFind.Alias).Equals("t1"))
                                {
                                    sbWhere.Append(string.Format("t1.{0} = @C{1}", dataField.Name, criterioCount));
                                    command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), valor);
                                }
                                else if (Array.IndexOf(tiposTexto, dataField.Type) > -1)
                                {
                                    sbWhere.Append(string.Format("principal.{0} LIKE @C{1}", dataField.Name, criterioCount));
                                    command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), string.Format("%{0}%", valor));
                                }
                                else
                                {
                                    sbWhere.Append(string.Format("principal.{0} = @C{1}", dataField.Name, criterioCount));
                                    command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), valor);
                                }
                            }
                        }

                        if (dataField.Busca &&
                            subSelectList.Find(a => a.EndsWith(dataField.Name)) == null)
                        {
                            selectList.Add(string.Concat("tabela.", dataField.Name));
                        }
                    }
                    #endregion

                    #region definir alias em Order By
                    var joinOrderBy = joinList.Find(a => a.ColumnName.EndsWith(orderBy.Substring(0, orderBy.IndexOf(" ")), StringComparison.InvariantCultureIgnoreCase));

                    if (joinOrderBy != null)
                    {
                        orderBy = string.Concat("t", joinOrderBy.Alias, ".", orderBy);
                    }
                    else
                    {
                        orderBy = string.Concat("principal.", orderBy);
                    }
                    #endregion

                    //por questão de performance, foi substituido o sql_total anterior por select count da tabela via subselect
                    //antes:
                    //sql_total = COUNT(1) OVER ()
                    command.CommandText = string.Format(
                        @"WITH tabela AS
                        (
                            SELECT principal.* {6}, 
                            sql_indice = ROW_NUMBER() OVER (ORDER BY {1} {2})
                            FROM {0} AS principal
                            {7}
                            {5}
                        )
                        SELECT DISTINCT {9} {8}, sql_indice, sql_total = (SELECT COUNT(1) FROM tabela)
                        FROM tabela
                        WHERE sql_indice BETWEEN {3} AND {4}",
                        nomeTabela, //0
                        orderBy, //1
                        "", //2
                        start + 1, //3
                        start + length, //4
                        sbWhere.ToString(), //5
                        sbJoinFields.ToString(), //6
                        sbJoinTables.ToString(), //7
                        string.Join("", subSelectList), //8
                        string.Join(",", selectList) //9
                    );

                    // Execucao
                    var dataSet = Database.ExecuteDataSet(command);


                    if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        retornoTotalRegistros = Convert.ToInt32(dataSet.Tables[0].Rows[0]["sql_total"]);

                        return Database.FillList<TipoModel>(dataSet.Tables[0]);
                    }
                    else
                    {
                        retornoTotalRegistros = 0;
                        return listaRetorno;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Listar Json

        public static JsonResult ListarJson<TipoModel>(TipoModel criterios, NameValueCollection request, string connectionString = "", Dictionary<string, string> criteriosAdicionais = null)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            double total;
            var lista = Listar<TipoModel>(criterios, request, out total, connectionString, criteriosAdicionais);

            // Retorna os resultados
            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = lista
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 50000000
            };

        }

        #endregion

        #region Excluir

        /// <summary>
        /// Exclui um registro da base de dados
        /// </summary>
        public static int Excluir<TipoModel>(decimal codigo, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var chavePrimaria = ObterChavePrimaria<TipoModel>();
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (chavePrimaria.Count != 1)
                throw new ApplicationException("Este método só aceita models com chave primária única, ou a chave não está definida");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE {1}=@CRITERIO",
                        nomeTabela,
                        chavePrimaria[0].Key
                    );

                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.Decimal, 18, codigo);

                    // Execucao
                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Exclui um registro da base de dados
        /// </summary>
        public static int Excluir<TipoModel>(decimal codigo, decimal codigo2, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var chavePrimaria = ObterChavePrimaria<TipoModel>();
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (chavePrimaria.Count != 2)
                throw new ApplicationException("Este método só aceita models com chave primária dupla, ou a chave não está definida");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE {1}=@CRITERIO AND {2}=@CRITERIO2",
                        nomeTabela,
                        chavePrimaria[0].Key,
                        chavePrimaria[1].Key
                    );

                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.Decimal, 18, codigo);
                    command.NewCriteriaParameter("@CRITERIO2", SqlDbType.Decimal, 18, codigo2);

                    // Execucao
                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Exclui um registro da base de dados
        /// </summary>
        public static int Excluir<TipoModel>(List<string> codigos, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var chavePrimaria = ObterChavePrimaria<TipoModel>();
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (chavePrimaria.Count != 1)
                throw new ApplicationException("Este método só aceita models com chave primária única, ou a chave não está definida");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            if (codigos.Count == 0)
                throw new ApplicationException("A lista de códigos está vazia");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE {1} IN (SELECT ITEM FROM dbo.SplitTable(@CRITERIO) WHERE ITEM IS NOT NULL)",
                        nomeTabela,
                        chavePrimaria[0].Key
                    );

                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, -1, String.Join(",", codigos.ToArray()));

                    // Execucao
                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Exclui um registro da base de dados
        /// </summary>
        public static int Excluir<TipoModel>(TipoModel criterios, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedades);
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            var sbWhere = new StringBuilder();
            int criterioCount = 0;

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    var items = (from e in typeof(TipoModel).GetProperties()
                                 where e.IsDefined(typeof(DataField), false)
                                 select new
                                 {
                                     valor = e.GetValue(criterios, null),
                                     dataField = e.GetCustomAttribute<DataField>(false)
                                 }).ToList();

                    foreach (var item in items)
                    {
                        // Campos de Dados
                        if (item.valor != null)
                        {
                            if (criterioCount == 0) sbWhere.Append(" ");
                            else sbWhere.Append(" AND ");

                            criterioCount++;

                            // Parametros
                            sbWhere.Append(string.Concat(item.dataField.Name, " = @C", criterioCount));
                            command.NewCriteriaParameter(string.Format("@C{0}", criterioCount), item.valor);
                        }
                    }

                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE {1}",
                        nomeTabela,
                        sbWhere.ToString()
                    );

                    // Execucao

                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Exclui um registro da base de dados podendo escolher o campo chave
        /// </summary>
        public static int Excluir<TipoModel>(string nomePropriedade, decimal codigo, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var chavePrimaria = ObterNomeCampo<TipoModel>(nomePropriedade);
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (string.IsNullOrEmpty(chavePrimaria))
                throw new ApplicationException("Nome da propriedade não encontrada na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE {1}=@CRITERIO",
                        nomeTabela,
                        chavePrimaria
                    );

                    // Parametros
                    command.NewCriteriaParameter("@CRITERIO", SqlDbType.Decimal, 18, codigo);

                    // Execucao
                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Salvar

        /// <summary>
        /// Insere ou atualiza o registro na base de dados
        /// </summary>
        public static decimal Salvar<TipoModel>(TipoModel model, string connectionString = "", string[] camposChave = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(model, propriedades, camposChave);
            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            var sbInsertFields = new StringBuilder();
            var sbInsertValues = new StringBuilder();

            var sbUpdate = new StringBuilder();
            var sbUpdateChaves = new StringBuilder();

            try
            {
                using (var command = Database.NewCommand("", connectionString))
                {
                    command.CommandType = CommandType.Text;

                    var lista = (from e in propriedades
                                 let valor = e.GetValue(model, null)
                                 let dataField = e.GetCustomAttribute<DataField>(false)
                                 where e.IsDefined(typeof(DataField), false) && // Campos de Dados
                                 !e.IsDefined(typeof(JoinField), false) && // Ignorar os Join Fields
                                 !dataField.AutoNumber &&
                                (valor != null || (valor == null && !dataField.IgnoreEmpty))
                                 select new { valor = valor, dataField = dataField }
                    ).ToList();


                    for (int i = 0; i < lista.Count; i++)
                    {
                        sbInsertFields.Append(string.Concat(lista[i].dataField.Name, ","));
                        sbInsertValues.Append(string.Concat("@C", i, ","));

                        if (!lista[i].dataField.PrimaryKey)
                            sbUpdate.Append(string.Format("{0}=@C{1},", lista[i].dataField.Name, i));

                        command.NewParameter(string.Format("@C{0}", i), lista[i].valor);
                    }

                    string chavesQuery;
                    bool existeValorChave = false;
                    for (var i = 0; i < chavePrimaria.Count; i++)
                    {
                        decimal chaveNumerica;
                        if (decimal.TryParse(chavePrimaria[i].Value, out chaveNumerica))
                            sbUpdateChaves.Append(string.Format("{0}={1},", chavePrimaria[i].Key, chaveNumerica));
                        else
                            sbUpdateChaves.Append(string.Format("{0}='{1}',", chavePrimaria[i].Key, chavePrimaria[i].Value));

                        if (!string.IsNullOrEmpty(chavePrimaria[i].Value)) existeValorChave = true;
                    }

                    if (!existeValorChave)
                        chavesQuery = "1 = 0";
                    else
                        chavesQuery = sbUpdateChaves.ToString().TrimEnd(',').Replace(",", " AND ");

                    var sqlInsert = string.Format(
                        "INSERT INTO {0} ({1}) VALUES ({2});"
                        , nomeTabela
                        , sbInsertFields.ToString().TrimEnd(',')
                        , sbInsertValues.ToString().TrimEnd(',')
                    );

                    var updateFields = sbUpdate.ToString().TrimEnd(',');

                    var sqlUpdate = string.Format(
                        "UPDATE {0} SET {1} WHERE {2};"
                        , nomeTabela
                        , updateFields
                        , chavesQuery
                    );

                    if (string.IsNullOrEmpty(updateFields)) sqlUpdate = "";

                    command.CommandText = string.Format(@"
                        IF (EXISTS (SELECT 1 FROM {0} WHERE {2}))
                        BEGIN
	                        {4}
                            SELECT -1;
                        END
                        ELSE
                        BEGIN
                            {3}
                            SELECT SCOPE_IDENTITY();
                        END
                    "
                        , nomeTabela
                        , chavePrimaria.Count == 1 && !string.IsNullOrEmpty(chavePrimaria[0].Value) ? chavePrimaria[0].Value : "0"
                        , chavesQuery
                        , sqlInsert
                        , sqlUpdate
                    );


                    // Execucao
                    decimal codigoRetorno = -1;
                    decimal.TryParse(Convert.ToString(Database.ExecuteScalar(command)), out codigoRetorno);

                    if (codigoRetorno == -1 &&
                        chavePrimaria.Count == 1 &&
                        !string.IsNullOrEmpty(chavePrimaria[0].Value) &&
                        !decimal.TryParse(chavePrimaria[0].Value, out codigoRetorno))
                    {
                        codigoRetorno = -1;
                    }

                    SalvarAuditoria<TipoModel>(codigoRetorno, model, existeValorChave ? Acao.Insert : Acao.Update);

                    return codigoRetorno;

                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Insere uma lista de registros na base de dados
        /// </summary>
        public static void Salvar<TipoModel>(List<TipoModel> model, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            if (model == null || model.Count == 0)
            {
                return;
            }

            var nomeTabela = ObterNomeTabela<TipoModel>();

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            var sbInsertFields = new StringBuilder();
            var sbValues = new StringBuilder(Environment.NewLine);

            try
            {
                using (var command = Database.NewCommand("", connectionString))
                {
                    command.CommandType = CommandType.Text;

                    var propriedades = (from e in typeof(TipoModel).GetProperties()
                                        where e.IsDefined(typeof(DataField), false)
                                        select new
                                        {
                                            info = e,
                                            dataField = e.GetCustomAttribute<DataField>(false)
                                        }).ToList();

                    for (int i = 0; i < model.Count; i++)
                    {
                        sbValues.Append("SELECT ");

                        for (int x = 0; x < propriedades.Count; x++)
                        {
                            var dataField = propriedades[x].dataField;

                            if (dataField.AutoNumber == false && dataField.IgnoreEmpty == false)
                            {
                                if (i == 0)
                                {
                                    sbInsertFields.Append(string.Concat(dataField.Name, ","));
                                }

                                sbValues.Append(string.Concat("@C", command.Parameters.Count, ","));
                                command.NewParameter(string.Concat("@C", command.Parameters.Count), propriedades[x].info.GetValue(model[i], null));
                            }
                        }

                        sbValues.Remove(sbValues.Length - 1, 1);
                        sbValues.Append(Environment.NewLine);

                        if (model.Count > 1 && model.Count - 1 > i)
                        {
                            sbValues.AppendLine("UNION ALL");
                        }
                    }

                    command.CommandText = string.Format(
                        "INSERT INTO {0} ({1}) {2};"
                        , nomeTabela
                        , sbInsertFields.ToString().TrimEnd(',')
                        , sbValues.ToString()
                    );

                    // Execucao
                    Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region SalvarParcial

        /// <summary>
        /// Insere ou atualiza o registro na base de dados
        /// No UPDATE: Este método não substitui os campos enviados NULOS, 
        /// mantendo o valor que já estiver na base de dados
        /// </summary>
        public static decimal SalvarParcial<TipoModel>(TipoModel model, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var propriedades = typeof(TipoModel).GetProperties();
            var chavePrimaria = ObterChavePrimaria<TipoModel>(model, propriedades);
            var nomeTabela = ObterNomeTabela<TipoModel>();
            var sbInsertFields = new StringBuilder();
            var sbInsertValues = new StringBuilder();
            var sbUpdate = new StringBuilder();
            var sbUpdateChaves = new StringBuilder();

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            try
            {
                using (var command = Database.NewCommand("", connectionString))
                {
                    command.CommandType = CommandType.Text;

                    // Procura as informações em [row] conforme as propriedades da [model]
                    var lista = (from e in propriedades
                                 let valor = e.GetValue(model, null)
                                 where e.IsDefined(typeof(DataField), false) && // Campos de Dados
                                 !e.IsDefined(typeof(JoinField), false) && // Ignorar os Join Fields
                                 valor != null // Ignorar os campos sem valores
                                 select new { valor = valor, dataField = e.GetCustomAttribute<DataField>(false) }
                                 ).ToList();

                    for (int i = 0; i < lista.Count; i++)
                    {
                        sbInsertFields.Append(string.Concat(lista[i].dataField.Name, ","));
                        sbInsertValues.Append(string.Concat("@C", i, ","));

                        if (!lista[i].dataField.PrimaryKey)
                            sbUpdate.Append(string.Format("{0}=@C{1},", lista[i].dataField.Name, i));

                        command.NewCriteriaParameter(string.Format("@C{0}", i), lista[i].valor);
                    }

                    string chavesQuery;
                    bool existeValorChave = false;

                    for (var i = 0; i < chavePrimaria.Count; i++)
                    {
                        sbUpdateChaves.Append(string.Format("{0}={1},", chavePrimaria[i].Key, chavePrimaria[i].Value));
                        if (!string.IsNullOrEmpty(chavePrimaria[i].Value)) existeValorChave = true;
                    }

                    if (!existeValorChave)
                        chavesQuery = "1 = 0";
                    else
                        chavesQuery = sbUpdateChaves.ToString().TrimEnd(',').Replace(",", " AND ");

                    var sqlInsert = string.Format(
                        "INSERT INTO {0} ({1}) VALUES ({2});"
                        , nomeTabela
                        , sbInsertFields.ToString().TrimEnd(',')
                        , sbInsertValues.ToString().TrimEnd(',')
                    );

                    var updateFields = sbUpdate.ToString().TrimEnd(',');

                    var sqlUpdate = string.Format(
                        "UPDATE {0} SET {1} WHERE {2};"
                        , nomeTabela
                        , updateFields
                        , chavesQuery
                    );

                    if (string.IsNullOrEmpty(updateFields)) sqlUpdate = "";

                    command.CommandText = string.Format(@"
                        IF (EXISTS (SELECT 1 FROM {0} WHERE {2}))
                        BEGIN
	                        {4}
                            SELECT {1};
                        END
                        ELSE
                        BEGIN
                            {3}
                            SELECT SCOPE_IDENTITY();
                        END
                    "
                        , nomeTabela
                        , chavePrimaria.Count == 1 && !string.IsNullOrEmpty(chavePrimaria[0].Value) ? chavePrimaria[0].Value : "0"
                        , chavesQuery
                        , sqlInsert
                        , sqlUpdate
                    );

                    // Execucao
                    decimal codigoRetorno;
                    decimal.TryParse(Convert.ToString(Database.ExecuteScalar(command)), out codigoRetorno);

                    SalvarAuditoria<TipoModel>(codigoRetorno, model, existeValorChave ? Acao.Insert : Acao.Update);

                    return codigoRetorno;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static void SalvarParcial<TipoModel>(List<TipoModel> model, string connectionString = "")
        {
            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    connectionString = ApplicationSettings.ConnectionStrings.Default;
            //}

            //var obj = Activator.CreateInstance<TipoModel>();

            //var propriedades = typeof(TipoModel).GetProperties();
            //var chavePrimaria = ObterChavePrimaria<TipoModel>(propriedades);
            //var nomeTabela = ObterNomeTabela<TipoModel>();
            //var sbInsertFields = new StringBuilder();
            //var sbInsertValues = new StringBuilder();
            //var sbUpdate = new StringBuilder();
            //var sbUpdateChaves = new StringBuilder();

            //var sbQueryFinal = new StringBuilder();

            //if (chavePrimaria.Count == 0)
            //    throw new ApplicationException("Chave primária não definida na model");

            //if (string.IsNullOrEmpty(nomeTabela))
            //    throw new ApplicationException("Nome da tabela não definida na model");

            //try
            //{
            //    int param = 0;

            //    using (var command = Database.NewCommand("", connectionString))
            //    {
            //        command.CommandType = CommandType.Text;

            //        foreach(var item in model)
            //        {
            //            // Procura as informações em [row] conforme as propriedades da [model]
            //            var lista = (from e in propriedades
            //                         let valor = e.GetValue(item, null)
            //                         where e.IsDefined(typeof(DataField), false) && // Campos de Dados
            //                         !e.IsDefined(typeof(JoinField), false) && // Ignorar os Join Fields
            //                         valor != null // Ignorar os campos sem valores
            //                         select new { valor = valor, dataField = e.GetCustomAttribute<DataField>(false) }
            //                         ).ToList();

            //            for (int i = 0; i < lista.Count; i++)
            //            {
            //                sbInsertFields.Append(string.Concat(lista[i].dataField.Name, ","));
            //                sbInsertValues.Append(string.Concat("@C", param, ","));

            //                if (!lista[i].dataField.PrimaryKey)
            //                    sbUpdate.Append(string.Format("{0}=@C{1},", lista[i].dataField.Name, param));

            //                command.NewCriteriaParameter(string.Format("@C{0}", param), lista[i].valor);

            //                param++;
            //            }

            //            string chavesQuery;
            //            bool existeValorChave = false;

            //            for (var i = 0; i < chavePrimaria.Count; i++)
            //            {
            //                sbUpdateChaves.Append(string.Format("{0}={1},", chavePrimaria[i].Key, chavePrimaria[i].Value));
            //                if (!string.IsNullOrEmpty(chavePrimaria[i].Value)) existeValorChave = true;
            //            }

            //            if (!existeValorChave)
            //                chavesQuery = "1 = 0";
            //            else
            //                chavesQuery = sbUpdateChaves.ToString().TrimEnd(',').Replace(",", " AND ");

            //            var sqlInsert = string.Format(
            //                "INSERT INTO {0} ({1}) VALUES ({2});"
            //                , nomeTabela
            //                , sbInsertFields.ToString().TrimEnd(',')
            //                , sbInsertValues.ToString().TrimEnd(',')
            //            );

            //            var updateFields = sbUpdate.ToString().TrimEnd(',');

            //            var sqlUpdate = string.Format(
            //                "UPDATE {0} SET {1} WHERE {2};"
            //                , nomeTabela
            //                , updateFields
            //                , chavesQuery
            //            );

            //            if (string.IsNullOrEmpty(updateFields)) sqlUpdate = "";

            //            sbQueryFinal.Append(string.Format(@" 
            //                IF (EXISTS (SELECT 1 FROM {0} WHERE {2})) " +
            //                "BEGIN " +
            //                    "{4} " +
            //                "END " +
            //                "ELSE " +
            //                "BEGIN " +
            //                    "{3} " +
            //                "END"
            //                , nomeTabela
            //                , chavePrimaria.Count == 1 && !string.IsNullOrEmpty(chavePrimaria[0].Value) ? chavePrimaria[0].Value : "0"
            //                , chavesQuery
            //                , sqlInsert
            //                , sqlUpdate)
            //            );
            //        }

            //        command.CommandText = sbQueryFinal.ToString();

            //        // Execucao
            //        Database.ExecuteNonQuery(command);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ApplicationLog.ErrorLog(ex);
            //    throw;
            //}
        }

        #endregion

        #region SalvarAuditoria

        public static void SalvarAuditoria<TipoModel>(decimal codigo, TipoModel model, Acao acao)
        {
            try
            {
                var properties = typeof(TipoModel).GetProperties().ToList();
                var aud = (from e in properties
                           where e.IsDefined(typeof(Auditing))
                           select e.GetCustomAttribute<Auditing>(false)).ToArray();

                if (aud == null || aud.Length == 0)
                    return;

                decimal? codigoPortal = null;
                var chave = string.Empty;
                var usuario = BLUsuario.ObterLogado();
                var url = aud[0].Url.ToLowerInvariant();

                if (!url.EndsWith("//")) url = string.Concat(url, "/");

                if (!string.IsNullOrEmpty(aud[0].CampoPortal))
                {
                    var portal = properties.Find(o => !string.IsNullOrEmpty(o.Name) && o.Name.Equals(aud[0].CampoPortal, StringComparison.InvariantCultureIgnoreCase));

                    if (portal != null)
                    {
                        codigoPortal = (decimal?)portal.GetValue(model, null);
                    }
                }

                if (aud[0].CamposChave != null && aud[0].CamposChave.Length > 0)
                {
                    var cref = (from e in properties
                                where !string.IsNullOrEmpty(e.Name) &&
                                    aud[0].CamposChave.Select(a => a.ToLower()).Contains(e.Name.ToLower())
                                select e.GetValue(model, null).ToString()).ToList();

                    for (int i = 0; i < cref.Count; i++)
                    {
                        chave = string.Concat(chave, cref[i], "|");
                    }

                    chave = chave.TrimEnd('|');
                }
                else
                {
                    chave = codigo.ToString();
                }

                if (aud[0].Url == null)
                {
                    aud[0].Url = string.Empty;
                }

                if (usuario.Funcionalidades == null)
                {
                    usuario.Funcionalidades = new List<MLUsuarioItemFuncionalidade>();
                }

                var item = usuario.Funcionalidades.Find(o => !string.IsNullOrEmpty(o.Url) && url.ToLowerInvariant().StartsWith((string.Concat(o.Url.ToLowerInvariant(), "/")).Replace("////", "//")));

                if (item != null)
                {
                    var modelauditoria = new MLAuditoria
                    {
                        CodigoPortal = codigoPortal,
                        CodigoFuncionalidade = item.CodigoFuncionalidade,
                        Login = usuario.Login,
                        CodigoUsuario = usuario.Codigo,
                        CodigoReferencia = chave,
                        Data = DateTime.Now
                    };

                    switch (acao)
                    {
                        case Acao.Insert:
                            modelauditoria.Acao = "INS";
                            break;
                        case Acao.Update:
                            modelauditoria.Acao = "UPD";
                            break;
                        case Acao.Delete:
                            modelauditoria.Acao = "DEL";
                            break;
                        default:
                            break;
                    }

                    if (System.Transactions.Transaction.Current != null &&
                        System.Transactions.Transaction.Current.Connection != null &&
                        System.Transactions.Transaction.Current.Connection.ConnectionString != ApplicationSettings.ConnectionStrings.Default)
                        System.Transactions.Transaction.CurrentAuditing = modelauditoria;
                    else
                        CRUD.Salvar<MLAuditoria>(modelauditoria, ApplicationSettings.ConnectionStrings.Default);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }

        #endregion

        #region Auxiliares

        #region ForeignKey

        private static ForeignKey ObterChaveEstrangeira<TipoModel>(PropertyInfo[] propriedades = null, string[] campos = null)
        {
            var model = Activator.CreateInstance<TipoModel>();
            return ObterChaveEstrangeira<TipoModel>(model, propriedades, campos);
        }

        /// <summary>
        /// Procura qual o campo chave primária de uma model
        /// </summary>
        private static ForeignKey ObterChaveEstrangeira<TipoModel>(TipoModel model, PropertyInfo[] propriedades = null, string[] campos = null)
        {
            if (propriedades == null)
            {
                propriedades = typeof(TipoModel).GetProperties();
            }

            //foreach(PropertyInfo e in propriedades)
            //{
            //    var dataField = e.GetCustomAttribute<DataField>(false)
            //               where e.IsDefined(typeof(DataField), false) && dataField.ForeignKey &&
            //               (campos == null || (campos.Select(a => a.ToLower()).Contains(dataField.Name.ToLower());
            //}

            //// Procura as informações em [row] conforme as propriedades da [model]
            var retorno = (from e in propriedades
                           let dataField = e.GetCustomAttribute<DataField>(false)
                           where e.IsDefined(typeof(DataField), false) && dataField.ForeignKey &&
                           (campos == null || (campos.Select(a => a.ToLower()).Contains(dataField.Name.ToLower())))
                           select new ForeignKey { Key = dataField.Name, Value = "" } //Convert.ToString(e.GetValue(model, null)) }
                        ).ToList();

            if(retorno != null && retorno.Count > 0)
                return retorno[0];

            return null;
        }

        #endregion

        #region ObterChavePrimaria

        private static List<PrimaryKey> ObterChavePrimaria<TipoModel>(PropertyInfo[] propriedades = null, string[] campos = null)
        {
            var model = Activator.CreateInstance<TipoModel>();
            return ObterChavePrimaria<TipoModel>(model, propriedades, campos);
        }

        /// <summary>
        /// Procura qual o campo chave primária de uma model
        /// </summary>
        private static List<PrimaryKey> ObterChavePrimaria<TipoModel>(TipoModel model, PropertyInfo[] propriedades = null, string[] campos = null)
        {
            if (propriedades == null)
            {
                propriedades = typeof(TipoModel).GetProperties();
            }

            //// Procura as informações em [row] conforme as propriedades da [model]
            var retorno = (from e in propriedades
                           let dataField = e.GetCustomAttribute<DataField>(false)
                           where e.IsDefined(typeof(DataField), false) && dataField.PrimaryKey &&
                           (campos == null || (campos.Select(a => a.ToLower()).Contains(dataField.Name.ToLower())))
                           select new PrimaryKey { Key = dataField.Name, Value = Convert.ToString(e.GetValue(model, null)) }
                        ).ToList();

            return retorno;
        }

        #endregion

        #region ObterNomeTabela

        /// <summary>
        /// Obtem o nome da tabela definido na model
        /// </summary>
        private static string ObterNomeTabela<TipoModel>()
        {
            return ObterNomeTabela(typeof(TipoModel));
        }

        private static string ObterNomeTabela(Type tipo)
        {
            var dbField = tipo.GetCustomAttribute<Table>(false);

            if (dbField == null) return "";

            return dbField.Name;
        }

        #endregion

        #region ObterNomeCampo

        /// <summary>
        /// Procura qual o campo da propriedade de uma model
        /// </summary>
        private static string ObterNomeCampo<TipoModel>(string propriedade, PropertyInfo[] propriedades = null)
        {
            if (propriedades == null)
            {
                propriedades = typeof(TipoModel).GetProperties();
            }

            var campos = (from e in propriedades
                          where e.IsDefined(typeof(DataField), false)
                          && e.Name.Equals(propriedade, StringComparison.InvariantCultureIgnoreCase)
                          select e.GetCustomAttribute<DataField>(false)).ToList();

            if (campos.Count > 0)
                return campos[0].Name;

            return string.Empty;
        }

        #endregion

        #region ObterValorCampo

        /// <summary>
        /// Obtem valor da chave primaria da model
        /// </summary>
        private static object ObterValorCampo<TipoModel>(TipoModel model, string campo, PropertyInfo[] propriedades = null)
        {
            if (propriedades == null)
            {
                propriedades = typeof(TipoModel).GetProperties();
            }

            // Procura as informações em [row] conforme as propriedades da [model]
            var retorno = (from e in propriedades
                           let dataField = e.GetCustomAttribute<DataField>(false)
                           where e.IsDefined(typeof(DataField), false) && dataField.Name.Equals(campo, StringComparison.InvariantCultureIgnoreCase)
                           select e.GetValue(model, null)
                        ).ToList();

            if (retorno.Count > 0)
                return retorno[0];

            return null;
        }

        #endregion

        #endregion

        #region CopiarValores

        /// <summary>
        ///  Copia os valores de uma model para outra, os nomes das propriedades devem ser iguais.
        /// </summary>
        public static TipoModel2 CopiarValores<TipoModel, TipoModel2>(TipoModel modelTabelaOrigem, TipoModel2 modelDestino)
        {
            var propOrigem = typeof(TipoModel).GetProperties();
            var propDestino = typeof(TipoModel2).GetProperties();

            foreach (var item in propOrigem)
            {
                var foundDestino = propDestino.Where(p => p.Name == item.Name);
                if (foundDestino != null)
                {
                    try
                    {
                        var enm = foundDestino.GetEnumerator();
                        while (enm.MoveNext())
                        {
                            enm.Current.SetValue(modelDestino, item.GetValue(modelTabelaOrigem, null), null);
                        }
                    }
                    catch { }
                }
            }

            return modelDestino;
        }

        #endregion

        #region Ordenar

        /// <summary>
        /// Atualiza uma série de registros para reordenar os registros
        /// </summary>
        /// <returns>
        /// Quantidade de itens alterados
        /// </returns>
        public static int Ordenar<TipoModel>(List<decimal> codigosOrdenados, string propriedadeOrdem, string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            var chavePrimaria = ObterChavePrimaria<TipoModel>();
            var nomeTabela = ObterNomeTabela<TipoModel>();
            var nomeOrdem = ObterNomeCampo<TipoModel>(propriedadeOrdem);

            if (chavePrimaria.Count == 0)
                throw new ApplicationException("Chave primária não definida na model");

            if (string.IsNullOrEmpty(nomeTabela))
                throw new ApplicationException("Nome da tabela não definida na model");

            if (string.IsNullOrEmpty(nomeOrdem))
                throw new ApplicationException("Nome do campo de ordenação não definida na model");

            var sbOrderBy = new StringBuilder();
            var sbWhere = new StringBuilder();

            try
            {
                int contador = 1;

                foreach (var codigo in codigosOrdenados)
                {
                    sbOrderBy.AppendLine(string.Format("WHEN {0} = {1} THEN {2}", chavePrimaria[0].Key, codigo, contador));

                    if (contador > 1) sbWhere.Append(",");
                    sbWhere.Append(codigo);

                    contador += 1;
                }

                sbOrderBy.Append(string.Format("ELSE {0}", contador));


                using (var command = Database.NewCommand("", connectionString))
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = string.Format(@"
                        UPDATE	{0}
                        SET		{2} = ORDEM
                        
                        FROM {0}
                        INNER JOIN (
	                        SELECT 
		                        {1}
	                        ,	ROW_NUMBER() 
		                        OVER ( ORDER BY CASE 
			                        {3}
		                        END ) AS ORDEM
	                        FROM
		                        {0}
	                        WHERE
		                        {1} IN ({4})
                        ) AS ORDENADOR
                        ON {0}.{1} = ORDENADOR.{1}
                    "
                        , nomeTabela
                        , chavePrimaria[0].Key
                        , nomeOrdem
                        , sbOrderBy.ToString()
                        , sbWhere.ToString()
                    );

                    // Execucao
                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executar uma query e retornar lista de model generica.
        /// </summary>
        /// <typeparam name="TipoModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <user>rvissontai</user>
        public static List<TipoModel> ExecuteQuery<TipoModel>(string query, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = query;
                    
                    return Database.ExecuteReader<TipoModel>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static void ExecuteNomQuery(string query, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ApplicationSettings.ConnectionStrings.Default;
            }

            try
            {
                using (var command = Database.NewCommand(string.Empty, connectionString))
                {
                    command.CommandText = query;

                    Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        public enum Acao
        {
            Insert,
            Update,
            Delete
        }
    }

    /// <summary>
    /// Classe usada para montagem dos JOINs pelo CRUD
    /// </summary>
    class JoinTable
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string SourceKey { get; set; }
        public string DestKey { get; set; }
        public int Alias { get; set; }

        public JoinTable(string tableName, string columnName, string sourceKey, string destKey, int alias)
        {
            TableName = tableName;
            ColumnName = columnName;
            SourceKey = sourceKey;
            DestKey = destKey;
            Alias = alias;
        }
    }

    class PrimaryKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public SqlDbType Type { get; set; }
        public int Size { get; set; }
    }

    class ForeignKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}