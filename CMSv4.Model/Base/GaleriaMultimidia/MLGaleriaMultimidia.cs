using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// GALERIA DE IMAGEM
    /// </summary>
    [Table("MOD_GAL_GALERIAS")]
    public class MLGaleriaMultimidia : BaseModel
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("GAL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("GAL_C_TITULO", SqlDbType.VarChar, 500)]
        public string Titulo { get; set; }

        [DataField("GAL_C_CHAMADA", SqlDbType.VarChar, 500)]
        public string Chamada { get; set; }

        [DataField("GAL_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("GAL_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("GAL_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("GAL_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("GAL_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("GAL_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }

    [Table("MOD_GAL_GALERIAS_TIPO")]
    public class MLGaleriaMultimidiaTipo
    {
        [DataField("GAT_N_CODIGO", SqlDbType.Int, PrimaryKey = true)]
        public int? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("GAT_C_TITULO", SqlDbType.VarChar, 50)]
        public string Titulo { get; set; }

        [DataField("GAT_C_CSS", SqlDbType.VarChar, 200)]
        public string Css { get; set; }

        [DataField("GAT_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
