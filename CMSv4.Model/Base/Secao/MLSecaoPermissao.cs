using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// Permissão de Visualização na Área Pública
    /// </summary>
    [Serializable]
    [Table("CMS_PER_PERMISSAO_SECAO")]
    public class MLSecaoPermissao
    {
        [DataField("PER_SEC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoSecao { get; set; }

        [DataField("PER_GCL_N_CODIGO", SqlDbType.Decimal, PrimaryKey = true)]
        public decimal? CodigoGrupoCliente { get; set; }

        public bool? Associado { get; set; }
    }
}
