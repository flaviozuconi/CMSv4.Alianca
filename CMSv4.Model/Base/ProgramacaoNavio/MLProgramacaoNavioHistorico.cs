using Framework.Model;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.Model.Base
{
    [Table("MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS_HISTORICO")]
    public class MLProgramacaoNavioHistorico
    {
        [DataField("PNH_N_CODIGO",SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PNH_D_DATA_IMPORTACAO",SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("PNH_C_USUARIO",SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("PNH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }

        [DataField("PNH_B_FINALIZADO",SqlDbType.Bit)]
        public bool? Finalizado { get; set; }

        [DataField("PNH_C_ARQUIVO",SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }
    }
}
