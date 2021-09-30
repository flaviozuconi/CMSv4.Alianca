using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ALI_SCH_SCHEDULE")]
    public class MLSchedule
    {
        [DataField("SCH_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("SCH_C_TIPO", SqlDbType.VarChar, 1)]
        public string Tipo { get; set; }

        [CsvField("Origem", 0)]
        [DataField("SCH_C_ORIGEM", SqlDbType.VarChar, 50)]
        public string Origem { get; set; }

        [CsvField("Destino", 1)]
        [DataField("SCH_C_DESTINO", SqlDbType.VarChar, 50)]
        public string Destino { get; set; }

        [CsvField("Serviços", 2)]
        [DataField("SCH_C_SERVICOS", SqlDbType.VarChar, 50)]
        public string Servicos { get; set; }

        [CsvField("Mapa", 3)]
        [DataField("SCH_C_MAPA", SqlDbType.VarChar, 50)]
        public string Mapa { get; set; }

        [CsvField("Tempo", 4)]
        [DataField("SCH_N_TEMPO", SqlDbType.Int)]        
        public int? Tempo { get; set; }

        [CsvField("Deadline", 5)]
        [DataField("SCH_C_DEADLINE", SqlDbType.VarChar, 20)]
        public string DeadLine { get; set; }

        [CsvField("Deadline - horário", 6)]
        [DataField("SCH_C_DEADLINE_HORARIO", SqlDbType.VarChar, 10)]
        public string DeadLineHorario { get; set; }

        [CsvField("ETA", 7)]
        [DataField("SCH_C_ETA", SqlDbType.VarChar, 20)]
        public string Eta { get; set; }

        [CsvField("ETS", 8)]
        [DataField("SCH_C_ETS", SqlDbType.VarChar, 20)]
        public string Ets { get; set; }
        
        public string TempoDisplay
        {
            get;set;
        }
        
    }

    [Serializable]
    public class MLScheduleRetorno
    {

        public MLScheduleRetorno()
        {
            schedule = new List<MLSchedule>();
        }
        public string Origem { get; set; }

        public List<MLSchedule> schedule {get;set;} 
    }

    [Table("MOD_ALI_SCH_SCHEDULE")]
    [Serializable]
    public class MLScheduleDataTable
    {
        [Required]
        [DataField("SCH_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("SCH_C_ORIGEM", SqlDbType.VarChar, 50)]
        public string Origem { get; set; }

        [DataField("SCH_C_DESTINO", SqlDbType.VarChar, 50)]
        public string Destino { get; set; }

        [DataField("SCH_C_SERVICOS", SqlDbType.VarChar, 50)]
        public string Servicos { get; set; }

        [DataField("SCH_N_TEMPO", SqlDbType.Int)]
        public int? Tempo { get; set; }

        [DataField("SCH_C_DEADLINE", SqlDbType.VarChar, 20)]
        public string DeadLine { get; set; }

        [DataField("SCH_C_DEADLINE_HORARIO", SqlDbType.VarChar, 10)]
        public string DeadLineHorario { get; set; }

        [DataField("SCH_C_ETA", SqlDbType.VarChar, 20)]
        public string Eta { get; set; }

        [DataField("SCH_C_ETS", SqlDbType.VarChar, 20)]
        public string Ets { get; set; }

        [DataField("sql_total", SqlDbType.Int)]
        public int? Total { get; set; }
    }
}
