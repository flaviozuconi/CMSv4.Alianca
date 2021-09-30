using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using System.Web.Security;
using System.Transactions;
using System.Configuration;
using Facebook;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ModuloLoginController : ModuloBaseController<MLModuloLoginEdicao, MLModuloLoginHistorico, MLModuloLoginPublicado>
    {
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

            // Visualizar Publicado
            MLModuloLogin model = CRUD.Obter<MLModuloLoginPublicado>(new MLModuloLoginPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

            if (model == null) model = new MLModuloLogin() { CodigoPagina = codigoPagina, Repositorio = repositorio };

            return View(model);
        }

        #endregion

        #region Login

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Login(MLModuloLogin model)
        {
            try
            {
                if (Request.QueryString["key"] != null)
                    model.UrlRetorno = BLEncriptacao.DesencriptarQueryString(Request.QueryString["key"]);

                if (string.IsNullOrEmpty(model.UrlPaginaLogin))
                    model.UrlPaginaLogin = Request.Url.AbsolutePath;

                if (Request.QueryString["token"] != null)
                {
                    model.Token = Convert.ToString(Request.QueryString["token"]);
                    var statusRetorno = AutenticacaoTokenStatus.Ok;
                    decimal? retorno = BLClienteAutenticacao.AutenticarPorToken(model.Token, out statusRetorno);

                    if (statusRetorno == AutenticacaoTokenStatus.Ok && retorno.HasValue)
                    {
                        return PartialView("EsqueciSenha", model);
                    }
                    else
                    {
                        if (statusRetorno == AutenticacaoTokenStatus.AcessoNegado)
                            ViewBag.Message = "Acesso negado";
                        else if (statusRetorno == AutenticacaoTokenStatus.TokenInvalido)
                            ViewBag.Message = "Token inválido";
                        else if (statusRetorno == AutenticacaoTokenStatus.UsuarioInativo)
                            ViewBag.Message = "Usuário inativo";

                        return PartialView("Index", model);
                    }
                }

                return PartialView("Login", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region AcessoNegado

        public ActionResult AcessoNegado(MLModuloLogin model)
        {
            return View(model);
        }

        #endregion

        #region AlterarSenha

        /// <summary>
        /// Altear Senha do Usuario
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult AlterarSenha()
        {
            var cliente = BLCliente.ObterLogado();
            MLClienteAlterarSenha model = new MLClienteAlterarSenha();

            if (cliente != null)
            {
                model.Codigo = cliente.Codigo;
                model.Nome = cliente.Nome;
            }

            return PartialView(model);
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult AlterarSenha(string novaSenha, string senhaAtual)
        {
            try
            {
                //Verifica se as informações estão válidas na model.
                if (ModelState.IsValid)
                {
                    //Obter usuário logado no sistema
                    //obs: É preciso realizar a busca da informação novamente do usuário para atualizar 
                    var cliente = new CRUD.Select<MLCliente>()
                            .Equals(a => a.Codigo, BLClienteAutenticacao.CodigoLogado)
                        .First();

                    //Verifica se a senha informada no formulário é igual a do usuário logado
                    if (cliente.Senha == BLEncriptacao.EncriptarSenha(senhaAtual))
                    {
                        //Popular model com a nova senh
                        var model = new MLClienteAlterarSenha();
                        model.Senha = BLEncriptacao.EncriptarSenha(novaSenha);
                        model.Codigo = cliente.Codigo;

                        //Atualizar as informações na base de dados
                        CRUD.SalvarParcial<MLClienteAlterarSenha>(model, BLPortal.Atual.ConnectionString);
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Senha Atual Inválida" });
                    }
                }

                return Json(new { success = ModelState.IsValid });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region BarraInfoUsuario

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult BarraInfoUsuario(MLModuloLogin model)
        {
            ViewBag.modulo = model;
            return View(BLCliente.ObterLogado());
        }

        #endregion

        #region Edição

        /// <summary>
        /// Editar
        /// </summary>
        public override ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLModuloLoginEdicao>(new MLModuloLoginEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloLoginEdicao();

                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloLoginEdicao model)
        {
            try
            {
                //Salvar
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                return Json(new { success = true });
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
            return View(BLPortal.Portais);
        }

        #endregion

        #region Esqueci Senha

        /// <summary>
        /// Página de recuperação de senha
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult EsqueciSenha(string nomeUsuario, string urlLogin)
        {
            try
            {
                var usuario = BLCliente.ObterCompleto(nomeUsuario);

                // Se não encontrar o usuário, ou estiver inativo não permitir continuar
                // Por questões de segurança, não se deve confirmar/negar que o LOGIN existe ou está inativo.

                if (usuario == null
                    || !usuario.Ativo.HasValue
                    || usuario.Ativo == false
                    || string.IsNullOrEmpty(usuario.Email)
                )
                    return Json(new { success = false, msg = "Usuário não cadastrado." });

                // enviar email
                var token = Guid.NewGuid().ToString();

                var conteudo = BLEmail.ObterModelo(BLEmail.ModelosPadrao.AlterarSenha);
                var urlPortal = Portal.Url(BLPortal.Atual.Diretorio);

                if (!string.IsNullOrWhiteSpace(urlPortal))
                    urlPortal = "/" + urlPortal;

                conteudo = conteudo.Replace("[[nome]]", usuario.Nome);
                conteudo = conteudo.Replace("[[url]]", string.Concat(Request.Url.Scheme, "://", Request.Url.Authority, urlPortal, urlLogin, "?token=", token));


                //else
                //    conteudo = conteudo.Replace("[[url]]", string.Concat(Request.Url.Scheme, "://", Request.Url.Authority, urlLogin, "?token=", token));

                BLEmail.Enviar("Alterar Senha", usuario.Email, conteudo);

                CRUD.SalvarParcial(new MLClienteAlterarSenha { Codigo = usuario.Codigo, TokenNovaSenha = token, AlterarSenha = true }, BLPortal.Atual.ConnectionString);

                return Json(new { success = true, msg = "Um e-mail foi enviado para você, acesse sua conta de e-mail para trocar sua senha." });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region AtualizarSenha
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult AtualizarSenha(MLClienteAlterarSenha model, string returnUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.TokenNovaSenha))
                {
                    var statusRetorno = AutenticacaoTokenStatus.Ok;

                    model.Codigo = BLClienteAutenticacao.AutenticarPorToken(model.TokenNovaSenha, out statusRetorno);
                    model.Senha = BLEncriptacao.EncriptarSenha(model.Senha);
                    model.AlterarSenha = false;
                    model.TokenNovaSenha = null;

                    CRUD.SalvarParcial<MLClienteAlterarSenha>(model);

                    if (model.Codigo.HasValue)
                    {
                        FormsAuthentication.SignOut();
                        FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("CLI" + Convert.ToString(model.Codigo), false, 60));
                        FormsAuthentication.SetAuthCookie("CLI" + Convert.ToString(model.Codigo), false);

                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, msg = "Erro ao realizar alteração de senha." });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Excluir

        /// <summary>
        /// Excluir
        /// </summary>
        public override ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                CRUD.Excluir<MLModuloLoginEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Exit

        /// <summary>
        /// Logoff do sistema e redireciona para área pública
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Sair()
        {
            BLClienteAutenticacao.Logoff();
            return new RedirectResult("/" + Portal.Url(BLPortal.Atual.Diretorio));
        }

        #endregion

        #region Login (realizar login)

        /// <summary>
        /// Testa a autenticação do usuário e envia para página de destino
        /// O ideal é a página de LOGIN já estar em HTTPS!
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        [NoCache]
        public ActionResult Autenticar(string usuario, string password)
        {
            try
            {
                var retorno = BLClienteAutenticacao.Autenticar(usuario, password);
                var strMsgErro = String.Empty;

                if (retorno == true)
                {
                    var loginCookie = new HttpCookie("admin-nome-login", usuario);
                    loginCookie.Expires = DateTime.Today.AddYears(1);

                    Response.Cookies.Add(loginCookie);
                }
                else
                {
                    strMsgErro = "E-mail e/ou senha incorretos";
                }

                return Json(new { success = retorno, msg = strMsgErro });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloLogin model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ScriptAlterarSenha

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptAlterarSenha(MLClienteAlterarSenha model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ScriptEsqueciSenha

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptEsqueciSenha(MLModuloLogin model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Visualizar

        /// <summary>
        /// Área de Construção
        /// </summary>
        public override ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = new MLModuloLogin();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter(new MLModuloLoginEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                    if (model == null)
                    {
                        model = new MLModuloLoginEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio, View = "Login" };
                        CRUD.Salvar((MLModuloLoginEdicao)model, BLPortal.Atual.ConnectionString);
                    }
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter(new MLModuloLoginHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter(new MLModuloLoginPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloLoginPublicado()
                {
                    CodigoPagina = codigoPagina,
                    Repositorio = repositorio,
                    View = "Login"
                };

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }


        #endregion

        #region Perfil

        /// <summary>
        /// Perfil
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Perfil(MLModuloLogin model)
        {
            var user = BLCliente.ObterLogado();

            if (Session["AccessToken"] != null && user == null)
            {
                ViewBag.CadastroFacebook = true;
                user = new MLClienteCompleto();

                try
                {
                    //Novo cadastro, preencher informações com as info do facebook
                    var fb = new FacebookClient();

                    //Chave de acesso para obter os dados do facebook
                    fb.AccessToken = Session["AccessToken"].ToString();
                    dynamic userFace = fb.Get("me?fields=name,id,email,birthday,gender,location");

                    user.Nome = userFace.name;
                    user.Email = userFace.email;

                    DateTime dataNascimento = DateTime.Now;

                    if (userFace.birthday != null && DateTime.TryParse(userFace.birthday, out dataNascimento))
                        user.DataNascimento = dataNascimento;

                    if (userFace.location != null && userFace.location.name != null && !string.IsNullOrWhiteSpace(userFace.location.name))
                        user.Cidade = userFace.location.name;

                    //if (userFace.gender != null)
                    //{
                    //    var generoTermo = string.Empty;

                    //    if (userFace.gender == "male")
                    //        generoTermo = "masculino";
                    //    else if (userFace)
                    //        generoTermo = "feminino";

                    //    var generos = CRUD.ListarCache(new MLClienteGenero() { Ativo = true });
                    //    var genero = generos.Find(o => o.Nome.ToLower().Contains(generoTermo));

                    //    if (genero != null)
                    //        user.CodigoGenero = genero.Codigo;
                    //}

                }
                catch (Exception)
                {
                }
            }

            ViewBag.UserLogado = user;

            return View(model);
        }

        /// <summary>
        /// Perfil
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarPerfil(MLClienteCompleto model, string currentUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var listaCodigoGrupo = Request.Form["listaCodigoGrupo"] ?? string.Empty;
                    var listaCodigoAreaInteresse = Request.Form["listaCodigoAreaInteresse"] ?? string.Empty;
                    var listaCodigoProgramaInteresse = Request.Form["listaCodigoProgramaInteresse"] ?? string.Empty;
                    var listaCodigoMembros = Request.Form["listaCodigoMembros"] ?? string.Empty;
                    var portal = BLPortal.Atual;
                    var logado = BLCliente.ObterLogado();
                    var novoRegistro = logado == null || !logado.Codigo.HasValue;
                    var connstring = portal.ConnectionString;

                    if (logado != null && model.Codigo.HasValue)
                        model.Codigo = logado.Codigo;

                    using (var scope = new TransactionScope())
                    {
                        model.Ativo = true;

                        if (!string.IsNullOrWhiteSpace(model.Senha))
                            model.Senha = BLEncriptacao.EncriptarSenha(model.Senha);

                        //Se o cliente seleiconou um estado no combo, preencher o text com o estado selecionado para facilitar a e
                        //A consulta nos relatórios
                        if (model.CodigoEstado.HasValue)
                        {
                            var estado = CRUD.Obter<MLEstado>(model.CodigoEstado.Value, portal.ConnectionString);

                            if (estado != null)
                                model.Estado = estado.Nome;
                        }

                        model.Login = model.Email;
                        var codigo = CRUD.Salvar<MLCliente>(model);

                        #region Salvar Permissões

                        //Verificar em quais grupos os clientes devem ser inseridos
                        if (novoRegistro)
                        {
                            model.DataCadastro = DateTime.Now;

                            var gruposCadastroPublico = CRUD.ListarCache(new MLGrupoCliente() { Ativo = true, CodigoPortal = portal.Codigo, DefaultCadastroPublico = true });

                            foreach (var item in gruposCadastroPublico)
                                model.Grupos.Add(new MLClienteItemGrupo() { CodigoCliente = codigo, CodigoGrupo = item.Codigo });
                            CRUD.Salvar(model.Grupos, connstring);
                        }

                        #endregion

                        scope.Complete();

                        var urlRetorno = string.Empty;
                        var tempData = Convert.ToString(TempData["qsKey"]);

                        //Autenticar o cliente no sistema
                        BLClienteAutenticacao.AutenticarUsuarioFacebook(model.Email);

                        //Renovar as informações no cache do cliente
                        BLClienteAutenticacao.ArmazenarLogado(true);

                        if (logado == null)
                            return Json(new { success = true, message = T("Seu cadastro foi realizado com sucesso, aguarde enquanto redirecionamos você..."), classe = "alert alert-success", UrlRetorno = BLEncriptacao.DesencriptarQueryString(Convert.ToString(TempData["qsKey"])) });
                        return Json(new { success = true, message = T("Seus dados foram atualizados com sucesso, aguarde enquanto redirecionamos você..."), classe = "alert alert-success", UrlRetorno = BLEncriptacao.DesencriptarQueryString(Convert.ToString(TempData["qsKey"])) });

                    }
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, message = T("Não foi possível realizar seu cadastro, tente novamente mais tarde."), classe = "alert alert-danger" });
                }
            }

            return Json(new { success = false, message = T("Preencha os campos corretamente."), classe = "alert alert-warning" }); ;
        }

        /// <summary>
        /// RecortarImagem
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Publico)]
        public ActionResult RecortarImagem(decimal codigo, string img)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(img.Replace("data:image/png;base64,", ""));
                var webImg = new System.Web.Helpers.WebImage(bytes);

                webImg.ReplicarArquivoClientes(codigo);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = T("Não foi possível recortar a imagem") });
            }
        }

        /// <summary>
        /// Verifica se o email não ficará duplicado ao salvar
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult IsValidEmail(decimal? id, string email)
        {
            var model = CRUD.Obter<MLCliente>(new MLCliente { Email = email, Ativo = true });

            if (model != null && model.Codigo.HasValue && ((id.HasValue && id.Value != model.Codigo) || !id.HasValue))
                return Json("Já existe um usuário cadastrado com esse e-mail");

            return Json(true);
        }

        #region ListarEstados

        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarEstados(decimal CodigoPais)
        {
            var estados = CRUD.Listar(new MLEstado() { CodigoPais = CodigoPais }, PortalAtual.ConnectionString);
            return Json(estados.OrderBy(o => o.Uf), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Facebook

        /// <summary>
        /// Code = Código de acesso gerado pelo facebook.
        /// Key = query string com a url de retorno enviado pelo CMS
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult FacebookCallback(string code)
        {
            string urlRedirect = ConfigurationManager.AppSettings["CMS.ModuloLogin.UrlCadastro"];

            if (!string.IsNullOrWhiteSpace(urlRedirect))
                urlRedirect = "/" + BLPortal.Atual.Diretorio + urlRedirect;

            if (!String.IsNullOrWhiteSpace(code))
            {
                var fb = new FacebookClient();
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.App.Login.Id"],
                    client_secret = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.App.Login.Secret"],
                    redirect_uri = RedirectUri.AbsoluteUri,
                    code = code
                });

                var accessToken = result.access_token;

                Session["AccessToken"] = accessToken;

                // Atualiza o cliente do facebook com o token de acesso
                // para que possamos efetuar requests
                fb.AccessToken = accessToken;

                //Obter informações do usuário
                dynamic userFace = fb.Get("me?fields=name,id,email,birthday,gender,location");
                string email = userFace.email;

                // Define o cookie de autenticação.
                bool? IsAutenticado = BLClienteAutenticacao.AutenticarUsuarioFacebook(email);

                //Usuário esta realizando login, já esta cadastrado no CMS
                if (IsAutenticado.HasValue && IsAutenticado.Value)
                {
                    //Url de retorno, página em que o cliente estava antes de cair na página de login
                    var url = Convert.ToString(TempData["qsKey"]);

                    if (!string.IsNullOrWhiteSpace(url))
                        urlRedirect = BLEncriptacao.DesencriptarQueryString(url);
                }
            }

            return Redirect(urlRedirect);
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult FacebookAutenticacao(string key)
        {
            //Se o usuário já esta autenticado, enviar para a tela de cadastro para visualização dos dados
            var user = BLCliente.ObterLogado();

            if (user != null && user.Codigo.HasValue)
                return Redirect("/" + BLPortal.Atual.Diretorio + ConfigurationManager.AppSettings["CMS.ModuloLogin.UrlCadastro"]);

            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.App.Login.Id"],
                client_secret = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.App.Login.Secret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "public_profile,email,user_birthday,user_location"
            });

            if (Request.UrlReferrer != null && !String.IsNullOrWhiteSpace(Request.UrlReferrer.ToString()))
                TempData["UrlRedirect"] = Request.UrlReferrer.ToString();
            else
                TempData["UrlRedirect"] = Request.Url.Scheme + "://" + Request.Url.Authority;

            TempData["qsKey"] = key;

            return Redirect(loginUrl.AbsoluteUri);
        }

        #endregion
    }
}
