using System.Web.Mvc;
using System.Web.Routing;
using CMSApp.App_Start;
using System.Web.Optimization;
using CMSv4.Rotinas;
using System.Web;
using Framework.Utilities;
using System.Collections.Generic;
using CMSv4.Model;
using System;

namespace CMSApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Add(new MyAreasViewEngine());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Rotinas.GetInstance.DisparaRotinas();

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        }

        protected void Application_BeginRequest()
        {           
            Redirecionar();
            //string[] headers = { "Server", "X-AspNetMvc-Version" };
            //if (!Response.HeadersWritten)
            //{
            //    Response.AddOnSendingHeaders((c) =>
            //    {
            //        if (c != null && c.Response != null && c.Response.Headers != null)
            //        {
            //            foreach (string header in headers)
            //            {
            //                if (c.Response.Headers[header] != null)
            //                {
            //                    c.Response.Headers.Remove(header);
            //                }
            //            }
            //        }
            //    });
            //}
        }

        private void Redirecionar()
        {
            var absoluthPath = HttpContext.Current.Request.Url.PathAndQuery;//HttpContext.Current.Request.Url.AbsolutePath;
            var absoluthUri = HttpContext.Current.Request.Url.AbsoluteUri;
            var authoritity = HttpContext.Current.Request.Url.Authority;
            //ApplicationLog.ErrorLog(new Exception(absoluthPath + "-" + absoluthUri + "-" + authoritity));
            try
            {
                var lstRedirect = BLCachePortal.Get<List<MLRedirect>>("redirect_url");

                if (lstRedirect == null)
                {
                    lstRedirect = CRUD.Listar(new MLRedirect());
                    BLCachePortal.Add("redirect_url", lstRedirect);
                }
                if (lstRedirect.Count == 0) return;
                var pagina = lstRedirect.Find(p => p.Tipo == "end" && absoluthPath.EndsWith(p.UrlDe, StringComparison.InvariantCultureIgnoreCase)/* absoluthPath.IndexOf(p.de, StringComparison.InvariantCultureIgnoreCase) > -1*/);
                if (pagina != null && !String.IsNullOrWhiteSpace(pagina.UrlPara))
                {
                    Redirect_301(lstRedirect, pagina.UrlPara, absoluthPath);
                }
                else
                {
                    pagina = lstRedirect.Find(p => p.Tipo == "replacedominio" && absoluthPath.Contains(p.UrlDe.ToLower()));
                    if (pagina != null && !String.IsNullOrWhiteSpace(pagina.UrlPara))
                    {

                        Redirect_301(lstRedirect, absoluthUri.Replace(authoritity, pagina.UrlPara), absoluthPath);
                    }
                    else
                    {
                        pagina = lstRedirect.Find(p => p.Tipo == "contains" && absoluthPath.ToLower().Contains(p.UrlDe.ToLower()));
                        if (pagina != null && !String.IsNullOrWhiteSpace(pagina.UrlPara))
                        {
                            Redirect_301(lstRedirect, pagina.UrlPara, absoluthPath);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }
        private void Redirect_301(List<MLRedirect> lstRedirect, string url, string absoluthPath)
        {

            if (lstRedirect.Find(o => o.Tipo == "ignore" && absoluthPath == o.UrlDe) == null)
            {
                HttpContext.Current.Response.StatusCode = 301;
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", url);
            }
        }
    }
}