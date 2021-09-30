using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Model
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class Auditing : Attribute
    {
        #region Propriedades

        /// <summary>Nome do campo na base de dados</summary>
        public string Url { get; set; }
        public string[] CamposChave { get; set; }
        public string CampoPortal { get; set; }

        #endregion

        #region Construtores

        public Auditing(string urlfuncionalidade)
        {
            Url = urlfuncionalidade;            
        }

        public Auditing(string urlfuncionalidade, string campoportal)
        {
            Url = urlfuncionalidade;            
            CampoPortal = campoportal;
        }

        public Auditing(string urlfuncionalidade,string[] camposchave, string campoportal)
        {
            Url = urlfuncionalidade;
            CamposChave = camposchave;
            CampoPortal = campoportal;
        }

        #endregion
    }
    
 
}
