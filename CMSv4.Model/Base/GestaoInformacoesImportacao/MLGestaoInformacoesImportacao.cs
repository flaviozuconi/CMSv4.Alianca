using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;


namespace CMSv4.Model.Base.GestaoInformacoesImportacao
{
    [Table("MOD_GII_GESTAO_INFORMACOES_IMPORTACAO")]
    [Serializable]
    public class MLGestaoInformacoesImportacao
    {
        [DataField("GII_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [CsvField("Proposta Comercial", 1)]
        [DataField("GII_C_PROPOSTA_COMERCIAL", SqlDbType.VarChar, 200)]
        public string PropostaComercial { get; set; }

        [Required]
        [CsvField("Número do Booking", 2)]
        [DataField("GII_C_NUMERO_BOOKING", SqlDbType.VarChar, 200)]
        public string NumeroBooking { get; set; }

        [Required]
        [CsvField("Número do BL", 3)]
        [DataField("GII_C_NUMERO_BL", SqlDbType.VarChar, 200)]
        public string NumeroBL { get; set; }

        [DataField("GII_D_DATA_ATUALIZACAO", SqlDbType.DateTime)]
        public DateTime? DataAtualizacao { get; set; }

        [JoinField("GII_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_NOME")]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("GII_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }
    }
}
