using Framework.Utilities;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSApp
{
    public class RouteConfig
    {
        private static string _urlSSL = BLConfiguracao.UrlSSL;

        public static void RegisterRoutes(RouteCollection routes)
        {
            //Inclua aqui os arquivos que não devem ser usados por rotas
            #region Rotas Customizadas
            #region Gerador de Miniaturas Imagens
            routes.MapRoute(
            "Thumb",
            "thumb/{diretorioPortal}/{modulo}/{codigoRegistro}/{width}/{heigth}/{imagem}",
            new { controller = "Thumb", action = "Index", diretorioPortal = UrlParameter.Optional, codigoRegistro = UrlParameter.Optional, outputType = UrlParameter.Optional }
            );

            routes.MapRoute(
            "Thumb_2",
            "thumb-{diretorioPortal}-{modulo}-{codigoRegistro}-{width}-{heigth}-{imagem}",
            new { controller = "Thumb", action = "Index", diretorioPortal = UrlParameter.Optional, codigoRegistro = UrlParameter.Optional }
            );

            routes.MapRoute(
            "Thumb_1",
            "thumb/{diretorioPortal}/{modulo}/{width}/{heigth}/{imagem}/",
            new { controller = "Thumb", action = "Index", diretorioPortal = UrlParameter.Optional }
            );
            #endregion

            #region Download
            //Modulo de Arquivos
            routes.MapRoute(
            "Download",
            "download/{diretorioPortal}/{codigoarquivo}/",
            new { area = "Modulo", controller = "Arquivos", action = "DownloadFile", diretorioPortal = UrlParameter.Optional }
            );


            routes.MapRoute(
           "arquivos",
           "arquivos/{nomepasta}",
           new { area = "Modulo", controller = "Arquivos", action = "DownloadFileNome", nomepasta = UrlParameter.Optional }
           );
            #endregion

            #region Roxy File Manager

            //RoxyFileman
            routes.MapRoute(
                "RoxyFileman",
                "Content/js/plugins/ckeditor/plugins/fileman/Index",
                new { area = "CMS", controller = "RoxyFileman", action = "Index", diretorioPortal = UrlParameter.Optional }
            );


            #endregion

            //TODO PRÓXIMO CMS DEVE TRATAR AS URLS ABAIXO FIXADAS !!!!!
            #region Área Administrativa CMS

            AddRoute(ref routes,
            "CMS_acesso_negado",
            "acessonegado",
            new { AreaName = "CMS", controller = "Login", action = "AcessoNegado" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_sem_permissao",
            "sempermissao",
            new { AreaName = "CMS", controller = "Login", action = "SemPermissao" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_login",
            "cms",
            new { AreaName = "CMS", controller = "Login", action = "Index" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_login_novasenha",
            "cms/esquecisenha",
            new { AreaName = "CMS", controller = "Login", action = "EsqueciSenha" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_login_escolherportal",
            "cms/escolherportal",
            new { AreaName = "CMS", controller = "Login", action = "EscolherPortal" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_login_sair",
            "cms/sair",
            new { AreaName = "CMS", controller = "Login", action = "Sair" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_treinamento",
            "cms/treinamento",
            new { AreaName = "CMS", controller = "Ajuda", action = "Index" },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_funcionalidade",
            "admin/funcionalidade/{action}/{id}",
            new { AreaName = "CMS", controller = "Funcionalidade", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_grupo",
            "admin/grupo/{action}/{id}",
            new { AreaName = "CMS", controller = "Grupo", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_usuario",
            "admin/usuario/{action}/{id}",
            new { AreaName = "CMS", controller = "Usuario", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_clienteadm",
            "admin/clienteadm/{action}/{id}",
            new { AreaName = "CMS", controller = "ClienteAdm", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_auditoria",
            "admin/auditoria/{action}/{id}",
            new { AreaName = "CMS", controller = "Auditoria", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_portal",
            "admin/portal/{action}/{id}",
            new { AreaName = "CMS", controller = "Portal", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            AddRoute(ref routes,
            "CMS_logerro",
            "admin/logerro/{action}/{id}",
            new { AreaName = "CMS", controller = "LogErro", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            #endregion

            #endregion

            //Importante: Nao tirar dessa ordem porque vai interferir nas Rotas Customizadas
            #region Ignorar Rotas
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("iconpack.axd/{*pathInfo}");
            routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|js|gif|jpg|png|htm|html|flv|swf|avi|mp4|mpeg)(/.*)?" });
            //routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|js|gif|jpg|png|flv|swf|avi|mp4|mpeg)(/.*)?" });
            //routes.MapRoute("HTML",
            routes.RouteExistingFiles = false;
            #endregion

            #region Área Administrativa
            AddRoute(ref routes,
            "CMS_default",
            "cms/{portal}/{controller}/{action}/{id}",
            new { AreaName = "CMS", controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers", "CMSApp.Areas.ModuloAdmin.Controllers", "CMSApp.Areas.Modulo.Controllers", "CMSApp.Areas.ModuloProdutoAdmin.Controllers" }
            );
            #endregion

            #region Sub Portais

            foreach (var portal in CRUD.Listar<MLPortal>(new MLPortal { Ativo = true }))
            {
                routes.MapRoute(
                "Default_" + portal.Codigo,
                portal.Diretorio + "/{url}/{extra1}/{extra2}/{*end}",
                new { controller = "Publico", action = "Index", codigoPortal = portal.Codigo, url = UrlParameter.Optional, extra1 = UrlParameter.Optional, extra2 = UrlParameter.Optional }
                );
            }

            routes.MapRoute(
            "Default",
            "{url}/{extra1}/{extra2}/{*end}",
            new { controller = "Publico", action = "Index", url = UrlParameter.Optional, extra1 = UrlParameter.Optional, extra2 = UrlParameter.Optional }
            );

            #endregion

            #region Defalut

            AddRoute(ref routes,
            "CMSApp",
            "{controller}/{action}/{id}",
            new { AreaName = "CMS", action = "Index", portal = "Principal", id = UrlParameter.Optional },
            new[] { "CMSApp.Areas.CMS.Controllers" }
            );

            #endregion
        }

        private static void AddRoute(ref RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            routes.MapRoute(name, url, defaults, null/*constraints*/, namespaces);

            //Rota para url segura
            if (!String.IsNullOrEmpty(_urlSSL))
                routes.MapRoute(_urlSSL + name, _urlSSL + url, defaults, null/*constraints*/, namespaces);
        }
    }
}