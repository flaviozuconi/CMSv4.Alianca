using System;
using System.Data;

namespace Framework.Model
{
    /// <summary>
    /// Atributo para as classes da model para integrar com os 
    /// campos no banco de dados SQL Server
    /// </summary>
    /// <remarks>
    /// Exemplos:
    /// 
    /// [Table("NOME_TABELA")] 
    /// public class MinnaModel { }
    /// 
    /// </remarks>
    [System.AttributeUsage(AttributeTargets.Class)]
    public class Table : Attribute
    {
        #region Propriedades

        /// <summary>Nome do campo na base de dados</summary>
        public string Name { get; set; }

        #endregion

        #region Construtores

        public Table(string name)
        {
            Name = name;
        }

        #endregion
    }
}
