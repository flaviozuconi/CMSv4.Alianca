using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    #region MLAgendamentoIntermodalImportacao
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal Importacao
    /// </summary> 
    [Serializable]
    [Table("MOD_AII_AGENDAMENTO_INTERMODAL_IMPORTACAO")]
    public class MLAgendamentoIntermodalImportacao
    {
        [DataField("AII_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [StringLength(150)]
        [DataField("AII_C_NOME", SqlDbType.VarChar, 150)]
        public string Nome { get; set; }

        [StringLength(200)]
        [DataField("AII_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [StringLength(200)]
        [DataField("AII_C_PROPOSTA_COMERCIAL", SqlDbType.VarChar, 200)]
        public string PropostaComercial { get; set; }

        [StringLength(200)]
        [DataField("AII_C_NUMERO_BOOKING", SqlDbType.VarChar, 200)]
        public string NumeroBooking { get; set; }

        [StringLength(200)]
        [DataField("AII_C_NUMERO_BL", SqlDbType.VarChar, 200)]
        public string NumeroBL { get; set; }

        [StringLength(20)]
        [DataField("AII_C_CNPJ", SqlDbType.VarChar, 20)]
        public string CNPJ { get; set; }

        [StringLength(10)]
        [DataField("AII_C_CEP", SqlDbType.VarChar, 10)]
        public string CEP { get; set; }

        [StringLength(200)]
        [DataField("AII_C_ENDERECO", SqlDbType.VarChar, 200)]
        public string Endereco { get; set; }

        [StringLength(200)]
        [DataField("AII_C_COMPLEMENTO", SqlDbType.VarChar, 200)]
        public string Complemento { get; set; }

        [StringLength(200)]
        [DataField("AII_C_BAIRRO", SqlDbType.VarChar, 200)]
        public string Bairro { get; set; }

        [StringLength(200)]
        [DataField("AII_C_CIDADE", SqlDbType.VarChar, 200)]
        public string Cidade { get; set; }

        [StringLength(2)]
        [DataField("AII_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }

        [DataField("AII_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }
    #endregion

    #region MLAgendamentoIntermodalImportacaoCarga
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    [Table("MOD_AIIC_AGENDAMENTO_INTERMODAL_IMPORTACAO_CARGA")]
    public class MLAgendamentoIntermodalImportacaoCarga
    {
        [DataField("AIIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIIC_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [DataField("AIIC_D_DATA_ENTREGA", SqlDbType.DateTime)]
        public DateTime? DataEntrega { get; set; }

        [DataField("AIIC_C_NUMERO_NFE", SqlDbType.VarChar, 50)]
        public string NumeroNfe { get; set; }

        [DataField("AIIC_N_VALOR_NFE", SqlDbType.Money)]
        public decimal? ValorNfe { get; set; }

        [DataField("AIIC_C_CONTAINER", SqlDbType.VarChar, 50)]
        public string Container { get; set; }

        [DataField("AIIC_C_ARQUIVO_ANEXO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("AIIC_C_COMENTARIO", SqlDbType.VarChar, 300)]
        public string Comentario { get; set; }

        [DataField("AIIC_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        public string DataEntregaFormatada
        {
            get
            {
                if (DataEntrega.HasValue) return DataEntrega.Value.ToString(BLTraducao.T("dd/MM/yyyy HH:mm"));
                return "";
            }
        }

        public string ValorNfeFormatado { get; set; }
        public string Sequencia { get; set; }
        public string ProximaSequencia { get; set; }
        public bool IsLinhaExcluida { get; set; }
        public string caminhoCompleto { get; set; }
    }
    #endregion

    #region RetornoMovidesk
    /// <summary>
    /// Rootobject
    /// </summary>
    public class RetornoMovidesk
    {
        public int id { get; set; }
        public object protocol { get; set; }
    }
    #endregion

    #region MLAgendamentoIntermodalImportacaoCargaVariasNf
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    public class MLAgendamentoIntermodalImportacaoCargaVariasNf
    {
        [DataField("AIIC_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [DataField("AIIC_D_DATA_ENTREGA", SqlDbType.DateTime)]
        public DateTime? DataEntrega { get; set; }

        [DataField("AIIC_C_CONTAINER", SqlDbType.VarChar, 50)]
        public string Container { get; set; }

        [DataField("AIIC_C_COMENTARIO", SqlDbType.VarChar, 300)]
        public string Comentario { get; set; }

        public List<MLAgendamentoIntermodalImportacaoCarga> LstNfs { get; set; }

        public string Sequencia { get; set; }
        public string ProximaSequencia { get; set; }

        public string DataEntregaFormatada
        {
            get
            {
                if (DataEntrega.HasValue) return DataEntrega.Value.ToString(BLTraducao.T("dd/MM/yyyy HH:mm"));
                return "";
            }
        }

        public string guid { get; set; }
    }
    #endregion

    #region MLAgendamentoIntermodalImportacaoCargaVariosContainer
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    public class MLAgendamentoIntermodalImportacaoCargaVariosContainer
    {
        [DataField("AIIC_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [DataField("AIIC_C_NUMERO_NFE", SqlDbType.VarChar, 50)]
        public string NumeroNfe { get; set; }

        [DataField("AIIC_N_VALOR_NFE", SqlDbType.Money)]
        public string ValorNfe { get; set; }

        [DataField("AIIC_C_ARQUIVO_ANEXO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        public List<MLAgendamentoIntermodalImportacaoCarga> LstContainer { get; set; }

        public string guid { get; set; }

        public string caminhoCompleto { get; set; }
    }
    #endregion
}
