using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    [Table("FWK_GPE_GRUPO_PERMISSAO")]
    public class MLGrupoPermissao
    {
        [DataField("GPE_GRP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = false)]
        public decimal? CodigoGrupo { get; set; }

        [DataField("GPE_FUN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = false)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("GPE_B_VISUALIZAR", SqlDbType.Bit)]
        public bool? Visualizar { get; set; }

        [DataField("GPE_B_MODIFICAR", SqlDbType.Bit)]
        public bool? Modificar { get; set; }

        [DataField("GPE_B_EXCLUIR", SqlDbType.Bit)]
        public bool? Excluir { get; set; }
    }
}


