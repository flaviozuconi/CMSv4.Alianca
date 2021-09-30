using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    [Table("FWK_FUN_FUNCIONALIDADE")]
    [Auditing("/admin/funcionalidade")]
    public class MLFuncionalidade
    {
        public MLFuncionalidade()
        {
            Permissoes = new List<MLFuncionalidadeItemPermissao>();
        }

        [DataField("FUN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required, StringLength(50)]
        [DataField("FUN_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [Required, StringLength(100)]
        [DataField("FUN_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("FUN_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        public List<MLFuncionalidadeItemPermissao> Permissoes { get; set; }
    }

    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    public class MLFuncionalidadeItemPermissao
    {
        [DataField("GPE_GRP_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoGrupo { get; set; }

        [DataField("GPE_FUN_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("GRP_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("GRP_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("GPE_B_VISUALIZAR", SqlDbType.Bit)]
        public bool? Visualizar { get; set; }

        [DataField("GPE_B_MODIFICAR", SqlDbType.Bit)]
        public bool? Modificar { get; set; }

        [DataField("GPE_B_EXCLUIR", SqlDbType.Bit)]
        public bool? Excluir { get; set; }
    }

    [Serializable]
    public class MLFuncionalidadeItemPermissaoForm
    {
        public decimal? CodigoGrupo { get; set; }
        public int? NivelPermissao { get; set; }

    }
}
