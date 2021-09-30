using CMSv4.BusinessLayer;
using Framework.Utilities;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMSApp.Areas.CMS.Controllers
{

    /// <summary>
    /// Login
    ///     Controladora para ações de autenticação no sistema
    /// </summary>
    public class LoginController : SecureController
    {
        #region Index

        /// <summary>
        /// Página padrão da área administrativa
        /// </summary>
        /// <returns></returns>
        [NoCache]
        public ActionResult Index(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var retorno = new BLLoginAdmin().LogarPorToken(token);

                if(retorno.Sucesso)
                    return View("AlterarSenha", new MLUsuarioAlterarSenha { TokenNovaSenha = token });

                ViewBag.Message = retorno.Mensagem;

                return View();
            }

            var usuarioLogado = BLUsuario.ObterLogado();

            if (usuarioLogado != null && usuarioLogado.Codigo.HasValue)
            {
                if (BLPortal.Portais.Count == 1)
                    return new RedirectResult("/cms/" + BLPortal.Portais[0].Diretorio.ToLower());
                else
                    return new RedirectResult("/cms/escolherportal");
            }

            return View();
        }

        #endregion

        #region Index (realizar login)

        /// <summary>
        /// Testa a autenticação do usuário e envia para página de destino
        /// O ideal é a página de LOGIN já estar em HTTPS!
        /// </summary>
        [HttpPost]
        [NoCache]
        public ActionResult Index(string email, string password, string returnUrl)
        {
            try
            {
                var retorno = BLUsuario.AutenticarUsuario(email, password);
                
                if (retorno == true)
                {
                    var loginCookie = new HttpCookie("admin-nome-login", email);
                    loginCookie.Expires = DateTime.Today.AddYears(1);

                    Response.Cookies.Add(loginCookie);

                    if (!string.IsNullOrEmpty(returnUrl))
                        return new RedirectResult(BLEncriptacao.DesencriptarQueryString(returnUrl));
                    else if (BLPortal.Portais.Count == 1)
                        return new RedirectResult("/cms/" + BLPortal.Portais[0].Diretorio.ToLower());
                    else
                        return new RedirectResult("/cms/escolherportal");
                }
                else
                {
                    ModelState.AddModelError("Error", T("Invalid Email Address or Username"));
                }

                return View();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Escolher Portal

        /// <summary>
        /// Página de escolha de portal
        /// </summary>
        [NoCache]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult EscolherPortal()
        {
            var lista = BLPortal.Portais;

            lista = lista.FindAll(a => a.Ativo.GetValueOrDefault());

            return View(lista);
        }

        #endregion

        #region Esqueci Senha

        /// <summary>
        /// Página de recuperação de senha
        /// </summary>
        /// <returns></returns>
        public ActionResult EsqueciSenha()
        {
            return View();
        }

        /// <summary>
        /// Página de recuperação de senha
        /// </summary>
        [HttpPost]
        public ActionResult EsqueciSenha(string usuarioEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(usuarioEmail))
                {
                    ViewBag.Message = T("Enter the Email Address or Username.");

                    return View();
                }
                
                var usuario = BLUsuario.ObterCompleto(usuarioEmail);

                // Se não encontrar o usuário, ou estiver inativo não permitir continuar
                // Por questões de segurança, não se deve confirmar/negar que o LOGIN existe ou está inativo.

                if (usuario == null
                    || !usuario.Ativo.GetValueOrDefault()
                    || string.IsNullOrEmpty(usuario.Email))
                {
                    ViewBag.Message = T("Invalid E-mail Address or Username, contact the system administrator.");
                }
                else
                {
                    // enviar email
                    var token = Guid.NewGuid().ToString();

                    CRUD.SalvarParcial<MLUsuarioAlterarSenha>(new MLUsuarioAlterarSenha { Codigo = usuario.Codigo, TokenNovaSenha = token, AlterarSenha = true });

                    // enviar email
                    BLEmail.Enviar(T("Alterar senha | Change Password | Cambiar contraseña"), usuario.Email,
                        BLEmail.ObterModelo(BLEmail.ModelosPadrao.AlterarSenhaEnUs)
                                    .Replace("[[url]]", string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, "/cms/?token=", token))
                                    .Replace("[[nome]]", usuario.Nome)
                    );
                    
                    ViewBag.Message = T("An email was sent to you, access your email account to change your password.");
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return View();
        }

        #endregion

        #region AlterarSenha

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost]
        public ActionResult AlterarSenha(MLUsuarioAlterarSenha model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var autenticado = new BLLoginAdmin().AlterarSenha(model);

                    if(autenticado)
                        return new RedirectResult("/cms/escolherportal");
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return View("AlterarSenha", model);
        }

        #endregion

        #region AcessoNegado
        public ActionResult AcessoNegado()
        {
            return View();
        }
        #endregion

        #region SemPermissao
        public ActionResult SemPermissao()
        {
            return View();
        }
        #endregion

        #region Exit

        /// <summary>
        /// Logoff do sistema e redireciona para área pública
        /// </summary>
        public ActionResult Sair()
        {
            BLUsuario.Logoff();
            return new RedirectResult("/");
        }

        #endregion
    }
}
