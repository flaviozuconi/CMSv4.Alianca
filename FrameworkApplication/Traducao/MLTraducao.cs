using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Model;
using System.Data;

namespace Framework.Utilities
{
    [Serializable]
    public class MLTraducao
    {
        [DataField("TER_N_CODIGO", SqlDbType.Decimal, true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("TER_C_TERMO", SqlDbType.VarChar, -1)]
        public string Termo { get; set; }

        [DataField("TRA_C_IDIOMA", SqlDbType.VarChar, 5)]
        public string Idioma { get; set; }

        [DataField("TRA_C_TERMO", SqlDbType.VarChar, -1)]
        public string Traducao { get; set; }
    }

    [Serializable]
    [Table("CMS_IDI_TRADUCAO")]
    public class MLTraducaoUpdate
    {
        [DataField("TRA_N_CODIGO", SqlDbType.Decimal, true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("TRA_TER_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoTermo { get; set; }

        [DataField("TRA_C_IDIOMA", SqlDbType.VarChar, 5)]
        public string Idioma { get; set; }

        [DataField("TRA_C_TERMO", SqlDbType.VarChar, -1)]
        public string Traducao { get; set; }
    }
}
