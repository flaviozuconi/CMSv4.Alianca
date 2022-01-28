using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model.Base
{
    /// <summary>
    /// GestaoDeCustosPenalidades
    /// </summary>
    
    [Table("MOD_GCP_GESTAO_CUSTO_PENALIDADE")]
    [Serializable]
    public class MLGestaoDeCustosPenalidades 
    {
        [DataField("GCP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [CsvField("POD", 0)]
        [DataField("GCP_C_POD", SqlDbType.VarChar, 100)]
        public string POD { get; set; }

        [Required]
        [CsvField("Tarifa adicional (R$)", 1)]
        [DataField("GCP_C_TARIFA_ADICIONAL", SqlDbType.Decimal)]
        public decimal? TarifaAdicional { get; set; }

        [Required]
        [CsvField("Valor da tarifa por extenso", 2)]
        [DataField("GCP_C_VALOR_TARIFA_ADICIONAL", SqlDbType.VarChar, 100)]
        public string ValorTarifa { get; set; }

        [Required]
        [CsvField("Penalidade Aliança (R$)", 3)]
        [DataField("GCP_C_PENALIDADE_ALIANCA", SqlDbType.Decimal)]
        public decimal? Penalidade { get; set; }

        [Required]
        [CsvField("Valor da penalidade por extenso", 4)]
        [DataField("GCP_C_VALOR_PENALIDADE_ALIANCA", SqlDbType.VarChar, 100)]
        public string ValorPenalidade { get; set; }
                 
        [DataField("GCP_D_DATA_ATUALIZACAO", SqlDbType.DateTime, IgnoreEmpty = false)]
        public DateTime? DataAtualizacao { get; set; }

        [Required]       
        [DataField("GCP_C_USUARIO", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }
      

    }
}
