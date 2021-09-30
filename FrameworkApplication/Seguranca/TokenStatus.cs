using System.Collections.Generic;

namespace Framework.Utilities
{
    public class TokenStatus
    {
        public static readonly Dictionary<AutenticacaoTokenStatus, string> Mensagens = new Dictionary<AutenticacaoTokenStatus, string>
        {
            { AutenticacaoTokenStatus.TokenInvalido, "Token inválido" },
            { AutenticacaoTokenStatus.UsuarioInativo, "Usuário inativo" },
            { AutenticacaoTokenStatus.AcessoNegado, "Acesso negado" },
            { AutenticacaoTokenStatus.Ok, "Ok" }
        };
    }
}
