using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// PAGINA
    /// </summary>
    [Serializable]
    [Table("CMS_PAG_PAGINA")]
    public class MLPaginaAdmin : BaseModel
    {
        public MLPaginaAdmin()
        {
            PaginaEdicao = new MLPaginaEdicao();
            PaginaPublicada = new MLPaginaPublicada();
            Seo = new MLPaginaSeo();
        }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("PAG_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required]
        [DataField("PAG_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("PAG_SEC_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoSecao { get; set; }

        [DataField("PAG_IDI_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("PAG_PAG_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPai { get; set; }


        [Required]
        [DataField("PAG_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("PAG_B_EXCLUIDA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? Excluida { get; set; }

        [DataField("PAG_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        [DataField("PAG_B_HTTPS", SqlDbType.Bit)]
        public bool? Https { get; set; }

        public MLPaginaEdicao PaginaEdicao { get; set; }

        public MLPaginaPublicada PaginaPublicada { get; set; }

        public MLPaginaSeo Seo { get; set; }
    }
}
