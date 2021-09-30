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
    [Table("CMS_LAY_LAYOUT")]
    public class MLLayout
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("LAY_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("LAY_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required]
        [DataField("LAY_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("LAY_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("LAY_D_CADASTRO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataCadastro { get; set; }

        [DataField("LAY_USU_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoUsuario { get; set; }

        public string Imagem { get; set; }
    }
}
