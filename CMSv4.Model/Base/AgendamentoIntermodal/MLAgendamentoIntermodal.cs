using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    #region MLAgendamentoIntermodal
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

        public string NumeroBL { get; set; }
        public string Tipo { get; set; }
        public List<MLAgendamentoIntermodalImportacaoCarga> lstCarga { get; set; }

        public MLAgendamentoIntermodal()
        {
            lstCarga = new List<MLAgendamentoIntermodalImportacaoCarga>();
        }
    }
    #endregion

    #region MLAgendamentoIntermodalCarga
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

        [DataField("AIC_C_COMENTARIO", SqlDbType.VarChar, 300)]
        public string Comentario { get; set; }

        [DataField("AIC_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("AIC_C_CHAVE", SqlDbType.VarChar, 300)]
        public string Chave { get; set; }

        public string DataColetaFormatada
        {
            get
            {
                if (DataColeta.HasValue) return DataColeta.Value.ToString(BLTraducao.T("dd/MM/yyyy HH:mm"));
                return "";
            }
        }

    }
    #endregion  

    #region MLAgendamentoIntermodalLog
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    [Serializable]
    [Table("MOD_AIL_AGENDAMENTO_INTERMODAL_LOG")]
    public class MLAgendamentoIntermodalLog
    {
        [DataField("AIL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIL_C_JSON", SqlDbType.VarChar, -1)]
        public string Json { get; set; }

        [DataField("AIL_C_IMAGEM", SqlDbType.VarChar, -1)]
        public string Imagem { get; set; }

        [DataField("AIL_C_SUCESSO", SqlDbType.VarChar, -1)]
        public string RetornoAPI { get; set; }

        [DataField("AIL_C_TIPO", SqlDbType.VarChar, 100)]
        public string Tipo { get; set; }

        [DataField("AIL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("AIL_B_INTEGRADO", SqlDbType.Bit)]
        public bool? isIntegrado { get; set; }

        [DataField("AIL_B_INTEGRADO_SEGUNDA_TENTATIVA", SqlDbType.Bit)]
        public bool? isIntegradoSegundaTentativa { get; set; }
    }
    #endregion  

    #region Integração
    // https://atendimento.movidesk.com/kb/article/189/movidesk-person-api
    // https://atendimento.movidesk.com/kb/article/256/movidesk-ticket-api#h86skud04v861ectu4vnakcjuo28zts


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    /*public class Action
    {
        public int id { get; set; }
        public int type { get; set; }
        public int origin { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string justification { get; set; }
        public DateTime createdDate { get; set; }
        public CreatedBy createdBy { get; set; }
        public bool isDeleted { get; set; }
        public List<TimeAppointment> timeAppointments { get; set; }
        public List<Expense> expenses { get; set; }
        public List<Attachments> attachments { get; set; }
        public List<ParentTicket> parentTickets { get; set; }
        public List<ChildrenTicket> childrenTickets { get; set; }
        public List<SatisfactionSurveyResponse> satisfactionSurveyResponses { get; set; }
        public List<CustomFieldValue> customFieldValues { get; set; }
    }

    public class Attachments
    {
        public string fileName { get; set; }
        public string path { get; set; }
        public CreatedBy createdBy { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class ChildrenTicket
    {
        public int id { get; set; }
        public string subject { get; set; }
        public bool isDeleted { get; set; }
    }

    public class Client
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool isDeleted { get; set; }
        public Organization organization { get; set; }
    }

    public class CreatedBy
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public object address { get; set; }
        public object complement { get; set; }
        public object cep { get; set; }
        public object city { get; set; }
        public object bairro { get; set; }
        public object number { get; set; }
        public object reference { get; set; }
    }

    public class CreatedByTeam
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class CustomFieldValue
    {
        public int customFieldId { get; set; }
        public int customFieldRuleId { get; set; }
        public int line { get; set; }
        public string value { get; set; }
        public List<Item> items { get; set; }
    }

    public class Expense
    {
        public int id { get; set; }
        public string type { get; set; }
        public string serviceReport { get; set; }
        public CreatedBy createdBy { get; set; }
        public object createdByTeam { get; set; }
        public DateTime date { get; set; }
        public object quantity { get; set; }
        public double value { get; set; }
    }

    public class Item
    {
        public object personId { get; set; }
        public object clientId { get; set; }
        public object team { get; set; }
        public string customFieldItem { get; set; }
    }

    public class Organization
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class ParentTicket
    {
        public int id { get; set; }
        public string subject { get; set; }
        public bool isDeleted { get; set; }
    }

    public class ResponsedBy
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class MLAgendamentoTicket
    {
        public int id { get; set; }
        public string protocol { get; set; }
        public int type { get; set; }
        public string subject { get; set; }
        public string category { get; set; }
        public string urgency { get; set; }
        public string status { get; set; }
        public string baseStatus { get; set; }
        public string justification { get; set; }
        public int origin { get; set; }
        public DateTime createdDate { get; set; }
        public string originEmailAccount { get; set; }
        public Owner owner { get; set; }
        public string ownerTeam { get; set; }
        public CreatedBy createdBy { get; set; }
        public List<string> serviceFull { get; set; }
        public int serviceFirstLevelId { get; set; }
        public string serviceFirstLevel { get; set; }
        public string serviceSecondLevel { get; set; }
        public string serviceThirdLevel { get; set; }
        public string contactForm { get; set; }
        public List<string> tags { get; set; }
        public string cc { get; set; }
        public DateTime resolvedIn { get; set; }
        public DateTime reopenedIn { get; set; }
        public DateTime closedIn { get; set; }
        public DateTime lastActionDate { get; set; }
        public int actionCount { get; set; }
        public DateTime lastUpdate { get; set; }
        public int lifeTimeWorkingTime { get; set; }
        public int stoppedTime { get; set; }
        public int stoppedTimeWorkingTime { get; set; }
        public bool resolvedInFirstCall { get; set; }
        public string chatWidget { get; set; }
        public string chatGroup { get; set; }
        public int chatTalkTime { get; set; }
        public int chatWaitingTime { get; set; }
        public int sequence { get; set; }
        public string slaAgreement { get; set; }
        public string slaAgreementRule { get; set; }
        public int slaSolutionTime { get; set; }
        public int slaResponseTime { get; set; }
        public bool slaSolutionChangedByUser { get; set; }
        public SlaSolutionChangedBy slaSolutionChangedBy { get; set; }
        public DateTime slaSolutionDate { get; set; }
        public bool slaSolutionDateIsPaused { get; set; }
        public DateTime slaResponseDate { get; set; }
        public DateTime slaRealResponseDate { get; set; }
        public List<Client> clients { get; set; }
        public List<Action> actions { get; set; }
    }

    public class SatisfactionSurveyResponse
    {
        public int id { get; set; }
        public ResponsedBy responsedBy { get; set; }
        public DateTime responseDate { get; set; }
        public int satisfactionSurveyModel { get; set; }
        public object satisfactionSurveyNetPromoterScoreResponse { get; set; }
        public object satisfactionSurveyPositiveNegativeResponse { get; set; }
        public int satisfactionSurveySmileyFacesResponse { get; set; }
        public string comments { get; set; }
    }

    public class SlaSolutionChangedBy
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class TimeAppointment
    {
        public int id { get; set; }
        public string activity { get; set; }
        public DateTime date { get; set; }
        public string periodStart { get; set; }
        public string periodEnd { get; set; }
        public string workTime { get; set; }
        public double accountedTime { get; set; }
        public string workTypeName { get; set; }
        public CreatedBy createdBy { get; set; }
        public CreatedByTeam createdByTeam { get; set; }
    }


    #region MLAgendamentoPerson
    /// <summary>
    /// MLAgendamentoPerson
    ///  persons
    /// </summary>
    public class MLAgendamentoPerson
    {
        public string id { get; set; }
        public string codRefAdditional { get; set; }
        public bool? isActive { get; set; }
        public int? personType { get; set; }
        public int? profileType { get; set; }
        public string accessProfile { get; set; }
        public string businessName { get; set; }
        public string corporateName { get; set; }
        public string cpfCnpj { get; set; }
        public string userName { get; set; }
    }
    #endregion*/



    #region MLAgendamentoPerson
    /// <summary>
    /// MLAgendamentoPerson
    ///  persons
    /// </summary>
    public class MLAgendamentoPerson
    {
        public string id { get; set; }
        public string codRefAdditional { get; set; }
        public bool? isActive { get; set; }
        public int? personType { get; set; }
        public int? profileType { get; set; }
        public string accessProfile { get; set; }
        public string businessName { get; set; }
        public string corporateName { get; set; }
        public string cpfCnpj { get; set; }
        public string userName { get; set; }
    }
    #endregion

    #region  MLAgendamentoTicket
    /// <summary>
    /// MLAgendamentoTicket
    /// </summary>
    public class MLAgendamentoTicket
    {
        public int type                 { get; set; }
        public string subject           { get; set; }
        public List<string> serviceFull { get; set; }
        public string serviceFirstLevel { get; set; }
        public string serviceSecondLevel { get; set; }
        public string serviceThirdLevel { get; set; }
        public string category          { get; set; }
        public string urgency           { get; set; }
        public string ownerTeam         { get; set; }
       
        public DateTime createdDate { get; set; }
        public List<Client> clients { get; set; }
        public List<Action> actions { get; set; }
        //public List<Attachments> attachments { get; set; }
        public Createdby createdBy { get; set; }
        public List<Customfieldvalue> customFieldValues { get; set; }

        public MLAgendamentoTicket()
        {
            clients = new List<Client>();
            //attachments = new List<Attachments>();
            actions = new List<Action>();
            createdBy = new Createdby();
            customFieldValues = new List<Customfieldvalue>();
        }
    }
    #endregion

    #region  Createdby
    /// <summary>
    /// Createdby
    /// </summary>
    public class Createdby
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    #endregion

    #region Client
    /// <summary>
    /// Client
    /// </summary>
    public class Client
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
    }
    #endregion

    #region  Action
    /// <summary>
    /// Action
    /// </summary>
    public class Action
    {
        public int type { get; set; }
        public int origin { get; set; }
        public string description { get; set; }
        public string justification { get; set; }
        public DateTime createdDate { get; set; }

        public List<Attachments> attachments { get; set; }
    }
    #endregion

    #region Customfieldvalue
    /// <summary>
    /// Customfieldvalue
    /// </summary>
    public class Customfieldvalue
    {
        public int customFieldId { get; set; }
        public int customFieldRuleId { get; set; }
        public int line { get; set; }
        public string value { get; set; }
        public List<Item> items { get; set; }

        public Customfieldvalue()
        {
            items = new List<Item>();
        }
    }
    #endregion

    #region  Item
    /// <summary>
    /// Item
    /// </summary>
    public class Item
    {
        public object personId { get; set; }
        public object clientId { get; set; }
        public object team { get; set; }
        public string customFieldItem { get; set; }
    }
    #endregion

    #region Attachments
    /// <summary>
    /// Attachments
    /// </summary>
    public class Attachments
    {
        public string fileName { get; set; }
        public string path { get; set; }
        public Createdby createdBy { get; set; }
        public DateTime createdDate { get; set; }
    }
    #endregion

    #endregion

    #region CEP
    /// <summary>
    /// ML CEp
    /// </summary>
    public class MLCep
    {
        public string logradouro;
        public string bairro;
        public string localidade;
        public string uf;
    }
    #endregion
}
