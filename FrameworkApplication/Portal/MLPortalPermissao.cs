using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace Framework.Utilities
{
    [Serializable]
    [Table("CMS_PER_PERMISSAO_PORTAL")]
    public class MLPortalPermissao
    {
        [DataField("PER_GRP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }
    }
}
