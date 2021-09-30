using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Model;
using System.Data;

namespace Framework.Utilities
{
    [Serializable]
    [Table("CMS_IDI_TERMOS")]
    public class MLTermo
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("TER_N_CODIGO", SqlDbType.Decimal, true, AutoNumber=true)]
        public decimal? Codigo { get; set; }

        [DataField("TER_C_TERMO", SqlDbType.VarChar, -1)]
        public string Termo { get; set; }
                
    }

  
}
