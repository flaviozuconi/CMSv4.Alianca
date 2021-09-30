using Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    [Table("CMS_SEC_SECAO")]
    [Auditing("/cms/secao", "CodigoPortal")]
    public class MLSecao
    {
        public MLSecao()
        {
            Grupos = new List<MLSecaoItemGrupo>();
            Permissao = new List<MLSecaoPermissao>();
        }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoPortal { get; set; }

        [DataField("SEC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("SEC_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("SEC_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        public List<MLSecaoItemGrupo> Grupos { get; set; }

        public List<MLSecaoPermissao> Permissao { get; set; }
    }


    /// <summary>
    /// Grupos de gestão administrativa da seção
    /// </summary>
    [Serializable]
    [Table("CMS_SXG_SECAO_X_GRUPO")]
    public class MLSecaoItemGrupo
    {
        [DataField("SXG_SEC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoSecao { get; set; }

        [DataField("SXG_GRP_N_CODIGO", SqlDbType.Decimal, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }

        public bool? Associado { get; set; }
    }
}
