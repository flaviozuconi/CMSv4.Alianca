using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Framework.Model
{  

    /// <summary>
    /// Esta classe armazena os comandos SQL que são executados durante o processo de análise
    /// de performance do site (PROFILER), os dados são registrados para gerar os planos 
    /// de execução
    /// </summary>
    [Serializable]
    public class Query
    {
        public Query(SqlCommand cmd, string connectionString)
        {
            Consulta = cmd.CommandText;
            ConnnectionString = connectionString;
            Contador = 1;
            Parameters = cmd.Parameters;
            Tipo = cmd.CommandType;
            IndicesAusentes = new List<string>();
        }

        #region Propriedades

        public string Consulta { get; set; }

        public string Resultado { get; set; }

        public int Contador { get; set; }

        public string ConnnectionString { get; set; }

        public List<string> IndicesAusentes { get; set; }

        public string BancoDados { get; set; }

        public SqlParameterCollection Parameters { get; set; }

        public CommandType Tipo { get; set; }

        public bool Status { get; set; }

        public string ConsultaResumida
        {
            get
            {
                var total = 300;
                if(Consulta.Length > total)
                    return Consulta.Substring(0,total) + "...";

                return Consulta;
            }
        }

        #endregion
    }

    /// <summary>
    /// Apresenta os resultados gerais do processo de melhoria de performance
    /// de banco de dados executados
    /// </summary>
    [Serializable]
    public class QueryResultado
    {
        public int Total { get; set; }

        public int Sucesso { get; set; }

        public int Erro { get; set; }

        public int Criados { get; set; }

        public int Recompilados { get; set; }

        public string Script { get; set; }
    }
  
}
