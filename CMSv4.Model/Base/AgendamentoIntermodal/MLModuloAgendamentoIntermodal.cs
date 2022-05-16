using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    public class MLModuloAgendamentoIntermodal
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public decimal? Repositorio { get; set; }

        [StringLength(500)]
        [DataField("AIN_C_URL_EXPORTACAO", SqlDbType.VarChar, 500)]
        public string UrlExportacao { get; set; }

        [StringLength(500)]
        [DataField("AIN_C_URL_IMPORTACAO", SqlDbType.VarChar, 500)]
        public string UrlImportacao { get; set; }

        [DataField("AIN_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("AIN_USU_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("AIN_C_VIEW", SqlDbType.VarChar, 200)]
        public string NomeView { get; set; }
    }

    [Table("MOD_AIN_AGENDAMENTO_INTERMODAL_EDICAO")]
    public class MLModuloAgendamentoIntermodalEdicao : MLModuloAgendamentoIntermodal { }

    [Table("MOD_AIN_AGENDAMENTO_INTERMODAL_PUBLICADO")]
    public class MLModuloAgendamentoIntermodalPublicado : MLModuloAgendamentoIntermodal { }

    [Table("MOD_AIN_AGENDAMENTO_INTERMODAL_HISTORICO")]
    public class MLModuloAgendamentoIntermodalHistorico : MLModuloAgendamentoIntermodal
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
