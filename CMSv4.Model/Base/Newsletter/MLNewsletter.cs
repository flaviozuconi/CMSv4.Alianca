using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_NEW_NEWSLETTERS")]
    public class MLNewsletter
    {
        [DataField("NEW_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber=true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        #region Validations
        [Required(ErrorMessage="O campo é obrigatório")]
        [StringLength(250)]
        [DataField("NEW_C_NOME", SqlDbType.VarChar, 250)]
        #endregion
        public string Nome { get; set; }

        #region Validations
        [Required(ErrorMessage = "O campo é obrigatório")]
        [StringLength(250)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Campo inválido")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Campo inválido")]
        [DataField("NEW_C_EMAIL", SqlDbType.VarChar, 250)]
        #endregion
        public string Email { get; set; }

        [DataField("NEW_C_ASSUNTOS", SqlDbType.VarChar, -1)]
        public string Assuntos { get; set; }

        [DataField("NEW_D_CADASTRO", SqlDbType.DateTime)]
        [Required(ErrorMessage = "O campo é obrigatório")]
        public DateTime? DataCadastro { get; set; }

        [DataField("NEW_D_OPT_IN", SqlDbType.DateTime)]
        public DateTime? DataOptIn { get; set; }

        [DataField("NEW_D_OPT_OUT", SqlDbType.DateTime)]
        public DateTime? DataOptOut { get; set; }

        [DataField("NEW_C_GUID", SqlDbType.UniqueIdentifier)]
        public Guid? ChaveSecreta { get; set; }
    }

     /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_NEW_NEWSLETTERS")]
    [Auditing("/cms/newsletteradmin", "CodigoPortal")]
    public class MLNewsletterAdmin : MLNewsletter
    {

    }

}
