using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;


namespace CMSv4.Model.Base.GestaoInformacoesExportacao
{
    [Table("MOD_GIE_GESTAO_INFORMACOES_EXPORTACAO")]
    [Serializable]
    public class MLGestaoInformacoesExportacao
    {
        [DataField("GIE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [CsvField("Proposta Comercial", 1)]
        [DataField("GIE_C_PROPOSTA_COMERCIAL", SqlDbType.VarChar, 200)]
        public string PropostaComercial { get; set; }

        [Required]
        [CsvField("Número do Booking", 2)]
        [DataField("GIE_C_NUMERO_BOOKING", SqlDbType.VarChar, 200)]
        public string NumeroBooking { get; set; }

        [Required]
        [DataField("GIE_D_DATA_ATUALIZACAO", SqlDbType.DateTime)]
        public DateTime? DataAtualizacao { get; set; }

        [JoinField("GIE_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_NOME")]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("GIE_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }
    }
}
