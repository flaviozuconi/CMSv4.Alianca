using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Model;

namespace Framework.Utilities
{
    public class MLPaginaPais
    {
        [DataField("PPA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Codigo { get; set; }

        [DataField("PPA_PAI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Paicodigo { get; set; }

        [DataField("PPA_PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }
    }
}
