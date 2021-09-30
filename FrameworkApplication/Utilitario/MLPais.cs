using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Model;

namespace Framework.Utilities
{
    public class MLPais
    {
        [DataField("PAI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PAI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("Contem", SqlDbType.Bit)]
        public bool Contem { get; set; }

        [DataField("ContemOutraPagina", SqlDbType.Bit)]
        public bool ContemOutraPagina { get; set; }
    }
}
