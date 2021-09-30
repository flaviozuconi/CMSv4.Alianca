using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class CacheField : Attribute
    {
        #region Propriedades

        /// <summary>Nome do campo utilizado no cache</summary>
        public string Name { get; set; }

        #endregion

         #region Construtores

        public CacheField(string Name)
        {
            this.Name = Name;
        }

        #endregion
    }
}
