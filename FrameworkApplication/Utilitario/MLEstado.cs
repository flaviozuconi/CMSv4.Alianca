using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    [Table("CMS_EST_ESTADO")]
    public class MLEstado
    {
        [DataField("EST_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("EST_PAI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPais { get; set; }

        [DataField("EST_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("EST_C_UF", SqlDbType.VarChar, 5)]
        public string Uf { get; set; }
    }
}
