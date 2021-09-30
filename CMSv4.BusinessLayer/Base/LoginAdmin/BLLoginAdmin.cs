using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace CMSv4.BusinessLayer
{
    public class BLLoginAdmin
    {
        public MLResultBase LogarPorToken(string token)
        {
            var retorno = new MLResultBase()
            {
                Sucesso = true
            };

            try
            {
                var statusRetorno = AutenticacaoTokenStatus.Ok;
                var codigoUsuarioAutenticado = BLUsuario.AutenticarUsuarioToken(token, out statusRetorno);

                if (statusRetorno == AutenticacaoTokenStatus.Ok)
                    return retorno;
                
                retorno.Mensagem = TokenStatus.Mensagens[statusRetorno];
                retorno.Sucesso = false;

                return retorno;
                
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw ex;
            }
        }

        public bool AlterarSenha(MLUsuarioAlterarSenha model)
        {
            if (!string.IsNullOrEmpty(model.TokenNovaSenha))
            {
                var statusRetorno = AutenticacaoTokenStatus.Ok;

                model.Codigo = BLUsuario.AutenticarUsuarioToken(model.TokenNovaSenha, out statusRetorno);
                model.Senha = BLEncriptacao.EncriptarSenha(model.Senha);
                model.AlterarSenha = false;
                model.TokenNovaSenha = string.Empty;

                CRUD.SalvarParcial(model);

                if (model.Codigo.HasValue)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("USU" + Convert.ToString(model.Codigo.Value), false, 60));
                    FormsAuthentication.SetAuthCookie("USU" + Convert.ToString(model.Codigo.Value), false);

                    return true;
                }
            }

            return false;
        }
    }
}
