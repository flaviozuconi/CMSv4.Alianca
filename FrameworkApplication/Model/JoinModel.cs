using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class JoinModel : Attribute
    {
        /// <summary>Nome do campo chave de origem na tabela principal</summary>
        public string SourceKey { get; set; }

        /// <summary>Nome do campo chave de origem na tabela que será unida</summary>
        public string DestKey { get; set; }

        /// <summary>Nome da tabela para uniao</summary>
        public string DestTable { get; set; }

        public JoinModel(string sourceKey, string destKey)
        {
            SourceKey = sourceKey;
            DestKey = destKey;
        }

        public JoinModel(string sourceKey, string destKey, string destTable)
        {
            SourceKey = sourceKey;
            DestKey = destKey;
            DestTable = destTable;
        }
    }
}
