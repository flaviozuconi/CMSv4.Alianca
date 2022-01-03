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
    [Table("MOD_FAL_FALE_CONOSCO_MENSAGENS")]
    public class MLFaleConoscoPadrao
    {
        [DataField("FAL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber=true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        #region Validations
        [Required(ErrorMessage="O campo é obrigatório")]
        [StringLength(250)]
        [DataField("FAL_C_NOME", SqlDbType.VarChar, 250)]
        #endregion
        public string Nome { get; set; }

        #region Validations
        [Required(ErrorMessage = "O campo é obrigatório")]
        [StringLength(250)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Campo inválido")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Campo inválido")]
        [DataField("FAL_C_EMAIL", SqlDbType.VarChar, 250)]
        #endregion
        public string Email { get; set; }

        [DataField("FAL_C_ASSUNTO", SqlDbType.VarChar, -1)]
        public string Assunto { get; set; }

        [DataField("FAL_C_DESTINATARIO", SqlDbType.VarChar, -1)]
        public string Destinatario { get; set; }

        [DataField("FAL_N_DDD", SqlDbType.Int)]
        public int? DDD { get; set; }

        [DataField("FAL_N_FONE", SqlDbType.Decimal)]
        public decimal? Fone { get; set; }

        #region Validations
        [Required(ErrorMessage = "O campo é obrigatório")]
        [DataField("FAL_C_MENSAGEM", SqlDbType.VarChar, -1)]
        #endregion
        public string Mensagem { get; set; }

        [DataField("FAL_C_OUTROSCAMPOS", SqlDbType.VarChar, -1)]
        public string XML { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int)]
        public int? Repositorio { get; set; }

        [DataField("FAL_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("FAL_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

    }

    [Serializable]
    [Table("VIEW_FORM_MENSAGENS")]
    public class MLFaleConoscoExportar
    {
        [DataField("FAL_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoPortal { get; set; }

        [CsvField("Nome Completo", 1)]
        [DataField("NOME", SqlDbType.VarChar, 1000)]
        public string Nome { get; set; }

        [CsvField("Empresa", 2)]
        [DataField("EMPRESA", SqlDbType.VarChar, 1000)]
        public string Empresa { get; set; }

        [CsvField("E-mail Corporativo", 3)]
        [DataField("EMAIL", SqlDbType.VarChar, 1000)]
        public string Email { get; set; }

        [CsvField("Celular", 4)]
        [DataField("CELULAR", SqlDbType.VarChar, 1000)]
        public string Celular { get; set; }

        [CsvField("Mensagem", 5)]
        [DataField("MENSAGEM", SqlDbType.VarChar, 1000)]
        public string Mensagem { get; set; }

        [CsvField("Source", 6)]
        [DataField("SOURCE", SqlDbType.VarChar, 1000)]
        public string Source { get; set; }

        [CsvField("Midia", 7)]
        [DataField("MIDIA", SqlDbType.VarChar, 1000)]
        public string Midia { get; set; }

        [CsvField("Campanha", 8)]
        [DataField("CAMPANHA", SqlDbType.VarChar, 1000)]
        public string Campanha { get; set; }

        [CsvField("Anuncio", 9)]
        [DataField("ANUNCIO", SqlDbType.VarChar, 1000)]
        public string Anuncio { get; set; }

        [CsvField("Data", 0,Format = "dd/MM/yyyy HH:mm")]
        [DataField("DATA", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [CsvField("Assunto", 10)]
        [DataField("ASSUNTO", SqlDbType.VarChar, 1000)]
        public string Assunto { get; set; }

        [CsvField("Assunto", 11)]
        [DataField("URLPAGINA", SqlDbType.VarChar, 1000)]
        public string UrlPagina { get; set; }
    }

    public class faleconosco
    {
        public string nome { get; set; }

        public string cpf { get; set; }

        public string cnpj { get; set; }

        public string empresa { get; set; }

        public string email { get; set; }

        public string cargo { get; set; }

        public string newsletter { get; set; }

        public string telefone { get; set; } 
        
        public DateTime DataCadastro { get; set; }

    }
}
