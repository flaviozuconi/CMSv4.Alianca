using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade HistoricoReplicacao 
    /// </summary> 
    /// <user>Gerador [1.0.0.0]</user> 
    [Table("CMS_HRE_HISTORICO_REPLICACAO")]
    public class MLHistoricoReplicacao
    {
        [DataField("HRE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("HRE_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("HRE_C_LOCAL", SqlDbType.VarChar, 500)]
        public string Local { get; set; }

        [DataField("HRE_C_ARQUIVO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("HRE_C_RESULTADO", SqlDbType.VarChar, -1)]
        public string Resultado { get; set; }

        [DataField("HRE_B_REPLICADO", SqlDbType.Bit)]
        public bool? IsReplicado { get; set; }
    }
}
