using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    #region MLIntegrar
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal
    /// </summary> 
    public class MLIntegrar
    {
        public string id { get; set; }
        public object codeReferenceAdditional { get; set; }
        public bool isActive { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string accessProfile { get; set; }
        public string businessName { get; set; }
        public object businessBranch { get; set; }
        public object corporateName { get; set; }
        public object cpfCnpj { get; set; }
        public object userName { get; set; }
        public object role { get; set; }
        public object bossId { get; set; }
        public object bossName { get; set; }
        public object classification { get; set; }
        public string cultureId { get; set; }
        public string timeZoneId { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public DateTime changedDate { get; set; }
        public string changedBy { get; set; }
        public object observations { get; set; }
        public object authenticateOn { get; set; }
        public List<object> addresses { get; set; }
        public List<object> contacts { get; set; }
        public List<Email> emails { get; set; }
        public List<object> teams { get; set; }
        public List<Relationship> relationships { get; set; }
        public List<object> customFieldValues { get; set; }
        public List<object> atAssets { get; set; }
    }
    #endregion

    public class Email
    {
        public string emailType { get; set; }
        public string email { get; set; }
        public bool isDefault { get; set; }
    }

    public class Relationship
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slaAgreement { get; set; }
        public bool forceChildrenToHaveSomeAgreement { get; set; }
        public bool allowAllServices { get; set; }
        public object includeInParents { get; set; }
        public object loadChildOrganizations { get; set; }
        public List<object> services { get; set; }
        public bool isGetMethod { get; set; }
    }

}
