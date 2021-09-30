using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Web;
using System.Web.Security;

namespace CMSv4.BusinessLayer
{
    public class BLClienteAutenticacao
    {
        #region Autenticar Usuário

        /// <summary>
        ///     Autenticar o Usuário
        /// </summary>
        public static bool? Autenticar(string login, string senha)
        {
            MLCliente objUsuario = null;

            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha)) return false;

                objUsuario = BLCliente.ObterCompletoLogin(login, senha);

                if (objUsuario == null || string.IsNullOrEmpty(objUsuario.Login)) return false;

                FormsAuthentication.SignOut();
                FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("CLI" + Convert.ToString(objUsuario.Codigo), false, 60));
                FormsAuthentication.SetAuthCookie("CLI" + Convert.ToString(objUsuario.Codigo), false);

                HttpContext.Current.Items["ctxUsuarioPublico"] = objUsuario;

                return true;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return false;
        }

        #endregion

        #region Autenticar Usuário por Facebook (Email)

        /// <summary>
        ///     Autenticar o Usuário via email,
        /// </summary>
        public static bool? AutenticarUsuarioFacebook(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email)) return false;

                var usuario = BLCliente.ObterCompleto(email);

                if (usuario == null) return false;
                if (!usuario.Codigo.HasValue) return false;

                FormsAuthentication.SignOut();
                FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("CLI" + Convert.ToString(usuario.Codigo), false, 60));
                FormsAuthentication.SetAuthCookie("CLI" + Convert.ToString(usuario.Codigo), false);

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return false;
        }

        #endregion

        #region Autenticar por Token

        public static decimal? AutenticarPorToken(string token, out AutenticacaoTokenStatus retorno)
        {
            retorno = AutenticacaoTokenStatus.Ok;

            var cliente = CRUD.Obter(new MLCliente { TokenNovaSenha = token });

            if (cliente == null || cliente.Codigo.HasValue)
                retorno = AutenticacaoTokenStatus.TokenInvalido;
            else if (!cliente.Ativo.GetValueOrDefault())
                retorno = AutenticacaoTokenStatus.UsuarioInativo;
            else if (!cliente.AlterarSenha.GetValueOrDefault())
                retorno = AutenticacaoTokenStatus.AcessoNegado;

            if (retorno != AutenticacaoTokenStatus.Ok)
                return null;

            return cliente.Codigo;
        }

        #endregion

        #region Armazenar Logado

        public static void ArmazenarLogado(bool UsarCache = false)
        {
            ArmazenarLogado(null, UsarCache);
        }

        /// <summary>
        ///     Armazena o usuário logado no contexto da aplicacao
        /// </summary>
        public static void ArmazenarLogado(HttpContext context, bool UsarCache = false)
        {
            if (context == null)
                context = HttpContext.Current;

            // Se não estiver autenticado, não faz nada
            if (!context.Request.IsAuthenticated) return;

            // Armazena usuário no contexto e na sessão
            if (CodigoLogado.HasValue)
            {
                MLClienteCompleto objUsuario = null;
                var nomeSession = "ctxUsuarioPublico" + CodigoLogado.GetValueOrDefault().ToString();

                if (context.Session != null
                    && context.Session[nomeSession] != null) objUsuario = (MLClienteCompleto)context.Session[nomeSession];

                if (context.Items["ctxUsuarioPublico"] == null && objUsuario != null) context.Items["ctxUsuarioPublico"] = objUsuario;

                if (objUsuario == null || UsarCache)
                {
                    objUsuario = BLCliente.ObterCompleto(CodigoLogado.GetValueOrDefault());

                    context.Items["ctxUsuarioPublico"] = objUsuario;
                    if (context.Session != null) context.Session[nomeSession] = objUsuario;
                }
            }
        }



        #endregion

        #region Efetuar Logoff

        public static void Logoff()
        {
            FormsAuthentication.SignOut();
            if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session.Clear();
        }

        #endregion

        public static decimal? CodigoLogado
        {
            get
            {
                int codigo = 0;

                if (HttpContext.Current.User.Identity.IsAuthenticated
                    && HttpContext.Current.User.Identity.Name.StartsWith("CLI")
                    && int.TryParse(HttpContext.Current.User.Identity.Name.Replace("CLI", ""), out codigo))
                {
                    return codigo;
                }

                return null;
            }
        }
    }
}
