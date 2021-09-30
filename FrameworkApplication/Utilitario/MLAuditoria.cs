using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    [Table("FWK_AUD_AUDITORIA")]
    public class MLAuditoria
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("AUD_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AUD_FUN_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("AUD_C_CODIGO_REFERENCIA", SqlDbType.VarChar, 200)]
        public string CodigoReferencia { get; set; }

        [DataField("AUD_D_DATA", SqlDbType.Decimal, 18)]
        public DateTime? Data { get; set; }

        [DataField("AUD_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("AUD_C_LOGIN", SqlDbType.VarChar, 200)]
        public string Login { get; set; }

        [DataField("AUD_C_ACAO", SqlDbType.VarChar, 3)]
        public string Acao { get; set; }
    }

    /// <summary> 
    /// Model da Entidade Grupo
    /// </summary> 
    [Serializable]
    [Table("FWK_AUD_AUDITORIA")]
    public class MLAuditoriaRelatorio : MLAuditoria
    {
        [JoinField("AUD_FUN_N_CODIGO", "FWK_FUN_FUNCIONALIDADE", "FUN_N_CODIGO", "FUN_C_NOME")]
        [DataField("FUN_C_NOME", SqlDbType.VarChar, 50)]
        public string Funcionalidade { get; set; }
    }
}
