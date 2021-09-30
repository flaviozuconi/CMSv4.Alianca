using System;
using System.Data;
using Framework.Model;

namespace Framework.Utilities
{
    /// <summary>
    /// Classe principal para as models do projeto seguirem
    /// Os campos na tabela podem seguir sempre essa nomenclatura para facilitar processos
    /// de auditoria
    /// </summary>
    public class BaseModel
    {
        [DataField("LOG_D_CADASTRO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? LogDataCadastro { get; set; }

        [DataField("LOG_USU_N_CODIGO_CADASTRO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? LogUsuarioCadastro { get; set; }

        [DataField("LOG_D_ALTERACAO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? LogDataAlteracao { get; set; }

        [DataField("LOG_USU_N_CODIGO_ALTERACAO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? LogUsuarioAlteracao { get; set; }


        public void LogPreencher(bool isAlteracao)
        {
            if (isAlteracao)
            {
                this.LogDataAlteracao = DateTime.Now;
                this.LogUsuarioAlteracao = BLUsuario.ObterLogado().Codigo;
            }
            else
            {
                this.LogDataCadastro = DateTime.Now;
                this.LogUsuarioCadastro = BLUsuario.ObterLogado().Codigo;
            }
        }
    }
}
