using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web;
using System.Linq;

namespace Framework.Utilities
{
    /// <summary>
    /// BLPortal.Atual USAR NA ÁREA PUBLICA
    /// 
    /// Area: Modulo
    /// </summary>
    public class BLPortal
    {
        public const string PORTAL_PREFIX = "/cms";

        /// <summary>
        /// Obtem a lista de portais
        /// </summary>
        public static List<MLPortal> Portais
        {
            get
            {
                HttpContextFactory.Current.Application["Portais"] = CRUD.ListarCache<MLPortal>(new MLPortal(), 4);

                return (List<MLPortal>)HttpContextFactory.Current.Application["Portais"];
            }
        }

        /// <summary>
        /// Obtem o portal atual
        /// </summary>
        public static MLPortal Atual
        {
            get
            {
                MLPortal portal = null;

                //obter url da Request
                string urlRequest = HttpContextFactory.Current.Request.Url.Authority.ToLower().Replace("www.", ""); ; //.Authority.ToLower().Replace("www.", "");

                //obter AbsolutePath e remover prefixo do portal
                string urlAbsolute = HttpContextFactory.Current.Request.Url.AbsolutePath.Replace(PORTAL_PREFIX, "");

                //listar portais ativos
                List<MLPortal> PortaisAtivos = Portais.FindAll(a => a.Ativo.GetValueOrDefault());
                
                //verificar url da Request e validar se há na url um diretório de portal
                if (!urlAbsolute.Equals("/"))
                    portal = PortaisAtivos.Find(a => urlAbsolute.StartsWith(string.Format("/{0}/", a.Diretorio), StringComparison.InvariantCultureIgnoreCase) ||
                                                     urlAbsolute.Equals(string.Format("/{0}", a.Diretorio), StringComparison.InvariantCultureIgnoreCase));

                //buscar um portal cuja url da Request esteja associado ao portal através da propriedade Url (separável por vírgula)
                //04/04/2017 rvissontai - Para obter somente o dominio sem http(s) e www, no lugar do replace utilizado foi modificado para Authorite para evitar problemas de codificação na query string com acento
                if (portal == null)
                    portal = PortaisAtivos.Find(a => !string.IsNullOrEmpty(a.Url) && a.Url.Split(',').ToList().Find(b => urlRequest.Equals(b.Replace("www.", ""), StringComparison.InvariantCultureIgnoreCase)) != null);

                //buscar um portal de acordo com a URL obtida da requisição do usuário
                if (portal == null && !string.IsNullOrEmpty(Url))
                    portal = PortaisAtivos.Find(a => a.Diretorio.Equals(Url, StringComparison.InvariantCultureIgnoreCase));

                //SE a connectionstring estiver vazia, preencher com o valor padrão
                if (portal != null && string.IsNullOrEmpty(portal.ConnectionString))
                    portal.ConnectionString = ApplicationSettings.ConnectionStrings.Default;

                //SE nenhum portal foi encontrado até então, define o portal atual como sendo o primeiro da lista de portais
                if (portal == null && PortaisAtivos.Count > 0)
                    portal = PortaisAtivos[0];

                //SE mesmo assim não encontrar, retorna erro ao usuário
                if (portal == null)
                    throw new ApplicationException("CMS: Portal não encontrado para o domínio requisitado. Contate administrador para configuração da tabela de PORTAIS");

                return portal;
            }
        }

        #region Atualizar com Usuarios

        /// <summary>
        /// Atualizar
        /// </summary>
        public static decimal? Salvar(MLPortalComUsuarios model)
        {
            using (var scope = new TransactionScope())
            {
                var codigo = CRUD.Salvar<MLPortal>(model);

                CRUD.Excluir<MLUsuarioItemPortal>("CodigoPortal", codigo);

                foreach (var item in model.Usuarios)
                {
                    if (item.UsuarioAssociado.HasValue && item.UsuarioAssociado.Value)
                    {
                        var novoItem = new MLUsuarioItemPortal
                        {
                            CodigoUsuario = item.CodigoUsuario,
                            CodigoPortal = codigo
                        };

                        CRUD.Salvar(novoItem);
                    }
                }

                scope.Complete();

                return codigo;
            }
        }

        #endregion

        /// <summary>
        /// Retorna DIRETORIO do Portal atual
        /// </summary>
        public static string Url
        {
            get
            {
                // tenta buscar portal com informações do request
                try
                {
                    var urlLocalPath = HttpContextFactory.Current.Request.Url.LocalPath.ToLowerInvariant();

                    if (urlLocalPath.Equals("/"))
                        return string.Empty;

                    if (!urlLocalPath.StartsWith(PORTAL_PREFIX) && HttpContextFactory.Current.Request.UrlReferrer != null)
                        urlLocalPath = HttpContextFactory.Current.Request.UrlReferrer.LocalPath.ToLowerInvariant();

                    var diretorioPortal = urlLocalPath.Split('/').Where(a => !string.IsNullOrEmpty(a) && !a.Equals(PORTAL_PREFIX.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToArray();

                    if (diretorioPortal.Length > 0)
                    {
                        MLPortal portal = Portais.Find(a => a.Diretorio.Equals(diretorioPortal[0], StringComparison.InvariantCultureIgnoreCase));

                        if (portal != null)
                            return portal.Diretorio;
                    }
                }
                catch { /*Não dar erro se nao tiver Request.*/ }

                return string.Empty;
            }
        }

        public static string UrlPublica
        {
            get
            {
                var retorno = Url;

                if (!string.IsNullOrEmpty(retorno))
                    return string.Concat("/", retorno, "/");
                else
                    return "/";
            }
        }
    }
}
