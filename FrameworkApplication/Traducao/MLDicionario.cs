using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Model;
using System.Data;

namespace Framework.Utilities
{
    public class MLDicionario
    {
        [DataField("TER_C_TERMO", SqlDbType.VarChar, -1)]
        public string Termo { get; set; }

        [DataField("TRA_C_IDIOMA", SqlDbType.VarChar, 5)]
        public string Idioma { get; set; }

        [DataField("TRA_C_TERMO", SqlDbType.VarChar, -1)]
        public string Traducao { get; set; }

    }
}
