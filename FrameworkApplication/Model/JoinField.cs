using System;
using System.Data;

namespace Framework.Model
{
    /// <summary>
    /// Atributo para os campos da model se integrarem com os 
    /// campos no banco de dados SQL Server
    /// 
    /// Este campo faz uma busca com LEFT JOIN e serve para 
    /// casos onde existe uma referência de CÒDIGO entre as tabelas
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property)]
    public class JoinField : Attribute
    {
        #region Propriedades

        /// <summary>Nome do campo chave de origem na tabela principal</summary>
        public string SourceKey { get; set; }

        /// <summary>Nome do campo chave de origem na tabela que será unida</summary>
        public string DestKey { get; set; }

        /// <summary>Nome da tabela para uniao</summary>
        public string DestTable { get; set; }

        /// <summary>Nome do campo que sera retornado</summary>
        public string DestField { get; set; }

        /// <summary>Nome do campo que sera retornado</summary>
        public string AliasName { get; set; }

        #endregion

        #region Construtores

        public JoinField(string sourceKey, string destTable, string destKey, string destField, string aliasName = "")
        {
            SourceKey = sourceKey;
            DestTable = destTable;
            DestKey = destKey;
            DestField = destField;
            AliasName = aliasName;
        }

        #endregion
    }
}
