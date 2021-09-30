using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    [Table("FWK_GXU_GRUPO_USUARIO")]
    public class MLUsuarioGrupo
    {
        [DataField("GXU_USU_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = false)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("GXU_GRP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = false)]
        public decimal? CodigoGrupo { get; set; }
    }
}
