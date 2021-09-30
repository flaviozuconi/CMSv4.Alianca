using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    #region MLModuloTakeOrPay
    /// <summary>
    /// Model base modulo TakeOrPay
    /// </summary>
    public class MLModuloTakeOrPay
    {
        [Required]
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [Required]
        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("TKP_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("TKP_FAL_N_MODELO_EMAIL_ADMIN", SqlDbType.Decimal, 18)]
        public decimal? ModeloEmailAdmin { get; set; }

        [DataField("TKP_FAL_N_MODELO_EMAIL_CLIENTE", SqlDbType.Decimal, 18)]
        public decimal? ModeloEmailCliente { get; set; }

        [DataField("TKP_C_LINK", SqlDbType.VarChar, 250)]
        public string Link { get; set; }

        [DataField("TKP_C_EMAIL", SqlDbType.VarChar, -1)]
        public string Email { get; set; }
    }
    #endregion

    #region MLModuloTakeOrPayEdicao
    /// <summary>
    /// Model modulo TakeOrPay Edicação
    /// </summary>
    [Table("MOD_TKP_TAKEORPAY_EDICAO")]
    public class MLModuloTakeOrPayEdicao : MLModuloTakeOrPay { }
    #endregion

    #region MLModuloTakeOrPayPublicado
    /// <summary>
    /// Model modulo TakeOrPay Publicado
    /// </summary>
    [Table("MOD_TKP_TAKEORPAY_PUBLICADO")]
    public class MLModuloTakeOrPayPublicado : MLModuloTakeOrPay { }
    #endregion

    #region MLModuloTakeOrPayHistorico
    /// <summary>
    /// Model modulo TakeOrPay Histórico
    /// </summary>
    [Table("MOD_TKP_TAKEORPAY_HISTORICO")]
    public class MLModuloTakeOrPayHistorico : MLModuloTakeOrPay
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
    #endregion
}