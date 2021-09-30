using Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    public class MLFaqView : MLFaq
    {
        public MLFaqView()
        {
            ListaCategorias = new List<MLFaqCategoria>();
        }

        public List<MLFaqCategoria> ListaCategorias { get; set; }
    }

    /// <summary>
    /// FAQ
    /// </summary>
    [Table("MOD_FAQ_FAQ")]
    public class MLFaq
    {
        [DataField("FAQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("FAQ_FCA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

        [DataField("FAQ_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [Required]
        [DataField("FAQ_C_PERGUNTA", SqlDbType.VarChar, 350)]
        public string Pergunta { get; set; }

        [Required]
        [DataField("FAQ_C_RESPOSTA", SqlDbType.VarChar, -1)]
        public string Resposta { get; set; }
        
        [DataField("FAQ_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("FAQ_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
