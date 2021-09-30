using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("CMS_PER_PERMISSAO_PAGINA")]
    public class MLPaginaPermissao
    {
        [DataField("PER_PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("PER_GCL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }
    }
}
