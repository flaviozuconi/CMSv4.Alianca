using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    public class MLModuloMultimidia
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("MUL_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("MUL_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("MUL_MUL_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoArquivo { get; set; }
    }

    [Table("MOD_MUL_MULTIMIDIA_EDICAO")]
    public class MLModuloMultimidiaEdicao : MLModuloMultimidia { }

    [Table("MOD_MUL_MULTIMIDIA_PUBLICADO")]
    public class MLModuloMultimidiaPublicado : MLModuloMultimidia { }

    [Table("MOD_MUL_MULTIMIDIA_PUBLICADO")]
    public class MLModuloMultimidiaHistorico : MLModuloMultimidia 
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
