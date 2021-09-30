using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    [Serializable]
    [Table("FWK_LOG_ERRO")]
    public class MLLogErro
    {
        [DataField("LOG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("LOG_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("LOG_C_ERRO", SqlDbType.VarChar, 255)]
        public string Erro { get; set; }

        [DataField("LOG_C_STACK", SqlDbType.VarChar, -1)]
        public string Mensagem { get; set; }

        [DataField("TOTAL", SqlDbType.Int, IgnoreEmpty = true)]
        public Int32? Total { get; set; }
    }
}
