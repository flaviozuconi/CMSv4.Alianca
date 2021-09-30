using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model.Base
{
    [Table("MOD_ALI_SCH_SCHEDULE_HISTORICO")]
    public class MLScheduleAdminHistorico
    {
        [DataField("SCH_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("SCH_USU_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("SCH_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }
        
        [DataField("SCH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }
        
        [DataField("SCH_B_FINALIZADO", SqlDbType.Bit)]
        public bool? Finalizado { get; set; }
        
        [DataField("SCH_C_ARQUIVO", SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }
        
        [JoinField("SCH_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_NOME")]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeUsuario { get; set; }
    }
}
