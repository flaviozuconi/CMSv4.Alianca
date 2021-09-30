using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    public class MLClienteViewModelItem
    {
        public MLClienteViewModelItem()
        {
            Cliente = new MLCliente();
            Portais = new List<MLUsuarioItemPortal>();
            Grupos = new List<MLGrupoCliente>();
        }

        public MLCliente Cliente { get; set; }

        public List<MLUsuarioItemPortal> Portais { get; set; }

        public List<MLGrupoCliente> Grupos { get; set; }
    }

    /// <summary>   
    /// Model da Entidade Cliente
    /// </summary> 
    [Serializable]
    [Table("CMS_CLI_CLIENTE")]
    [Auditing("/cms/cliente")]
    public class MLCliente
    {
        public MLCliente()
        {
            Grupos = new List<MLClienteItemGrupo>();
            Colaborador = new MLColaborador();
        }

        [DataField("CLI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CLI_PAI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPais { get; set; }

        [DataField("CLI_EST_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEstado { get; set; }

        [Required, StringLength(100)]
        [DataField("CLI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [StringLength(200)]
        [DataField("CLI_C_LOGIN", SqlDbType.VarChar, 200)]
        public string Login { get; set; }

        [StringLength(50), DataType(DataType.Password)]
        [DataField("CLI_C_SENHA", SqlDbType.VarChar, 50, IgnoreEmpty = true)]
        public string Senha { get; set; }

        [StringLength(200), DataField("CLI_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [DataField("CLI_C_CIDADE", SqlDbType.VarChar, 300)]
        public string Cidade { get; set; }

        [DataField("CLI_D_ANIVERSARIO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataNascimento { get; set; }

        [DataField("CLI_D_CADASTRO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataCadastro { get; set; }

        [DataField("CLI_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("CLI_B_ALTERARSENHA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? AlterarSenha { get; set; }

        [DataField("CLI_C_TOKEN_NOVA_SENHA", SqlDbType.VarChar, 36)]
        public string TokenNovaSenha { get; set; }

        [StringLength(5), DataField("CLI_C_FOTO", SqlDbType.VarChar, 5)]
        public string Foto { get; set; }

        [DataField("CLI_C_TELEFONE", SqlDbType.VarChar, 50)]
        public string Telefone { get; set; }

        [DataField("CLI_C_ESTADO", SqlDbType.VarChar, 200)]
        public string Estado { get; set; }

        public List<MLClienteItemGrupo> Grupos { get; set; }

        [JoinModel("CLI_N_CODIGO", "COL_CLI_N_CODIGO", "CMS_COL_CLIENTE_COLABORADOR")]
        public MLColaborador Colaborador { get; set; }

        [DataField("CLI_C_EMPRESA", SqlDbType.VarChar, 300)]
        public string Empresa { get; set; }

        [DataField("CLI_C_CARGO", SqlDbType.VarChar, 300)]
        public string Cargo { get; set; }

        [DataField("CLI_C_ASSUNTOS_INTERESSE", SqlDbType.VarChar, -1)]
        public string AssuntosInteresse { get; set; }

        [DataField("CLI_C_DDD", SqlDbType.VarChar, 4)]
        public string DDD { get; set; }

        [DataField("CLI_B_PRE_CADASTRO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? IsPreCadastro { get; set; }

    }

    #region MLClienteItemGrupo

    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    [Table("CMS_CXG_CLIENTE_X_GRUPO")]
    public class MLClienteItemGrupo
    {
        [DataField("CXG_CLI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoCliente { get; set; }

        [DataField("CXG_GCL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }
    }

    #endregion

    #region MLClienteAlterarSenha

    /// <summary> 
    /// Model da Entidade Usuario
    /// </summary> 
    [Serializable]
    [Table("CMS_CLI_CLIENTE")]
    public class MLClienteAlterarSenha
    {
        [DataField("CLI_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? Codigo { get; set; }

        [DataField("CLI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O Campo Senha é de Preenchimento Obrigatório"), StringLength(50, MinimumLength = 6), DataType(DataType.Password)]
        [DataField("CLI_C_SENHA", SqlDbType.VarChar, 50)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O Campo Nova Senha é de Preenchimento Obrigatório"), StringLength(50, MinimumLength = 6), DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "@T O Campo Confirmar Senha é de Preenchimento Obrigatório"), StringLength(50, MinimumLength = 6), DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.CompareAttribute("NovaSenha", ErrorMessage = "Informe uma senha e digite novamente para confirmar")]
        public string ConfirmarSenha { get; set; }

        [DataField("CLI_B_ALTERARSENHA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? AlterarSenha { get; set; }

        [DataField("CLI_C_TOKEN_NOVA_SENHA", SqlDbType.VarChar, 36)]
        public string TokenNovaSenha { get; set; }
    }

    #endregion

    public class MLClienteCompleto : MLCliente
    {
    }

    public class MLClienteGrid
    {
        [DataField("CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Codigo { get; set; }

        [DataField("CLI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("CLI_C_LOGIN", SqlDbType.VarChar, 200)]
        public string Login { get; set; }

        [DataField("CLI_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [DataField("CLI_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("CLI_C_CODIGO_INTERNO", SqlDbType.VarChar, 100)]
        public string CodigoInterno { get; set; }

        [DataField("TOTAL_ACESSOS", SqlDbType.Int)]
        public int? TotalAcessos { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }
    }

    /// <summary>
    /// Model para todos os itens / listas que serão utilizados para BIND na tela
    /// </summary>
    public class MLClienteInformacoesCadastro
    {
        public MLClienteInformacoesCadastro()
        {
            Paises = new List<MLCMSPais>();
            Generos = new List<MLClienteGenero>();
            Idiomas = new List<MLClienteIdioma>();
            NivelIdiomas = new List<MLClienteNivelIdioma>();
            Areas = new List<MLClienteAreaInteresse>();
            Membros = new List<MLClienteMembro>();
        }

        public List<MLCMSPais> Paises { get; set; }
        public List<MLClienteGenero> Generos { get; set; }
        public List<MLClienteIdioma> Idiomas { get; set; }
        public List<MLClienteNivelIdioma> NivelIdiomas { get; set; }
        public List<MLClienteAreaInteresse> Areas { get; set; }
        public List<MLClienteMembro> Membros { get; set; }
    }

}
