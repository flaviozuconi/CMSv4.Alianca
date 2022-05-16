using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    [Table("MOD_AIN_AGENDAMENTO_INTERMODAL")]
    public class MLAgendamentoIntermodal
    {
        [DataField("AIN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [StringLength(150)]
        [DataField("AIN_C_NOME", SqlDbType.VarChar, 150)]
        public string Nome { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_PROPOSTA_COMERCIAL", SqlDbType.VarChar, 200)]
        public string PropostaComercial { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_NUMERO_BOOKING", SqlDbType.VarChar, 200)]
        public string NumeroBooking { get; set; }

        [StringLength(20)]
        [DataField("AIN_C_CNPJ", SqlDbType.VarChar, 20)]
        public string CNPJ { get; set; }

        [StringLength(10)]
        [DataField("AIN_C_CEP", SqlDbType.VarChar, 10)]
        public string CEP { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_ENDERECO", SqlDbType.VarChar, 200)]
        public string Endereco { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_COMPLEMENTO", SqlDbType.VarChar, 200)]
        public string Complemento { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_BAIRRO", SqlDbType.VarChar, 200)]
        public string Bairro { get; set; }

        [StringLength(200)]
        [DataField("AIN_C_CIDADE", SqlDbType.VarChar, 200)]
        public string Cidade { get; set; }

        [StringLength(2)]
        [DataField("AIN_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }

        [DataField("AIN_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

    }

    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    [Table("MOD_AIC_AGENDAMENTO_INTERMODAL_CARGA")]
    public class MLAgendamentoIntermodalCarga
    {
        [DataField("AIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIC_AIN_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoExportacao { get; set; }

        [DataField("AIC_D_DATA_COLETA", SqlDbType.DateTime)]
        public DateTime? DataColeta { get; set; }

        [DataField("AIC_N_PESO", SqlDbType.Decimal, 18)]
        public decimal? Peso { get; set; }

        [DataField("AIC_C_EQUIPAMENTO", SqlDbType.VarChar, 100)]
        public string Equipamento { get; set; }

        [DataField("AIC_C_REEFER", SqlDbType.Bit)]
        public bool? Reefer { get; set; }

        [DataField("AIC_C_COMENTARIO", SqlDbType.VarChar, 300)]
        public string Comentario { get; set; }

        [DataField("AIC_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        public string DataColetaFormatada
        {
            get
            {
                if (DataColeta.HasValue) return DataColeta.Value.ToString(BLTraducao.T("dd/MM/yyyy HH:mm"));
                return "";
            }
        }

    }
}
