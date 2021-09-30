using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    /// <summary>
    /// Categoria de FAQ
    /// </summary>
    [Table("MOD_FCA_FAQ_CATEGORIA")]
    public class MLFaqCategoria
    {
        [DataField("FCA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("FCA_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("FCA_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [Required]
        [DataField("FCA_DESCRICAO", SqlDbType.VarChar, 350)]
        public string Titulo { get; set; }

        [DataField("FCA_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("FCA_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
