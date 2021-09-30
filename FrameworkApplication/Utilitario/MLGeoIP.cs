using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Model;

namespace Framework.Utilities
{
    [Table("BRK_GEO_IP")]
    public class MLGeoIP
    {
        [DataField("GEO_N_CODIGO", SqlDbType.Int)]
        public int? Codigo { get; set; }

        [DataField("GEO_C_IP_INICIO", SqlDbType.VarChar, 20)]
        public string IpInicio { get; set; }

        [DataField("GEO_C_IP_FIM", SqlDbType.VarChar, 20)]
        public string IpFim { get; set; }

        [DataField("GEO_C_CODE2", SqlDbType.VarChar, 5)]
        public string Code2 { get; set; }

        [DataField("GEO_C_CODE3", SqlDbType.VarChar, 5)]
        public string Code3 { get; set; }

        [DataField("GEO_C_NAME", SqlDbType.VarChar, 50)]
        public string Name { get; set; }

        [DataField("PPA_PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }  
    }
}
