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
    public class MLModuloFaleConosco
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("FAL_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("FAL_C_MODELO", SqlDbType.VarChar, 100)]
        public string NomeModelo { get; set; }

        [StringLength(100)]
        [DataField("FAL_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("FAL_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("FAL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("FAL_B_ANONIMO", SqlDbType.Bit)]
        public bool? Anonimo { get; set; }

        [DataField("FAL_B_ENVIA_EMAIL", SqlDbType.Bit)]
        public bool? EnviaEmail { get; set; }
    }

    [Table("MOD_FAL_FALE_CONOSCO_EDICAO")]
    public class MLModuloFaleConoscoEdicao : MLModuloFaleConosco { }

    [Table("MOD_FAL_FALE_CONOSCO_PUBLICADO")]
    public class MLModuloFaleConoscoPublicado : MLModuloFaleConosco { }

    [Table("MOD_BRK_FAL_FALE_CONOSCO_CONFIGURACAO_TESTE")]
    public class MLFaleConoscoConfiguracaoEnvioEmail
    {
        [DataField("FCE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("FCE_C_FROM", SqlDbType.VarChar, 500)]
        public string From { get; set; }

        [DataField("FCE_C_HOST", SqlDbType.VarChar, 500)]
        public string Host { get; set; }

        [DataField("FCE_N_PORT", SqlDbType.Int)]
        public int? Port { get; set; }

        [DataField("FCE_C_USERNAME", SqlDbType.VarChar, 500)]
        public string UserName { get; set; }

        [DataField("FCE_C_PASSWORD", SqlDbType.VarChar, 500)]
        public string Password { get; set; }

        [DataField("FCE_C_ENCODING", SqlDbType.VarChar, 500)]
        public string SubjectEncoding { get; set; }

        [DataField("FCE_B_DEFAULT_CREDENTIAL", SqlDbType.Bit)]
        public bool? DefaultCredential { get; set; }
    }
}
