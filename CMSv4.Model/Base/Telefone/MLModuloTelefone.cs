using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    [Table("MOD_TEL_TELEFONES_EDICAO")]
    public class MLModuloTelefoneEdicao
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("TEL_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("TEL_C_CATEGORIAS", SqlDbType.VarChar, 100)]
        public string Regioes { get; set; }

        [DataField("TEL_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("TEL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

    [Table("MOD_TEL_TELEFONES_PUBLICADO")]
    public class MLModuloTelefonePublicado : MLModuloTelefoneEdicao { }

    [Table("MOD_TEL_TELEFONES_HISTORICO")]
    public class MLModuloTelefoneHistorico : MLModuloTelefoneEdicao
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

    }
}


