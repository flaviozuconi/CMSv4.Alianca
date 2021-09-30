using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model.Base
{
    [Table("MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS")]
    [Serializable]
    public class MLProgramacaoNavio
    {
        [DataField("PRN_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [CsvField("Origem",0)]
        [DataField("PRN_C_ORIGEM", SqlDbType.VarChar, 150, IgnoreEmpty = false)]
        public string Origem { get; set; }

        [CsvField("Saida - Previsto",1)]
        [DataField("PRN_D_SAIDA_PREVISTO", SqlDbType.DateTime)]
        public DateTime? SaidaPrevisto { get; set; }

        [CsvField("Saida - Realizado", 2)]
        [DataField("PRN_D_SAIDA_REALIZADO", SqlDbType.DateTime)]
        public DateTime? SaidaRealizado { get; set; }

        [CsvField("Destino",3)]
        [DataField("PRN_C_DESTINO", SqlDbType.VarChar, 150)]
        public string Destino { get; set; }

        [CsvField("Chegada - Previsto", 4)]
        [DataField("PRN_D_CHEGADA_PREVISTO", SqlDbType.DateTime)]
        public DateTime? ChegadaPrevisto { get; set; }

        [CsvField("Chegada - Realizado", 5)]
        [DataField("PRN_D_CHEGADA_REALIZADO", SqlDbType.DateTime)]
        public DateTime? ChegadaRealizado { get; set; }

        [CsvField("Navio/Viagem", 6)]
        [DataField("PRN_C_NAVIO_VIAGEM", SqlDbType.VarChar, 150, IgnoreEmpty = false)]
        public string NavioViagem { get; set; }

        [CsvField("Transit Time",7)]
        [DataField("PRN_C_TRANSIT_TIME", SqlDbType.VarChar, 20)]
        public string TransitTime { get; set; }

        [CsvField("Deadline", 8)]
        [DataField("PRN_D_DEADLINE", SqlDbType.DateTime, IgnoreEmpty = false)]
        public DateTime? Deadline { get; set; }

        [CsvField("Navio Transbordo 1", 9)]
        [DataField("PRN_C_NAVIO_TRANSBORDO_1", SqlDbType.VarChar, 150)]
        public string NavioTransbordo1 { get; set; }

        [CsvField("Porto Transbordo 1",10)]
        [DataField("PRN_C_PORTO_TRANSBORDO_1", SqlDbType.VarChar, 100)]
        public string PortoTransbordo1 { get; set; }

        [CsvField("Transit Time Transbordo 1", 11)]
        [DataField("PRN_C_TRANSIT_TIME_TRANSBORDO_1", SqlDbType.VarChar, 20)]
        public string TransitTimeTransbordo1 { get; set; }

        [CsvField("Chegada Transbordo - Previsto 1", 12)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_PREVISTO_1", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoPrevisto1 { get; set; }

        [CsvField("Chegada Transbordo - Realizado 1", 13)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_REALIZADO_1", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoRealizado1 { get; set; }

        [CsvField("Navio Transbordo 2", 14)]
        [DataField("PRN_C_NAVIO_TRANSBORDO_2", SqlDbType.VarChar, 150)]
        public string NavioTransbordo2 { get; set; }

        [CsvField("Porto Transbordo 2", 15)]
        [DataField("PRN_C_PORTO_TRANSBORDO_2", SqlDbType.VarChar, 100)]
        public string PortoTransbordo2 { get; set; }

        [CsvField("Transit Time Transbordo 2", 16)]
        [DataField("PRN_C_TRANSIT_TIME_TRANSBORDO_2", SqlDbType.VarChar, 20)]
        public string TransitTimeTransbordo2 { get; set; }

        [CsvField("Chegada Transbordo - Previsto 2", 17)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_PREVISTO_2", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoPrevisto2 { get; set; }

        [CsvField("Chegada Transbordo - Realizado 2", 18)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_REALIZADO_2", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoRealizado2 { get; set; }

        [CsvField("Navio Transbordo 3", 19)]
        [DataField("PRN_C_NAVIO_TRANSBORDO_3", SqlDbType.VarChar, 150)]
        public string NavioTransbordo3 { get; set; }

        [CsvField("Porto Transbordo 3", 20)]
        [DataField("PRN_C_PORTO_TRANSBORDO_3", SqlDbType.VarChar, 100)]
        public string PortoTransbordo3 { get; set; }

        [CsvField("Transit Time Transbordo 3", 21)]
        [DataField("PRN_C_TRANSIT_TIME_TRANSBORDO_3", SqlDbType.VarChar, 20)]
        public string TransitTimeTransbordo3 { get; set; }

        [CsvField("Chegada Transbordo - Previsto 3", 22)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_PREVISTO_3", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoPrevisto3 { get; set; }

        [CsvField("Chegada Transbordo - Realizado 3", 23)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_REALIZADO_3", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoRealizado3 { get; set; }

        [CsvField("Navio Transbordo 4", 24)]
        [DataField("PRN_C_NAVIO_TRANSBORDO_4", SqlDbType.VarChar, 150)]
        public string NavioTransbordo4 { get; set; }

        [CsvField("Porto Transbordo 4", 25)]
        [DataField("PRN_C_PORTO_TRANSBORDO_4", SqlDbType.VarChar, 100)]
        public string PortoTransbordo4 { get; set; }

        [CsvField("Transit Time Transbordo 4", 26)]
        [DataField("PRN_C_TRANSIT_TIME_TRANSBORDO_4", SqlDbType.VarChar, 20)]
        public string TransitTimeTransbordo4 { get; set; }

        [CsvField("Chegada Transbordo - Previsto 4", 27)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_PREVISTO_4", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoPrevisto4 { get; set; }

        [CsvField("Chegada Transbordo - Realizado 4", 28)]
        [DataField("PRN_D_CHEGADA_TRANSBORDO_REALIZADO_4", SqlDbType.DateTime)]
        public DateTime? ChegadaTransbordoRealizado4 { get; set; }

        [DataField("PRN_C_USUARIO_IMPORTACAO", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("PRN_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }
    }

    /// <summary>
    /// Model de retorno para listar a deadline embarque certo
    /// </summary>
    public class MLProgramacaoNavioProc : MLProgramacaoNavio
    {
        [DataField("DEADLINE_ATIVA", SqlDbType.Bit)]
        public bool? DeadlineAtiva { get; set; }
    }
}
