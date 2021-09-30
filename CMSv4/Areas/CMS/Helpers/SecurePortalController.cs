using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;

namespace Framework.Utilities
{
     /// <summary>
    /// Classe para veriricar permissões de autenticação e autorização das requisições
    /// de uma página que precisa ter portal selecionado
    /// </summary>
    public class SecurePortalController : SecureController, IActionFilter
    {
        /// <summary>
        /// * OVERRIDES SECURECONTROLLER CLASS *
        /// Os métodos só poderão ser chamados
        /// por usuários autenticados. Se a ação solicitar uma permissão específica ela é
        /// validada contra o usuário pela URL da requisição
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var permissoes = (CheckPermissionAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(CheckPermissionAttribute), true);
            var usuario = BLUsuario.ObterLogado();
            var portal = PortalAtual.Obter;
            var IsAjax = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();

            if (permissoes != null && permissoes.Length > 0 && permissoes[0].Permissao == Permissao.Publico)
                return;

            //BLIdioma.Atual = IdiomaAdmin;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(BLIdioma.Atual.Sigla);

            if (usuario == null || portal == null)
            {
                if (filterContext.ActionDescriptor.ActionName == "propriedades")
                    filterContext.Result = new RedirectResult(URL_ACESSO_NEGADO_EDICAO); //sem permissao para edicao
                else if (filterContext.ActionDescriptor.ActionName == "novomodulo")
                    filterContext.Result = View(); //nao exibir conteudo do modulo

                else if(IsAjax)
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new
                        {
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            ExpirouSessao = true,
                            Mensagem = TAdm("Sua sessão expirou, por favor realize o login novamente."),
                            data = new System.Collections.Generic.List<string>()
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                    filterContext.Result = new RedirectResult(string.Concat(URL_LOGIN_ADM, "?urlRetorno=", BLEncriptacao.EncriptarQueryString(Request.Url.ToString())));    
                return;
            }

            if (permissoes == null || permissoes.Length == 0)
                return;

            //Validar model state
            if (!ModelState.IsValid && permissoes[0].ValidarModelState)
                filterContext.Result = View(permissoes[0].ViewDeRetornoParaModelStateInvalido ?? "Index");            

            var url = string.IsNullOrEmpty(permissoes[0].Url) ? Request.Url.LocalPath : string.Format(permissoes[0].Url, portal.Diretorio);
            if (!url.StartsWith(BLPortal.PORTAL_PREFIX))
            {
                url = PortalAtual.Url + url;
            }

            if (!usuario.CheckPermissao(permissoes[0].Permissao, url, true))
            {
                filterContext.Result = new RedirectResult(URL_ACESSO_NEGADO);

                return;
            }

            //Auditoria por registro
            var auditoria = (AuditingAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuditingAttribute), true);
            if (auditoria != null && auditoria.Length > 0)
            {
                url = string.IsNullOrEmpty(auditoria[0].Url) ? Request.Url.LocalPath : permissoes[0].Url;
                if (!url.StartsWith(BLPortal.PORTAL_PREFIX))
                {
                    url = PortalAtual.Url + url;
                }
               
                ViewBag.Funcionalidade = usuario.GetCodigoFuncionalidade(url);
                
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
                ViewBag.Log = usuario.GetAuditoria(referencia, url, portal.Codigo);

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
            //Se já foi feito um tratamento especifico de erro em outro Atributo, encerrar o tratamento padrão.
            if (filterContext.ExceptionHandled)
                return;

            filterContext.ExceptionHandled = true;
            
            //Salvar log
            ApplicationLog.ErrorLog(filterContext.Exception);

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Areas/ModuloAdmin/Views/Shared/_Error.cshtml"
            };
        }
    }
}