using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLModuloConteudo
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("CON_C_CHAVE", SqlDbType.VarChar, 50)]
        public string Chave { get; set; }

        [DataField("CON_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("CON_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

    [Table("MOD_CON_CONTEUDO_MODULO_EDICAO")]
    public class MLModuloConteudoEdicao : MLModuloConteudo { }

    [Table("MOD_CON_CONTEUDO_MODULO_PUBLICADO")]
    public class MLModuloConteudoPublicado : MLModuloConteudo { }

    [Table("MOD_CON_CONTEUDO_MODULO_HISTORICO")]
    public class MLModuloConteudoHistorico : MLModuloConteudo
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
