using System;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    [Table("CMS_PAG_PAGINA_EDICAO")]
    public class MLPaginaEdicao : MLPaginaCompleta
    {
        public MLPaginaEdicao()
        {
            Modulos = new List<MLPaginaModuloEdicao>();
        }

        public List<MLPaginaModuloEdicao> Modulos { get; set; }
    }

    [Table("CMS_PAG_PAGINA_PUBLICACAO")]
    public class MLPaginaPublicada : MLPaginaCompleta
    {
        public MLPaginaPublicada()
        {
            Modulos = new List<MLPaginaModuloPublicado>();
            PageSpeed = new MLPaginaPageSpeed();
        }

        public List<MLPaginaModuloPublicado> Modulos { get; set; }

        public MLPaginaPageSpeed PageSpeed { get; set; }
    }


    /// <summary>
    /// Pagina Completa
    /// </summary>
    public class MLPaginaCompleta
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
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
        public decimal? UsuarioEditor { get; set; }

        [DataField("PAG_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataEdicao { get; set; }

        [DataField("PAG_C_URL_LOGIN", SqlDbType.VarChar, 500)]
        public string UrlLogin { get; set; }
    }
}
