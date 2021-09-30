using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    public class MLModuloDicionario
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("DIC_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("DIC_C_GRUPOS", SqlDbType.VarChar, 100)]
        public string Grupos { get; set; }

        [DataField("DIC_B_ORDEM_ALFABETICA", SqlDbType.Bit)]
        public bool? IsOrdem { get; set; }

    }

    [Table("MOD_DIC_DICIONARIO_EDICAO")]
    public class MLModuloDicionarioEdicao : MLModuloDicionario { }

    [Table("MOD_DIC_DICIONARIO_PUBLICADO")]
    public class MLModuloDicionarioPublicado : MLModuloDicionario { }

    [Table("MOD_DIC_DICIONARIO_HISTORICO")]
    public class MLModuloDicionarioHistorico : MLModuloDicionario
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

    }
}
