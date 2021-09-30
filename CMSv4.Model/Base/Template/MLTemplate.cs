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
    [Table("CMS_TMP_TEMPLATE")]
    public class MLTemplate
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("TMP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("TMP_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required]
        [DataField("TMP_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("TMP_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("TMP_N_REPOSITORIOS", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Repositorios { get; set; }

        public string Imagem { get; set; }
    }
}
