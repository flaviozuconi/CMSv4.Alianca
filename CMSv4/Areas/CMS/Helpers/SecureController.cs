using CMSApp.Helpers;
using System;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace Framework.Utilities
{
    /// <summary>
    /// Classe para veriricar permissões de autenticação e autorização das requisições
    /// </summary>
    public class SecureController : BaseController, IActionFilter
    {
        /// <summary>
        /// Endereço da página de LOGIN administrativa
        /// </summary>
        public const string URL_LOGIN_ADM = "/cms";

        /// <summary>
        /// Endereço da página de ACESSO NEGADO
        /// </summary>
        public const string URL_ACESSO_NEGADO = "/acessonegado";

        /// <summary>
        /// Endereço da página de ACESSO NEGADO
        /// </summary>
        public const string URL_ACESSO_NEGADO_EDICAO = "/sempermissao";

        /// <summary>
        /// Os métodos só poderão ser chamados
        /// por usuários autenticados. Se a ação solicitar uma permissão específica ela é
        /// validada contra o usuário pela URL da requisição
        /// </summary>
        /// <param name="filterContext"></param>
        [Compress]
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            var urlRequisitada = Request.Url.ToString();
            var urlRequisitadaAbsolutePath = Request.Url.AbsolutePath;
            var permissoes = (CheckPermissionAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(CheckPermissionAttribute), true);

            //Redirecionar da área administrativa
            if ((urlRequisitadaAbsolutePath.StartsWith("/cms", StringComparison.InvariantCultureIgnoreCase) || urlRequisitadaAbsolutePath.StartsWith("/moduloprodutoadmin", StringComparison.InvariantCultureIgnoreCase)) && /*possuir /cms ou /moduloprodutoadmin no endereço*/
                urlRequisitada.IndexOf("http://localhost", StringComparison.InvariantCultureIgnoreCase) == -1 && /*não é localhost*/
                urlRequisitada.IndexOf("homolog.co", StringComparison.InvariantCultureIgnoreCase) == -1) /*não é homolog.co*/
            {
                var redirecionar = false;
                var novaUrl = urlRequisitada;

                //Adicionar www. caso não possua no endereço
                if (Request.Url.Host.IndexOf("www") == -1)
                {
                    novaUrl = novaUrl.Replace(Request.Url.Host, string.Concat("www.", Request.Url.Host));
                }

                //Validar HTTPS
                if (BLConfiguracao.Obter<bool>("CMS.Https.Habilitar", false) && /*Primeiramente o HTTPS deve estar habilitado*/
                    Request.Url.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase)) /*não possuir https*/
                {
                    redirecionar = true;
                    novaUrl = novaUrl.Replace(Request.Url.Scheme, "https");
                }

                if (redirecionar)
                {
                    Response.Redirect(novaUrl);
                    return;
                }
            }

            //qualquer um pode acessar
            if (permissoes == null || permissoes.Length == 0)
                return;
            
            //IMPORTANTE:
            //Se passou deste ponto, a partir de agora é preciso que o usuário esteja autenticado para acessar a view

            //definir cultura
            //BLIdioma.Atual = IdiomaAdmin;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(BLIdioma.Atual.Sigla);

            var usuario = BLUsuario.ObterLogado();

            //usuário perdeu a sessão
            if (usuario == null)
            {
                filterContext.Result = new RedirectResult(string.Concat(URL_LOGIN_ADM, "?urlRetorno=", BLEncriptacao.EncriptarQueryString(Request.Url.ToString())));

                return;
            }

            //qualquer um pode acessar
            if (permissoes[0].Permissao == Permissao.Publico)
            {
                return;
            }

            if ((usuario.Publico.GetValueOrDefault()) || !usuario.CheckPermissao(permissoes[0].Permissao, string.IsNullOrEmpty(permissoes[0].Url) ? Request.Url.LocalPath : permissoes[0].Url))
            {
                filterContext.Result = new RedirectResult(URL_ACESSO_NEGADO);

                return;
            }

            //Auditoria por registro
            var auditoria = (AuditingAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuditingAttribute), true);
            
            if (auditoria != null && auditoria.Length > 0)
            {
                var url = string.IsNullOrEmpty(permissoes[0].Url) ? Request.Url.LocalPath : permissoes[0].Url;

                url = string.IsNullOrEmpty(auditoria[0].Url) ? Request.Url.LocalPath : auditoria[0].Url;

                if (!url.StartsWith(BLPortal.PORTAL_PREFIX))
                {
                    url = PortalAtual.Url + url;
                }

                ViewBag.Funcionalidade = usuario.GetCodigoFuncionalidade(url, true);

                var referencia = string.Empty;
                
                foreach (var p in auditoria[0].CodigoRef)
                {
                    var valueProviderResult = filterContext.Controller.ValueProvider.GetValue(p);

                    if (valueProviderResult != null)
                    {
                        referencia += valueProviderResult.AttemptedValue.ToString() + "|";
                    }
                }
                
                referencia = string.IsNullOrEmpty(referencia) ? "id" : referencia.TrimEnd('|');

                ViewBag.Log = usuario.GetAuditoria(referencia, url, null);
            }
            else
            {
                ViewBag.Funcionalidade = usuario.GetCodigoFuncionalidade(Request.Url.LocalPath);
            }
        }

        /// <summary>
        /// Tratamento de erro global para a área administrativa
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            filterContext.ExceptionHandled = true;

            ApplicationLog.ErrorLog(filterContext.Exception);

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Erro500.cshtml"
            };
        }

        public string GetModelStateErrors()
        {
            var textBuilder = new StringBuilder();

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    textBuilder.AppendLine(error.ErrorMessage);
                }
            }

            return textBuilder.ToString();
        }

        public ActionResult NotFound(string url = "")
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                Response.Redirect(string.Concat("~/", CMSApp.Controllers.PublicoController._defaultPaginaNaoEncontrada, "?url=", Url.Encode(url)));
            }
            else
            {
                Response.Redirect(string.Concat("~/", CMSApp.Controllers.PublicoController._defaultPaginaNaoEncontrada));
            }

            return null;
        }

        //public MLIdioma IdiomaAdmin
        //{
        //    get
        //    {
        //        var obj = Session["idioma-atual-admin"];
        //        //var obj = TempData["idioma-atual-admin"];

        //        if (obj == null)
        //        {
        //            obj = new MLIdioma
        //            {
        //                Codigo = 1,
        //                Ativo = true,
        //                Nome = "Português - Brasil",
        //                Sigla = "pt-BR"
        //            };
        //        }

        //        return obj as MLIdioma;
        //    }
        //    set
        //    {
        //        Session["idioma-atual-admin"] = value;
        //        //TempData["idioma-atual-admin"] = value;
        //    }
        //}
    }

    /// <summary>
    /// Esse atribuuto permite definir qual a permissão necessária para o usuário 
    /// executar esse método de uma controladora
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckPermissionAttribute : Attribute
    {
        public Permissao Permissao { get; set; }
        public string Url { get; set; }
        public bool Persist { get; set; }
        public bool ValidarModelState { get; set; }
        public string ViewDeRetornoParaModelStateInvalido { get; set; }

        public CheckPermissionAttribute(Permissao permisssao)
        {
            Permissao = permisssao;
        }

        public CheckPermissionAttribute(Permissao permisssao, string url)
        {
            Permissao = permisssao;
            Url = url;
        }

        public CheckPermissionAttribute(Permissao permissao, string url, bool persist)
        {
            Permissao = permissao;
            Url = url;
            Persist = persist;
        }

        public CheckPermissionAttribute(Permissao permissao, string url, bool persist, bool validarModelState)
        {
            Permissao = permissao;
            Url = url;
            Persist = persist;
            ValidarModelState = validarModelState;
        }

        public CheckPermissionAttribute(Permissao permissao, string url, bool persist, bool validarModelState, string viewDeRetornoParaModelStateInvalido)
        {
            Permissao = permissao;
            Url = url;
            Persist = persist;
            ValidarModelState = validarModelState;
            ViewDeRetornoParaModelStateInvalido = ViewDeRetornoParaModelStateInvalido;
        }
    }

    /// <summary>
    /// Esse atribuuto permite definir qual a permissão necessária para o usuário 
    /// executar esse método de uma controladora
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class AuditingAttribute : Attribute
    {
        public string Url { get; set; }
        public string[] CodigoRef { get; set; }

        public AuditingAttribute()
        {
            CodigoRef = new string[] { "id" };
        }

        public AuditingAttribute(string url)
        {
            Url = url;
            CodigoRef = new string[] { "id" };
        }

        public AuditingAttribute(string[] codigos)
        {
            CodigoRef = codigos;
        }

        public AuditingAttribute(string url, string[] codigos)
        {
            CodigoRef = codigos;
            Url = url;
        }
    }

    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("deflate"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encodingsAccepted.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}