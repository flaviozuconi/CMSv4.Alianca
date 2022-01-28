using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model.Base
{
    [Table("MOD_GCP_GESTAO_CUSTO_PENALIDADE_HISTORICO")]
    public class MLGestaoDeCustosPenalidadesHistorico
    {
        [DataField("CPH_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CPH_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("CPH_C_USUARIO", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("CPH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }

        [DataField("CPH_B_FINALIZADO", SqlDbType.Bit)]
        public bool? Finalizado { get; set; }

        [DataField("CPH_C_ARQUIVO", SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }
    }
}
