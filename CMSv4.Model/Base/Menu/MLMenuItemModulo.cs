using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade MenuItem 
    /// </summary> 
    [Serializable]
    [Table("MOD_MEN_MENU_ITEM")]
    public class MLMenuItemModulo : IComparable<MLMenuItemModulo>
    {
        public MLMenuItemModulo()
        {            
            Filhos = new List<MLMenuItemModulo>();
        }

        [DataField("MNI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("MNI_MEN_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoMenu { get; set; }

        [Required]
        [DataField("MNI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("MNI_N_CODIGO_PAI", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPai { get; set; }

        [DataField("MNI_FUN_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("MNI_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("MNI_C_CAMINHO_PAGINA", SqlDbType.VarChar, 300)]
        public string CaminhoPagina { get; set; }

        [DataField("MNI_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }
        
        [DataField("MNI_C_IMAGEM", SqlDbType.VarChar, 300)]
        public string Imagem { get; set; }

        [DataField("MNI_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

        [DataField("MNI_B_NOVA_PAGINA", SqlDbType.Bit)]
        public bool? AbrirNovaPagina { get; set; }

        [DataField("MNI_C_ICONE", SqlDbType.VarChar, 100)]
        public string Icone { get; set; }

        [DataField("MNI_C_CSS", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        public string Css { get; set; }

        public string Href
        {
            get
            {
                return CaminhoPagina ?? "#";
            }
        }

        public List<MLMenuItemModulo> Filhos { get; set; }

        /// <summary>
        /// Ordenação dos itens
        /// </summary>
        int IComparable<MLMenuItemModulo>.CompareTo(MLMenuItemModulo other)
        {
            if (this.Ordem.HasValue && !other.Ordem.HasValue) return 1;
            if (!this.Ordem.HasValue && other.Ordem.HasValue) return -1;

            if (this.Ordem == other.Ordem) return 0;

            if (this.Ordem > other.Ordem) return 1;
            if (this.Ordem < other.Ordem) return -1;

            return 0;
        }
    }

    [Serializable]
    public class MLMenuItemAdminModulo
    {
        public decimal? CodigoAtual { get; set; }
        public List<MLMenuItemModulo> Itens { get; set; }
    }
}


