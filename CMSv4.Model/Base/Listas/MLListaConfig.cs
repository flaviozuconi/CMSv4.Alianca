using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    /// <summary>
    /// LISTAS DE CONTEUDO
    /// </summary>
    [Serializable]
    [Table("MOD_LIS_LISTA_CONFIG")]
    public class MLListaConfig
    {
        [DataField("LIS_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [Required]
        [DataField("LIS_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("LIS_C_ICONE", SqlDbType.VarChar, 50)]
        public string Icone { get; set; }

        [DataField("LIS_C_DESCRICAO", SqlDbType.VarChar, 50)]
        public string Descricao { get; set; }

        [DataField("LIS_C_CONFIGURACAO", SqlDbType.VarChar, -1)]
        public string Configuracao { get; set; }

        [DataField("LIS_GRP_N_CODIGO_EDITOR", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoEditor { get; set; }

        [DataField("LIS_GRP_N_CODIGO_APROVADOR", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoAprovador { get; set; }

        [DataField("LIS_B_EXIBIR_SIDEBAR", SqlDbType.Bit)]
        public bool? ExibirSideBar { get; set; }
    }


    /// <summary>
    /// LISTAS DE CONTEUDO CATEGORIA
    /// </summary>
    [Serializable]
    [Table("MOD_LIS_LISTA_CONFIG_CATEGORIAS")]
    public class MLListaConfigCategoria
    {
        [DataField("LIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber=true)]
        public decimal? Codigo { get; set; }

        [DataField("LIC_LIS_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoLista { get; set; }

        [Required]
        [DataField("LIC_C_NOME_CATEGORIA", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("LIC_CAA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoriaAgrupador { get; set; }
    }
}
