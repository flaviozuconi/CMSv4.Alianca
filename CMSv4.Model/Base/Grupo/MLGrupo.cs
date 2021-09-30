using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    [Table("FWK_GRP_GRUPO")]
    [Auditing("/admin/grupo")]
    public class MLGrupo
    {
        public MLGrupo()
        {
            Permissoes = new List<MLGrupoItemPermissao>();
        }

        [DataField("GRP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required, StringLength(50)]
        [DataField("GRP_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("GRP_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [Display(Name = "Importar do AD")]
        [DataField("GRP_B_IMPORTA_AD", SqlDbType.Bit, IgnoreEmpty=true)]
        public bool? ImportaAD { get; set; }

        public List<MLGrupoItemPermissao> Permissoes { get; set; }
    }

    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    public class MLGrupoItemPermissao
    {
        [DataField("GPE_GRP_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoGrupo { get; set; }

        [DataField("FUN_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("FUN_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("FUN_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("FUN_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("GPE_B_VISUALIZAR", SqlDbType.Bit)]
        public bool? Visualizar { get; set; }

        [DataField("GPE_B_MODIFICAR", SqlDbType.Bit)]
        public bool? Modificar { get; set; }

        [DataField("GPE_B_EXCLUIR", SqlDbType.Bit)]
        public bool? Excluir { get; set; }
    }

    [Serializable]
    public class MLGrupoItemPermissaoForm
    {
        public decimal? CodigoFuncionalidade { get; set; }
        public int? NivelPermissao { get; set; }

    }
}
