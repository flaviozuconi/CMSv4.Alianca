using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMSv4.Model
{
    #region MLTakeOrPayEmbarqueCerto

    /// <summary>
    /// MOdel embaruqe certo
    /// </summary>
    [Table("MOD_TPE_TAKE_OR_PAY_EMBARQUE_CERTO")]
    public class MLTakeOrPayEmbarqueCerto
    {
        [Required]
        [DataField("TPE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("TPE_D_DATA_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("TPE_C_NOME", SqlDbType.VarChar, 150)]
        public string Nome { get; set; }

        [DataField("TPE_C_EMAIL", SqlDbType.VarChar, 100)]
        public string Email { get; set; }

        [DataField("TPE_C_CNPJ", SqlDbType.VarChar, 20)]
        public string CNPJ { get; set; }

        [DataField("TPE_C_CEP", SqlDbType.VarChar, 15)]
        public string CEP { get; set; }

        [DataField("TPE_C_LOGADOURO", SqlDbType.VarChar, 50)]
        public string Logadouro { get; set; }

        [DataField("TPE_C_BAIRRO", SqlDbType.VarChar, 50)]
        public string Bairro { get; set; }

        [DataField("TPE_C_CIDADE", SqlDbType.VarChar, 100)]
        public string Cidade { get; set; }

        [DataField("TPE_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }

        [DataField("TPE_B_IS_BID", SqlDbType.Bit)]
        public bool? isBID { get; set; }

        [DataField("TPE_B_RESERVAR_ESPACO", SqlDbType.Bit)]
        public bool? ReservarEspaco { get; set; }

        [DataField("TPE_B_IS_SEMANAL", SqlDbType.Bit)]
        public bool? IsSemanal { get; set; }

        [DataField("TPE_B_TERMO_ACEITO", SqlDbType.Bit)]
        public bool? TermoAceito { get; set; }
    }
    #endregion

    #region MLTakeOrPayEmbarqueCertoXProposta

    /// <summary>
    /// Model Embaruqe X Proposta
    /// </summary>
    [Table("MOD_TEP_TAKE_OR_PAY_EMBARQUE_CERTO_X_PROPOSTA")]
    public class MLTakeOrPayEmbarqueCertoXProposta
    {
        [Required]
        [DataField("TEP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("TEP_D_DATA_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [Required]
        [DataField("TEP_TPE_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoTakeOrPayEmbarqueCerto { get; set; }

        [Required]
        [DataField("TEP_N_NUMERO_PROPSTA", SqlDbType.Decimal, 18, 0)]
        public decimal? NumeroProposta { get; set; }
    }
    #endregion

    #region MLTakeOrPayEmbarqueCertoXContainers

    /// <summary>
    /// Model Embarque X Container
    /// </summary>
    [Table("MOD_TEC_TAKE_OR_PAY_EMBARQUE_CERTO_X_CONTAINERS")]
    public class MLTakeOrPayEmbarqueCertoXContainers
    {
        [Required]
        [DataField("TEC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("TEC_TPE_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEmbarqueCerto { get; set; }

        [DataField("TEC_C_NAVIO_VIAGEM", SqlDbType.VarChar, 150)]
        public string NavioViagem { get; set; }

        [DataField("TEC_C_NAVIO_VIAGEM_OPCAO_2", SqlDbType.VarChar, 150)]
        public string NavioViagemOp2 { get; set; }

        [DataField("TEC_C_NAVIO_VIAGEM_OPCAO_3", SqlDbType.VarChar, 150)]
        public string NavioViagemOp3 { get; set; }

        [DataField("TEC_C_TAMANHO_CONTAINER", SqlDbType.VarChar, 3)]
        public string TamanhoContainer { get; set; }

        [DataField("TEC_C_TIPO_CONTAINER", SqlDbType.VarChar, 20)]
        public string TipoContainer { get; set; }

        [DataField("TEC_N_TONELAGEM_MEDIA", SqlDbType.Decimal, 18)]
        public decimal? TonelagemMedia { get; set; }

        [DataField("TEC_N_RIG", SqlDbType.Decimal, 18)]
        public decimal? RIG { get; set; }

        [DataField("TEC_N_IBB", SqlDbType.Decimal, 18)]
        public decimal? IBB { get; set; }

        [DataField("TEC_N_IOA", SqlDbType.Decimal, 18)]
        public decimal? IOA { get; set; }

        [DataField("TEC_N_SSZ", SqlDbType.Decimal, 18)]
        public decimal? SSZ { get; set; }

        [DataField("TEC_N_SPB", SqlDbType.Decimal, 18)]
        public decimal? SPB { get; set; }

        [DataField("TEC_N_VIX", SqlDbType.Decimal, 18)]
        public decimal? VIX { get; set; }

        [DataField("TEC_N_SSA", SqlDbType.Decimal, 18)]
        public decimal? SSA { get; set; }

        [DataField("TEC_N_SUA", SqlDbType.Decimal, 18)]
        public decimal? SUA { get; set; }

        [DataField("TEC_N_PEC", SqlDbType.Decimal, 18)]
        public decimal? PEC { get; set; }
    }
    #endregion

    #region MLTakeOrPayEmbarqueCertoCompleto

    /// <summary>
    /// Model Embarque Completo
    /// </summary>
    public class MLTakeOrPayEmbarqueCertoCompleto : MLTakeOrPayEmbarqueCerto
    {
        public MLTakeOrPayEmbarqueCertoCompleto()
        {
            lstProposta = new List<MLTakeOrPayEmbarqueCertoXProposta>();
            lstContainer = new List<MLTakeOrPayEmbarqueCertoXContainers>();
        }

        public List<MLTakeOrPayEmbarqueCertoXProposta> lstProposta { get; set; }
        public List<MLTakeOrPayEmbarqueCertoXContainers> lstContainer { get; set; }
    }
    #endregion

    #region Portos

    /// <summary>
    /// Nome completo Portos
    /// </summary>
    public class Portos
    {
        public string MAO { get { return "Manaus"; } }
        public string RIG { get { return "Rio Grande"; } }
        public string IBB { get { return "Imbituba"; } }
        public string IOA { get { return "Itapoá"; } }
        public string SSZ { get { return "Santos"; } }
        public string SPB_1 { get { return "Itaguaí"; } }
        public string SPB_2 { get { return "Sepetiba"; } }
        public string VIX { get { return "Vitória"; } }
        public string SSA { get { return "Salvador"; } }
        public string SUA { get { return "Suape"; } }
        public string PEC { get { return "Pecém"; } }
    }
    #endregion
}