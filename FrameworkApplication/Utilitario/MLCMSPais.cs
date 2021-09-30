using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    [Table("CMS_PAI_PAIS")]
    public class MLCMSPais
    {
        [DataField("PAI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PAI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("PAI_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("PAI_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdiomaBase { get; set; }
    }
}
