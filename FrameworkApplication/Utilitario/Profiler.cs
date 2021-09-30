using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using Framework.DataLayer;
using Framework.Model;

namespace Framework.Utilities
{

    /// <summary>
    /// Classe para realização de processo de análise e melhoria de perfomance
    /// dos comandos SQL executados pela aplicação. Os comandos são armazenados na navegação
    /// do usuário que iniciou o Profiler, e depois é possível realizar os planos
    /// de execução para as consultas obtidas
    /// </summary>
    public static class Profiler
    {
        #region Start

        /// <summary>
        /// Inicia o processo de captura de comandos na sessão do usuário
        /// </summary>
        public static void Start()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null)
                {
                    if (session["profiler"] == null) session.Add("profiler", "on");
                    else session["profiler"] = "on";
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }
        #endregion

        #region Stop

        /// <summary>
        /// Para o processo de captura de comandos na sessão do usuário
        /// </summary>
        /// <param name="query"></param>
        public static void Stop()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null)
                {
                    if (session["profiler"] == null) session.Add("profiler", "off");
                    else session["profiler"] = "off";
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }
        #endregion

        #region Save

        /// <summary>
        /// Armazena a consulta executada na sessão do usuário na lista de registro
        /// para poder ser analisada posteriormente
        /// </summary>
        public static void Save(SqlCommand cmd, string connectionString)
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null && Convert.ToString(session["profiler"]) == "on")
                {
                    var query = cmd.CommandText;
                    if (session["lista-profiler"] == null) session["lista-profiler"] = new List<Query>();

                    if (((List<Query>)session["lista-profiler"]).Find(o => o.Consulta == query) == null)
                        ((List<Query>)session["lista-profiler"]).Add(new Query(cmd, connectionString));
                    else
                        ((List<Query>)session["lista-profiler"]).Find(o => o.Consulta == query).Contador += 1;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

        }

        #endregion

        #region List

        /// <summary>
        /// Obtem a lista de comandos armazenados durante o processo de captura
        /// </summary>
        public static List<Query> List()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null && session["lista-profiler"] != null)
                {
                    return ((List<Query>)session["lista-profiler"]);
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return new List<Query>();

        }
        #endregion

        #region Clear

        /// <summary>
        /// Apaga a lista de comandos armazenados durante o processo de captura
        /// </summary>
        public static void Clear()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null && (session["lista-profiler"] != null))
                {
                    session["lista-profiler"] = new List<Query>();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }
        #endregion

        #region Status

        /// <summary>
        /// Obtem o status de execução do serviço
        /// </summary>
        public static string Status()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null)
                {
                    if (session["profiler"] != null) return Convert.ToString(session["profiler"]);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return "off";

        }
        #endregion

        #region Analyze

        /// <summary>
        /// Realiza o processo de análise dos comandos SQL armazenados na lista durante o processo de captura
        /// de navegação do usuário. Os resultados são obtidos em formato XML e armazenados na sessão, assim 
        /// como a lista de índices de tabela sugeridos para criação.
        /// </summary>
        public static void Analyze()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;

                if (session != null && session["lista-profiler"] != null)
                {
                    for (var i = 0; i < ((List<Query>)session["lista-profiler"]).Count; i++)
                    {
                        var query = ((List<Query>)session["lista-profiler"])[i];

                        try
                        {
                            using (var scope = new TransactionScope(query.ConnnectionString))
                            {
                                using (var command = Database.NewCommand(string.Empty, query.ConnnectionString))
                                {
                                    ((List<Query>)session["lista-profiler"])[i].BancoDados = command.Connection.Database;

                                    command.CommandText = "SET STATISTICS XML ON";
                                    Database.ExecuteNonQuery(command);


                                    command.CommandText = query.Consulta;
                                    command.CommandType = query.Tipo;

                                    foreach (SqlParameter p in query.Parameters)
                                        command.NewCriteriaParameter(p.ParameterName, p.SqlDbType, p.Size, p.Value);
                                    var dataSet = Database.ExecuteDataSet(command);


                                    command.CommandText = "SET STATISTICS XML OFF";
                                    command.CommandType = CommandType.Text;
                                    Database.ExecuteNonQuery(command);

                                    for (var counttable = 1; counttable < dataSet.Tables.Count; counttable = counttable + 2)
                                    {
                                        if (dataSet != null && dataSet.Tables.Count > 1 && dataSet.Tables[counttable].Rows.Count > 0)
                                        {
                                            var plan = dataSet.Tables[counttable].Rows[0][0].ToString();
                                            var indices = Tuning(plan);
                                            ((List<Query>)session["lista-profiler"])[i].IndicesAusentes.AddRange(indices);
                                            ((List<Query>)session["lista-profiler"])[i].Resultado += plan;
                                            ((List<Query>)session["lista-profiler"])[i].Status = true;
                                        }
                                    }
                                }
                                scope.Complete();
                            }
                        }
                        catch (Exception ex)
                        {
                            ((List<Query>)session["lista-profiler"])[i].Resultado = ex.Message;
                            ((List<Query>)session["lista-profiler"])[i].Status = false;
                        }
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

        #region Tuning

        /// <summary>
        /// Utilizado pelo processo de análise para realizar a leitura do XML
        /// retornado pelo plano de execução do SQL
        /// </summary>
        public static List<string> Tuning(string xml)
        {
            try
            {
                var scriptTuning = new List<string>();

                xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + xml;
                XDocument doc = XDocument.Parse(xml);
                XNamespace ns2 = XNamespace.Get("http://schemas.microsoft.com/sqlserver/2004/07/showplan");

                var cols = new List<string>();

                var indices = doc.Descendants(ns2 + "ShowPlanXML").Elements(ns2 + "BatchSequence").
                    Elements(ns2 + "Batch").Elements(ns2 + "Statements").Elements(ns2 + "StmtSimple").
                    Elements(ns2 + "QueryPlan").Elements(ns2 + "MissingIndexes").Descendants(ns2 + "MissingIndex");

                foreach (var item in indices)
                {
                    var table = item.Attribute("Table").Value;
                    var groups = item.Descendants(ns2 + "ColumnGroup");
                    var colsmain = string.Empty;
                    var colsinclude = string.Empty;

                    foreach (var g in groups)
                    {
                        var usage = g.Attribute("Usage").Value;

                        foreach (var c in g.Descendants(ns2 + "Column"))
                        {
                            var name = c.Attribute("Name").Value;
                            if (usage == "INCLUDE")
                                colsinclude += name + ",";
                            else
                                colsmain += name + ",";
                        }

                    }
                    colsmain = colsmain.TrimEnd(',');
                    var nameindex = "IX_" + table.Replace("[", "").Replace("]", "") + "_" + colsmain.Replace("[", "").Replace("]", "").Replace(",", "_");

                    if (!string.IsNullOrEmpty(colsmain))
                    {                        
                        var scriptIF = string.Format("IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].{0}') AND name = N'{1}')\n",table,nameindex);
                        var scriptInclude = string.Empty;
                        if (!string.IsNullOrEmpty(colsinclude))
                            scriptInclude = string.Format("INCLUDE({0})",colsinclude.TrimEnd(','));

                        scriptTuning.Add(string.Format("{3} CREATE NONCLUSTERED INDEX [{0}] ON {1} ({2})\n{4}", nameindex, table, colsmain, scriptIF,scriptInclude));
                    }
                    
                }

                return scriptTuning;

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        
        #region Script

        /// <summary>
        /// Monta o resultado de realização das consultas para apresentação na área administrativa
        /// </summary>
        public static QueryResultado Script()
        {
            try
            {
                var consultas = List();
                var script = new StringBuilder();
                var indices = 0;

                foreach(var c in consultas){
                    if (c.IndicesAusentes.Count > 0)
                    {                        
                        script.AppendLine(string.Format("use {0};\n", c.BancoDados));
                        script.AppendLine(string.Format("/*{0}*/\n", c.ConsultaResumida));
                        foreach (var i in c.IndicesAusentes)
                        {
                            indices++;
                            script.AppendLine(i + "\n");
                        }
                    }
                }
                var ret = new QueryResultado();
                ret.Criados = indices;
                ret.Script = script.ToString();
                ret.Total = consultas.Count();
                ret.Erro = consultas.Count(o => !o.Status);
                ret.Sucesso = consultas.Count(o => o.Status);
                

                return ret;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
            
        }
        #endregion

        #region Execute

        /// <summary>
        /// Executa o script de melhoria de perfomance no SQL para criação dos índices
        /// </summary>
        /// <param name="query"></param>
        public static void Execute()
        {
            try
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session != null && session["lista-profiler"] != null)
                {
                    for (var i = 0; i < ((List<Query>)session["lista-profiler"]).Count; i++)
                    {
                        var query = ((List<Query>)session["lista-profiler"])[i];

                        try
                        {
                            using (var scope = new TransactionScope(query.ConnnectionString))
                            {
                                using (var command = Database.NewCommand(string.Empty, query.ConnnectionString))
                                {
                                    foreach (var script in query.IndicesAusentes)
                                    {
                                        command.CommandText = script;
                                        Database.ExecuteNonQuery(command);
                                    }
                                }
                                scope.Complete();
                            }
                        }
                        catch (Exception ex)
                        {
                            ((List<Query>)session["lista-profiler"])[i].Resultado = ex.Message;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

        }
        #endregion

        #region Get

        /// <summary>
        /// Obtem as informações de uma consulta previamente armazenada na lista
        /// </summary>
        /// <param name="query"></param>
        public static Query Get(string consulta)
        {
            try
            {                
                var session = System.Web.HttpContext.Current.Session;
                if (session != null && session["lista-profiler"] != null)
                {
                    var list = ((List<Query>)session["lista-profiler"]);
                    return list.Find(o => o.Consulta == consulta);
                }
                return null;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }
        #endregion
    }
}
