using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade 
    /// </summary> 
    [Serializable]
    [Table("FWK_IDI_IDIOMA")]
    public class MLIdioma
    {
        [DataField("IDI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("IDI_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("IDI_C_SIGLA", SqlDbType.VarChar, 5)]
        public string Sigla { get; set; }

        [DataField("IDI_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        // ATIVO NA NOVA AREA DOS CADASTROS DE PRODUTOS
        [DataField("IDI_PRO_B_ATIVO", SqlDbType.Bit, IgnoreEmpty = true )] 
        public bool? AtivoProduto { get; set; }

        [DataField("IDI_C_FLAG", SqlDbType.VarChar, 50)]
        public string Flag { get; set; }

        public bool Iniciado { get; set; }
    }
    public class MLIdiomaPagina : MLIdioma
    {
        public decimal? CodigoPagina { get; set; }

        public string urlPagina { get; set; }
    }
}
