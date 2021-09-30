using System;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{

    /// <summary>
    /// Pagina Completa de Histórico
    /// </summary>
    [Table("CMS_PAG_PAGINA_HISTORICO")]
    public class MLPaginaHistorico
    {
        public MLPaginaHistorico()
        {
            Modulos = new List<MLPaginaModuloHistorico>();
        }

        public List<MLPaginaModuloHistorico> Modulos { get; set; }

        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("HIS_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Codigo { get; set; }

        [DataField("PAG_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("PAG_C_DESCRICAO", SqlDbType.VarChar, 250)]
        public string Descricao { get; set; }

        [DataField("PAG_C_TAGS", SqlDbType.VarChar, 250)]
        public string Tags { get; set; }
                
        [DataField("PAG_C_NOME_LAYOUT", SqlDbType.VarChar, 100)]
        public string NomeLayout { get; set; }

        [DataField("PAG_C_NOME_TEMPLATE", SqlDbType.VarChar, 100)]
        public string NomeTemplate { get; set; }

        [DataField("PAG_C_TEMPLATE", SqlDbType.VarChar, -1)]
        public string TemplateCustomizado { get; set; }

        [DataField("PAG_C_SCRIPT", SqlDbType.VarChar, -1), System.Web.Mvc.AllowHtml]
        public string Scripts { get; set; }

        [DataField("PAG_C_CSS", SqlDbType.VarChar, -1), System.Web.Mvc.AllowHtml]
        public string Css { get; set; }

        [DataField("PAG_B_APRESENTAR_NA_BUSCA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? ApresentarNaBusca { get; set; }

        [DataField("PAG_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? UsuarioPublicador { get; set; }

        [DataField("PAG_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataPublicacao { get; set; }
    }

    [Table("CMS_PAG_PAGINA_HISTORICO_MODULO")]
    public class MLPaginaModuloHistorico
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("PAG_MOD_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoModulo { get; set; }

        [JoinField("PAG_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_C_URL")]
        [DataField("MOD_C_URL", SqlDbType.VarChar, 50)]
        public string UrlModulo { get; set; }

        [JoinField("PAG_MOD_N_CODIGO", "CMS_MOD_MODULO", "MOD_N_CODIGO", "MOD_B_EDITAVEL")]
        [DataField("MOD_B_EDITAVEL", SqlDbType.Bit)]
        public bool Editavel { get; set; }
    }

}
