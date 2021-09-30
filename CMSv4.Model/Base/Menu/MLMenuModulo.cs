using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Menu 
    /// </summary> 
    [Serializable]
    [Table("MOD_MEN_MENU")]
    public class MLMenuModulo
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MEN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [StringLength(50)]
        [DataField("MEN_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("MEN_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("MEN_C_ICONE", SqlDbType.VarChar, 100)]
        public string Icone { get; set; }

        /// <summary>
        /// Diretório onde serão gerados os arquivos físicos das views do menu
        /// </summary>
        public static class Pasta
        {
            public const string Views = "MenuPublico";
        }
    }

    [Table("MOD_MEN_MENU_VIEW")]
    public class MLMenuModuloView
    {
        [DataField("MNV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MVC_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MNV_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("MNV_C_VIEW", SqlDbType.VarChar, -1)]
        public string View { get; set; }

        [DataField("MNV_C_SCRIPTS", SqlDbType.VarChar, -1)]
        public string Script { get; set; }
    }
}


