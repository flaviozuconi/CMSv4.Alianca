using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.UI.HtmlControls;
using Framework.DataLayer;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.Model.Conteudo;
using System.Web.Mvc;

namespace CMSv4.BusinessLayer
{
    public class BLConteudo : BLCRUD<MLConteudoEdicao>
    {
        private string PASTA_CONTEUDO_FISICA = "";
        private string CONNECTION_STRING = "";
        private string PASTA_CONTEUDO = "~/portal/{0}/paginas/";
        private const string EXTENSAO_ARQUIVOS = ".cshtml";
        private const string HEAD_VISUALIZAR = @"
            @section head {
                <link href=""@Url.Content(""~/content/areaconstrucao/areaconstrucao.css"")"" rel=""stylesheet"" type=""text/css"" />
                <link href=""@Url.Content(""~/content/css/plugins.css"")"" rel=""stylesheet"" type=""text/css"" />
                @CMSv4.BusinessLayer.BLConteudo.GetTextForHead()
                @Html.Raw(CMSv4.BusinessLayer.BLConteudo.GetFreeTextForHead())
#STYLE_PAGINA#
            }
            @section body {
            }
            @section scripts {
                <script src=""~/Content/js/plugins/ckeditor/ckeditor.js""></script>
                <script src=""~/Content/js/plugins/ckeditor/adapters/jquery.js""></script>
                <script src=""~/Content/js/plugins/html2canvas/html2canvas.js""></script>
                <script src=""@Url.Content(""~/content/areaconstrucao/areaconstrucao.js"")"" type=""text/javascript""></script>
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScripts()
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScriptsView()
#SCRIPT_PAGINA#
            }
        ";

        private const string HEAD_PUBLICAR = @"
            @section head {
                @CMSv4.BusinessLayer.BLConteudo.GetTextForHead()
                @Html.Raw(CMSv4.BusinessLayer.BLConteudo.GetFreeTextForHead())
#STYLE_PAGINA#
            }
            @section scripts {
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScriptsView()
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScripts()
#SCRIPT_PAGINA#
            }
        ";

        #region BLConteudo
        /// <summary>
        /// BLConteudo
        /// </summary>
        /// <param name="pastaBase"></param>
        /// <param name="connectionString"></param>
        public BLConteudo(string pastaBase, string connectionString)
        {
            PASTA_CONTEUDO = string.Format(PASTA_CONTEUDO, pastaBase);
            if (string.IsNullOrEmpty(PASTA_CONTEUDO_FISICA)) PASTA_CONTEUDO_FISICA = HttpContext.Current.Server.MapPath(PASTA_CONTEUDO);

            if (!Directory.Exists(PASTA_CONTEUDO_FISICA)) Directory.CreateDirectory(PASTA_CONTEUDO_FISICA);

            CONNECTION_STRING = connectionString;
        }

        public BLConteudo() {}

        #endregion

        #region CRUD

        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;

                if (string.IsNullOrEmpty(connectionString))
                    connectionString = portal.ConnectionString;

                var retorno = CRUD.Excluir<MLConteudoEdicao>(ids, portal.ConnectionString);
                CRUD.Excluir<MLConteudoPublicado>(ids, portal.ConnectionString);

                return retorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Obter

        public override MLConteudoEdicao Obter(decimal Codigo, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;

                if (string.IsNullOrWhiteSpace(connectionString))
                    connectionString = portal.ConnectionString;

                var model = base.Obter(Codigo, connectionString);

                if (model == null || !model.Codigo.HasValue)
                {
                    model = new MLConteudoEdicao();
                    model.CodigoIdioma = PortalAtual.Obter.CodigoIdioma;
                }

                if (Codigo == 0)
                {
                    if (HttpContextFactory.Current.Request.QueryString["CodigoIdioma"] != null)
                        model.CodigoIdioma = Convert.ToDecimal(HttpContextFactory.Current.Request.QueryString["CodigoIdioma"]);

                    if (HttpContextFactory.Current.Request.QueryString["CodigoBase"] != null)
                        model.CodigoBase = Convert.ToDecimal(HttpContextFactory.Current.Request.QueryString["CodigoBase"]);
                }

                if (!string.IsNullOrEmpty(model.Conteudo))
                {
                    model.Conteudo = model.Conteudo.Unescape();
                }

                // Obter Grupo Aprovador
                decimal? CodigoGrupoAprovador = BLModulo.Obter(16, portal.ConnectionString).CodigoGrupoAprovador;

                // APROVADORES
                if (CodigoGrupoAprovador.HasValue)
                {
                    model.CodigoGrupoAprovador = CodigoGrupoAprovador;
                    model.Aprovadores = BLUsuario.ListarAprovadores(portal.Codigo.Value, CodigoGrupoAprovador.Value);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Salvar

        public override decimal Salvar(MLConteudoEdicao model, string connectionString = "")
        {
            try
            {

                var Autorizar = Convert.ToBoolean(HttpContextFactory.Current.Request.Form["Autorizar"]);
                var Publicar = Convert.ToBoolean(HttpContextFactory.Current.Request.Form["Publicar"]);

                var portal = PortalAtual.Obter;
                var publicarAgora = false;
                model.CodigoPortal = portal.Codigo;

                //validar se ja existe um item com a mesma chave
                var conteudoEdicao =base.Obter(new MLConteudoEdicao { Chave = model.Chave, CodigoPortal = portal.Codigo }, portal.ConnectionString);

                if (conteudoEdicao != null && conteudoEdicao.Codigo > 0 && conteudoEdicao.Codigo != model.Codigo)
                    return 0;

                using (var scope = new TransactionScope(portal.ConnectionString))
                {
                    model.Editado = true;

                    if (Publicar) { model.DataPublicacao = null; }
                    if (Autorizar)
                    {
                        var usuario = BLUsuario.ObterLogado();
                        decimal? CodigoGrupoAprovador = BLModulo.Obter(16, portal.ConnectionString).CodigoGrupoAprovador;

                        if (!CodigoGrupoAprovador.HasValue || usuario.Grupos.Find(o => o.CodigoGrupo == CodigoGrupoAprovador.Value) != null)
                        {
                            model.DataAprovacao = DateTime.Now;
                            model.CodigoUsuarioAprovacao = usuario.Codigo;

                            if (!model.DataPublicacao.HasValue || model.DataPublicacao.Value < DateTime.Now) publicarAgora = true;
                        }
                    }

                    model.Codigo = base.Salvar(model, portal.ConnectionString);

                    //Gerar histórico da modificação
                    var historico = new MLConteudoHistorico()
                    {
                        CodigoConteudo = model.Codigo,
                        CodigoUsuario = BLUsuario.ObterLogado().Codigo,
                        DataCadastro = DateTime.Now,
                        DataPublicacao = model.DataPublicacao,
                        Conteudo = model.Conteudo,
                        Chave = model.Chave
                    };

                    CRUD.Salvar(historico, portal.ConnectionString);

                    scope.Complete();
                }

                if (publicarAgora)
                {
                    using (var scope = new TransactionScope(portal.ConnectionString))
                    {
                        var edicao = CRUD.Obter<MLConteudoEdicao>(model.Codigo.Value, portal.ConnectionString);
                        var publicar = new MLConteudoPublicado();
                        CRUD.CopiarValores(edicao, publicar);

                        edicao.Publicado = true;
                        edicao.Editado = false;
                        CRUD.Salvar(edicao, portal.ConnectionString);
                        CRUD.Salvar(publicar, portal.ConnectionString);
                        scope.Complete();
                    }
                }
                
                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #endregion

        #region MontarCaminhoView

        public string MontarNomeView(decimal codigoPagina)
        {
            return string.Format("p{0}", codigoPagina);
        }

        public string MontarCaminhoView(decimal codigoPagina)
        {
            return PASTA_CONTEUDO + MontarNomeView(codigoPagina) + EXTENSAO_ARQUIVOS;
        }

        public string MontarCaminhoFisicoView(decimal codigoPagina)
        {
            return Path.Combine(PASTA_CONTEUDO_FISICA, MontarNomeView(codigoPagina) + EXTENSAO_ARQUIVOS);
        }

        public string MontarCaminhoLayout(string nome)
        {
            return PASTA_CONTEUDO + nome + EXTENSAO_ARQUIVOS;
        }

        public string MontarCaminhoFisicoLayout(string nome)
        {
            return HttpContext.Current.Server.MapPath(nome);
        }

        #endregion

        #region Carregar Pagina Visualizar

        /// <summary>
        /// Carrega e atualiza o arquivo físico de visualização da página e retorna
        /// o nome do arquivo (view) gerado
        /// </summary>
        public string CarregarPaginaVisualizar(MLPortal portal, MLPaginaAdmin model)
        {
            if (model == null || !model.Codigo.HasValue) return "";

            string nomeArquivo = string.Format("v{0}i{1}", model.Codigo, Guid.NewGuid().ToString());

            // Montar Template
            string template = "";
            if (string.IsNullOrEmpty(model.PaginaEdicao.TemplateCustomizado))
            {
                if (!string.IsNullOrEmpty(model.PaginaEdicao.NomeTemplate))
                {
                    template = BLTemplate.CarregarArquivo(portal, model.PaginaEdicao.NomeTemplate);
                }
                else
                    template = "";
            }
            else
                template = model.PaginaEdicao.TemplateCustomizado;

            // Substituir Repositórios por Chamada dos módulos (sempre dinâmico neste caso)
            template = string.Concat(SubstituirRepositoriosEdicao(template, model.Codigo.Value, model.PaginaEdicao.Modulos), Environment.NewLine, HEAD_VISUALIZAR);

            if (!string.IsNullOrEmpty(model.PaginaEdicao.Css))
                template = template.Replace("#STYLE_PAGINA#", $"<style type=\"text/css\">{model.PaginaEdicao.Css.Unescape()}</style>");
            else
                template = template.Replace("#STYLE_PAGINA#", string.Empty);
                
            if (!string.IsNullOrEmpty(model.PaginaEdicao.Scripts))
                template = template.Replace("#SCRIPT_PAGINA#", $"<script type=\"text/javascript\">{model.PaginaEdicao.Scripts.Unescape()}</script>");
            else
                template = template.Replace("#SCRIPT_PAGINA#", string.Empty);

            // Escrever a view em disco
            var arquivoCaminhoCompleto = Path.Combine(PASTA_CONTEUDO_FISICA, nomeArquivo) + ".cshtml";
            File.WriteAllText(arquivoCaminhoCompleto, template);
            BLReplicar.Arquivo(arquivoCaminhoCompleto);

            return PASTA_CONTEUDO + nomeArquivo + ".cshtml";
        }

        #endregion

        #region Carregar Pagina Publicada

        public string CarregarPaginaPublicada(decimal codigo, bool sobrescrever)
        {
            return CarregarPaginaPublicada(new BLPagina(CONNECTION_STRING).ObterPaginaAdmin(codigo), sobrescrever);
        }

        /// <summary>
        /// Carrega e atualiza o arquivo físico de visualização da página e retorna
        /// o nome do arquivo (view) gerado
        /// </summary>
        public string CarregarPaginaPublicada(MLPaginaAdmin model, bool sobrescrever)
        {
            if (model == null || !model.Codigo.HasValue) return "";

            string nomeArquivo = MontarNomeView(model.Codigo.Value);
            var arquivoCaminho = MontarCaminhoView(model.Codigo.Value);
            var arquivoCaminhoFisico = MontarCaminhoFisicoView(model.Codigo.Value);


            if (!sobrescrever)
            {
                if (File.Exists(arquivoCaminhoFisico)) return arquivoCaminho;
            }

            EscreverPaginaPublicada(model.PaginaPublicada, CONNECTION_STRING);

            return arquivoCaminho;
        }

        #endregion

        #region Substituir Repositorios Edição

        /// <summary>
        /// Substitui as marcações de repositório pelos módulos colocados na página
        /// </summary>
        private string SubstituirRepositoriosEdicao(string template, decimal codigoPagina, List<MLPaginaModuloEdicao> modulos)
        {
            if (string.IsNullOrEmpty(template)) return "";

            var modoVisualizacao = System.Web.HttpContext.Current.Request.QueryString["edit"] == null;

            foreach (var modulo in modulos)
            {
                if (modoVisualizacao)
                {
                    template = template.Replace("{R" + modulo.Repositorio.Value.ToString("00") + "}",
                                string.Format(@"<div class=""clearfix cms-ac-container"">
                                    @{{ Html.RenderAction(""visualizar"", ""{0}"", new {{ area=""Modulo"", codigoPagina = ""{1}"", repositorio = ""{2}"", edicao=true }}); }} </div>",
                                    modulo.UrlModulo,
                                    modulo.CodigoPagina,
                                    modulo.Repositorio
                                    ));
                }
                else
                {
                    template = template.Replace("{R" + modulo.Repositorio.Value.ToString("00") + "}",
                                string.Format(@"<div class=""clearfix cms-ac-container"">
                                    @Html.Partial(""~/areas/modulo/views/shared/toolbar.cshtml"", new ViewDataDictionary{{ {{ ""urlModulo"", ""{0}"" }}, {{ ""codigoPagina"" , {1} }}, {{ ""repositorio"", ""{2}""}}, {{ ""editavel"",""{3}""}}  }})
                                    @{{ Html.RenderAction(""visualizar"", ""{0}"", new {{ area=""Modulo"", codigoPagina = ""{1}"", repositorio = ""{2}"", edicao=true }}); }} </div>",
                                    modulo.UrlModulo,
                                    modulo.CodigoPagina,
                                    modulo.Repositorio,
                                    modulo.Editavel
                                    ));
                }
            }

            for (var i = 1; i <= 99; i++)
            {
                var rep = "{R" + i.ToString("00") + "}";

                if (modoVisualizacao)
                {
                    template = template.Replace(rep, string.Empty);
                }
                else
                {
                    template = template.Replace(rep,
                        string.Format(@"
                        @{{ Html.RenderAction(""NovoModulo"", ""Pagina"", new {{ area=""CMS"", codigoPagina = ""{0}"", repositorio = ""{1}"" }}); }}",
                            codigoPagina,
                            i
                            ));
                }
            }

            return template;
        }


        #endregion

        // PUBLICACAO

        #region Publicar

        public void EscreverPaginaPublicada(MLPaginaPublicada model, string connectionString)
        {
            try
            {
                // Modulos
                var modulos = BLModulo.Listar(connectionString);

                // Criar Arquivo
                string nomeArquivo = string.Format("p{0}", model.Codigo);
                string template = "";

                // Montar Template                
                if (string.IsNullOrEmpty(model.TemplateCustomizado))
                {
                    if (!string.IsNullOrEmpty(model.NomeTemplate))
                        template = BLTemplate.CarregarArquivo(BLPortal.Atual, model.NomeTemplate);
                    else
                        template = "";
                }
                else
                    template = model.TemplateCustomizado;

                template = string.Concat(template, Environment.NewLine, HEAD_PUBLICAR);

                var customCss = string.Empty;
                var customJs = string.Empty;

                if (!string.IsNullOrWhiteSpace(model.Css))
                    customCss = $"<link rel=\"stylesheet\" href=\"{PASTA_CONTEUDO}/custom_style_{model.Codigo}.css\">";

                if (!string.IsNullOrWhiteSpace(model.Scripts))
                    customJs = $"<script type=\"text/javascript\" src=\"{PASTA_CONTEUDO}/custom_script_{model.Codigo}.js\"></script>";

                template = template.Replace("#STYLE_PAGINA#", customCss);
                template = template.Replace("#SCRIPT_PAGINA#", customJs);

                // Salvar
                using (var scope = new TransactionScope(connectionString))
                {
                    var objBLPagina = new BLPagina(connectionString);

                    var codigoHistorico = objBLPagina.GerarHistorico(model.Codigo.Value);

                    CRUD.Salvar(model);

                    //Excluir de Edição
                    objBLPagina.ExcluirModulosEdicao(model.Codigo.Value);
                    objBLPagina.ExcluirModulosPublicados(model.Codigo.Value);

                    var path = HttpContext.Current.Server.MapPath("/bin");

                    foreach (var item in model.Modulos)
                    {
                        CRUD.Salvar(item);

                        System.Reflection.Assembly assembly;
                        try
                        {
                            var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                            if (moduloEncontrado == null
                                || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                                || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                            assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                            var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                            var method = moduloType.GetMethod("Publicar");
                            template = template.Replace("{R" + item.Repositorio.Value.ToString("00") + "}",
                                Convert.ToString(method.Invoke(null, new object[] { item.UrlModulo, item.CodigoPagina, item.Repositorio, codigoHistorico })));
                        }
                        catch (Exception ex)
                        {
                            ApplicationLog.ErrorLog(ex);
                        }
                        finally
                        {
                            assembly = null;
                        }
                    }

                    //Excluir de Edição
                    CRUD.Excluir<MLPaginaEdicao>(model.Codigo.Value, connectionString);

                    scope.Complete();
                }

                for (var i = 1; i <= 99; i++)
                {
                    var rep = "{R" + i.ToString("00") + "}";
                    template = template.Replace(rep, "");
                }

                // Escrever a view em disco
                var arquivoCaminhoCompleto = Path.Combine(PASTA_CONTEUDO_FISICA, nomeArquivo) + ".cshtml";
                var arquivoCssCaminhoCompleto = $"{PASTA_CONTEUDO_FISICA}custom_style_{model.Codigo}.css";
                var arquivoJsCaminhoCompleto = $"{PASTA_CONTEUDO_FISICA}custom_script_{model.Codigo}.js";

                File.WriteAllText(arquivoCaminhoCompleto, template);
                BLReplicar.Arquivo(arquivoCaminhoCompleto);
                
                if(!string.IsNullOrWhiteSpace(model.Css))
                {
                    File.WriteAllText(arquivoCssCaminhoCompleto, model.Css.Unescape());
                    BLReplicar.Arquivo(arquivoCssCaminhoCompleto);
                }

                if (!string.IsNullOrWhiteSpace(model.Scripts))
                {
                    File.WriteAllText(arquivoJsCaminhoCompleto, model.Scripts.Unescape());
                    BLReplicar.Arquivo(arquivoJsCaminhoCompleto);
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }


        #endregion

        // HISTORICO

        #region Carregar Pagina Historico

        /// <summary>
        /// Carrega e atualiza o arquivo físico de visualização da página e retorna
        /// o nome do arquivo (view) gerado
        /// </summary>
        public string CarregarPaginaHistorico(MLPortal portal, MLPaginaHistorico model, Guid codigoHistorico)
        {
            if (model == null || !model.Codigo.HasValue) return "";

            string nomeArquivo = string.Format("v{0}i{1}", model.Codigo, Guid.NewGuid().ToString());

            // Montar Template
            string template = "";
            if (string.IsNullOrEmpty(model.TemplateCustomizado))
            {
                if (!string.IsNullOrEmpty(model.NomeTemplate))
                {
                    template = BLTemplate.CarregarArquivo(portal, model.NomeTemplate);
                }
                else
                    template = "";
            }
            else
                template = model.TemplateCustomizado;

            // Substituir Repositórios por Chamada dos módulos (sempre dinâmico neste caso)
            template = string.Concat(SubstituirRepositoriosHistorico(template, model.Codigo.Value, codigoHistorico, model.Modulos), Environment.NewLine, HEAD_VISUALIZAR);

            var customCss = string.Empty;
            var customJs = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.Css))
                customCss = "<style type='text/css'>" + model.Css.Unescape() + "</style>";

            if (!string.IsNullOrWhiteSpace(model.Scripts))
                customJs = "<script type='text/javascript'>" + model.Scripts.Unescape() + "</script>";

            template = template.Replace("#STYLE_PAGINA#", customCss);
            template = template.Replace("#SCRIPT_PAGINA#", customJs);

            // Escrever a view em disco
            var arquivoCaminhoCompleto = Path.Combine(PASTA_CONTEUDO_FISICA, nomeArquivo) + ".cshtml";
            File.WriteAllText(arquivoCaminhoCompleto, template);
            BLReplicar.Arquivo(arquivoCaminhoCompleto);

            return PASTA_CONTEUDO + nomeArquivo + ".cshtml";

        }

        #endregion

        #region Substituir Repositorios Edição

        /// <summary>
        /// Substitui as marcações de repositório pelos módulos colocados na página
        /// </summary>
        private string SubstituirRepositoriosHistorico(string template, decimal codigoPagina, Guid codigoHistorico, List<MLPaginaModuloHistorico> modulos)
        {
            if (string.IsNullOrEmpty(template)) return "";

            foreach (var modulo in modulos)
            {
                template = template.Replace("{R" + modulo.Repositorio.Value.ToString("00") + "}",
                    string.Format(@"
                        @{{ Html.RenderAction(""visualizar"", ""{0}"", new {{ area=""Modulo"", codigoPagina = ""{1}"", repositorio = ""{2}"", edicao=false, codigoHistorico = ""{4}"" }}); }}",
                        modulo.UrlModulo,
                        modulo.CodigoPagina,
                        modulo.Repositorio,
                        modulo.Editavel,
                        codigoHistorico.ToString()
                        ));
            }

            for (var i = 1; i <= 99; i++)
            {
                var rep = "{R" + i.ToString("00") + "}";

                template = template.Replace(rep, "");
            }

            return template;
        }


        #endregion

        #region CopiarPublicadaParaEdicao

        /// <summary>
        /// Copia a página de publicação para edição, caso ela ainda não exista ou seja marcada para recuperar
        /// </summary>
        public static MLPaginaEdicao CopiarPublicadaParaEdicao(decimal id, bool recuperar, string connectionString)
        {

            try
            {
                var pagina = new BLPagina(connectionString).ObterPaginaAdmin(id);

                if ((!pagina.PaginaEdicao.Codigo.HasValue && pagina.PaginaPublicada.Codigo.HasValue)
                   || (pagina.PaginaPublicada.Codigo.HasValue && recuperar)
                   )
                {
                    // copiar de publicado para edicao
                    var model = new MLPaginaEdicao()
                    {
                        ApresentarNaBusca = pagina.PaginaPublicada.ApresentarNaBusca,
                        Codigo = pagina.PaginaPublicada.Codigo,
                        NomeLayout = pagina.PaginaPublicada.NomeLayout,
                        NomeTemplate = pagina.PaginaPublicada.NomeTemplate,
                        DataEdicao = DateTime.Now,
                        Descricao = pagina.PaginaPublicada.Descricao,
                        Tags = pagina.PaginaPublicada.Tags,
                        TemplateCustomizado = pagina.PaginaPublicada.TemplateCustomizado,
                        Scripts = pagina.PaginaPublicada.Scripts,
                        Css = pagina.PaginaPublicada.Css,
                        Titulo = pagina.PaginaPublicada.Titulo,
                        UsuarioEditor = BLUsuario.ObterLogado().Codigo,
                        UrlLogin = pagina.PaginaPublicada.UrlLogin
                    };

                    foreach (var item in pagina.PaginaPublicada.Modulos)
                    {
                        model.Modulos.Add(new MLPaginaModuloEdicao()
                        {
                            CodigoModulo = item.CodigoModulo,
                            CodigoPagina = item.CodigoPagina,
                            Repositorio = item.Repositorio,
                            UrlModulo = item.UrlModulo
                        });
                    }

                    // Modulos
                    var modulos = BLModulo.Listar(connectionString);

                    // Salvar
                    using (var scope = new TransactionScope(connectionString))
                    {
                        CRUD.Salvar<MLPaginaEdicao>(model);

                        var path = HttpContext.Current.Server.MapPath("/bin");

                        foreach (var item in model.Modulos)
                        {
                            CRUD.Salvar<MLPaginaModuloEdicao>(item);

                            System.Reflection.Assembly assembly;
                            try
                            {
                                var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                                if (moduloEncontrado == null
                                    || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                                    || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                                assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                                var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                                var method = moduloType.GetMethod("CarregarConteudo");
                                method.Invoke(null, new object[] { item.CodigoPagina, item.Repositorio });
                            }
                            catch (Exception ex)
                            {
                                ApplicationLog.ErrorLog(ex);
                            }
                            finally
                            {
                                assembly = null;
                            }
                        }

                        scope.Complete();
                    }

                    return model;
                }

                return null;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }


        #endregion

        #region CopiarHistoricoParaEdicao

        /// <summary>
        /// Copia a página armazenada em histórico para edição, caso ela ainda não exista ou seja marcada para recuperar
        /// </summary>
        public static MLPaginaEdicao CopiarHistoricoParaEdicao(Guid codigoHistorico, string connectionString)
        {

            try
            {
                var pagina = new BLPagina(connectionString).ObterPaginaHistorico(codigoHistorico);

                if (pagina.Codigo.HasValue)
                {
                    // copiar de publicado para edicao
                    var model = new MLPaginaEdicao()
                    {
                        ApresentarNaBusca = pagina.ApresentarNaBusca,
                        Codigo = pagina.Codigo,
                        NomeLayout = pagina.NomeLayout,
                        NomeTemplate = pagina.NomeTemplate,
                        DataEdicao = DateTime.Now,
                        Descricao = pagina.Descricao,
                        Tags = pagina.Tags,
                        TemplateCustomizado = pagina.TemplateCustomizado,
                        Scripts = pagina.Scripts,
                        Titulo = pagina.Titulo,
                        UsuarioEditor = BLUsuario.ObterLogado().Codigo
                    };

                    foreach (var item in pagina.Modulos)
                    {
                        model.Modulos.Add(new MLPaginaModuloEdicao()
                        {
                            CodigoModulo = item.CodigoModulo,
                            CodigoPagina = item.CodigoPagina,
                            Repositorio = item.Repositorio,
                            UrlModulo = item.UrlModulo
                        });
                    }

                    // Modulos
                    var modulos = CRUD.Listar<MLModulo>(new MLModulo() { Ativo = true }, connectionString).OrderBy(x => x.Nome).ToList();

                    // Salvar
                    using (var scope = new TransactionScope(connectionString))
                    {
                        CRUD.Salvar<MLPaginaEdicao>(model);

                        var path = HttpContext.Current.Server.MapPath("/bin");
                        CRUD.Excluir<MLPaginaModuloEdicao>("CodigoPagina", model.Codigo.Value, connectionString);
                        foreach (var item in model.Modulos)
                        {
                            CRUD.Salvar<MLPaginaModuloEdicao>(item);

                            System.Reflection.Assembly assembly;
                            try
                            {
                                var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                                if (moduloEncontrado == null
                                    || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                                    || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                                assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                                var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                                var method = moduloType.GetMethod("CarregarHistorico");
                                method.Invoke(null, new object[] { item.CodigoPagina, item.Repositorio, codigoHistorico });
                            }
                            catch (Exception ex)
                            {
                                ApplicationLog.ErrorLog(ex);
                            }
                            finally
                            {
                                assembly = null;
                            }
                        }

                        scope.Complete();
                    }

                    return model;
                }

                return null;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }


        #endregion

        #region AdicionarSeo
        /// <summary>
        /// Adiciona as Tags no cabeçalho da Página
        /// </summary>
        /// <param name="seo"></param>
        public static void AdicionarSeo(MLConteudoSeo seo)
        {
            try
            {
                #region Open Graph

                if (!string.IsNullOrEmpty(seo.Ogtotitle)) AdicionarMetaAoHead("og:title", seo.Ogtotitle);
                if (!string.IsNullOrEmpty(seo.Ogsitename)) AdicionarMetaAoHead("og:site_name", seo.Ogsitename);
                if (!string.IsNullOrEmpty(seo.Url)) AdicionarMetaAoHead("og:url", seo.Url);
                if (!string.IsNullOrEmpty(seo.Ogdescription)) AdicionarMetaAoHead("og:description", seo.Ogdescription);
                if (!string.IsNullOrEmpty(seo.Ogtype)) AdicionarMetaAoHead("og:type", seo.Ogtype);
                if (!string.IsNullOrEmpty(seo.Oglocale)) AdicionarMetaAoHead("og:locale", seo.Oglocale);
                if (!string.IsNullOrEmpty(seo.Ogimage)) AdicionarMetaAoHead("property", "og:image", seo.Ogimage);

                #endregion

                #region Twitter

                if (!string.IsNullOrEmpty(seo.Twittercard)) AdicionarMetaAoHead("name", "twitter:card", seo.Twittercard);
                if (!string.IsNullOrEmpty(seo.Twittersite)) AdicionarMetaAoHead("name", "twitter:site", seo.Twittersite);
                if (!string.IsNullOrEmpty(seo.Twittertitle)) AdicionarMetaAoHead("name", "twitter:title", seo.Twittertitle);
                if (!string.IsNullOrEmpty(seo.Twitterdescription)) AdicionarMetaAoHead("name", "twitter:description", seo.Twitterdescription);
                if (!string.IsNullOrEmpty(seo.Twitterimage)) AdicionarMetaAoHead("name", "twitter:image", seo.Twitterimage);
                if (!string.IsNullOrEmpty(seo.Url)) AdicionarMetaAoHead("name", "twitter:url", seo.Url);

                #endregion

                #region Google Plus

                //AdicionarMetaAoHead("itemprop", "description", seo.Ogdescription ?? seo.Description);

                #endregion

                if (!string.IsNullOrEmpty(seo.Url)) AdicionarLinkAoHead("canonical", seo.Url);

                if (!string.IsNullOrEmpty(seo.Titulo)) AdicionarMetaAoHead("itemprop", "name", seo.Titulo);

                if (!string.IsNullOrEmpty(seo.Description))
                {
                    AdicionarMetaAoHead("itemprop", "description", seo.Description);
                    AdicionarMetaAoHead("description", seo.Description);
                }

                if (!string.IsNullOrEmpty(seo.Tags)) AdicionarMetaAoHead("keywords", seo.Tags);

                if (!string.IsNullOrEmpty(seo.Copyright)) AdicionarMetaAoHead("copyright", seo.Copyright);
                if (!string.IsNullOrEmpty(seo.Revisitafter)) AdicionarMetaAoHead("revisit-after", seo.Revisitafter);
                if (!string.IsNullOrEmpty(seo.Author)) AdicionarMetaAoHead("author", seo.Author);

                if (!string.IsNullOrEmpty(seo.OgAuthor)) AdicionarMetaAoHead("article:author", seo.OgAuthor);
                if (!string.IsNullOrEmpty(seo.Ogpublisher)) AdicionarMetaAoHead("article:publisher", seo.Ogpublisher);

                if (!string.IsNullOrEmpty(seo.Shortcuticon)) AdicionarLinkAoHead("shortcut icon", seo.Shortcuticon);
                if (!string.IsNullOrEmpty(seo.Appletouchicon)) AdicionarLinkAoHead("apple-touch-icon", seo.Appletouchicon);
                
                if (!string.IsNullOrEmpty(seo.Outros))
                {
                    AdicionarAoHead(seo.Outros.Unescape());
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        // HEAD HELPER

        #region AdicionarAoHead

        /// <summary>
        /// Adiciona itens ao cabeçalho da página durante a renderização
        /// Apenas para módulos dinâmicos
        /// Usar na CONTROLLER que vai renderiza ro módulo
        /// </summary>
        public static void AdicionarAoHead(HtmlControl controle)
        {
            const string ID = "DynamicHeadContent";
            var existe = (List<HtmlControl>)CallContext.GetData(ID);
            if (existe == null) existe = new List<HtmlControl>();

            var item = existe.Find(o => o.Attributes["property"] == controle.Attributes["property"] && controle.Attributes["property"] != null);

            if (item != null)
                existe.Remove(item);
            existe.Add(controle);
            CallContext.SetData(ID, existe);
        }
        public static void AdicionarAoHead(string conteudo)
        {
            const string ID = "DynamicFreeHeadContent";
            var existe = (List<string>)CallContext.GetData(ID);
            if (existe == null) existe = new List<string>();

            var item = existe.Find(o => !string.IsNullOrEmpty(o) && o.Equals(conteudo, StringComparison.InvariantCultureIgnoreCase));

            if (item != null)
                existe.Remove(item);

            existe.Add(conteudo);

            CallContext.SetData(ID, existe);
        }

        /// <summary>
        /// Adicionar scripts
        /// </summary>
        public static void AdicionarJavaScript(string script, bool addScriptTag = true)
        {
            var controle = new HtmlGenericControl();

            if (addScriptTag)
            {
                controle.TagName = "script";
                controle.Attributes.Add("type", "text/javascript");
                controle.InnerHtml = script;

                const string ID = "DynamicScriptContent";

                var existe = (List<HtmlControl>)CallContext.GetData(ID);
                if (existe == null) existe = new List<HtmlControl>();

                existe.Add(controle);
                CallContext.SetData(ID, existe);
            }
            else
            {
                const string ID = "DynamicScriptContentView";

                var existe = (List<MvcHtmlString>)CallContext.GetData(ID);
                if (existe == null) existe = new List<MvcHtmlString>();

                existe.Add(MvcHtmlString.Create(script));
                CallContext.SetData(ID, existe);
            }
        }

        /// <summary>
        /// Adicionar scripts
        /// </summary>
        public static void AdicionarJavaScript(MvcHtmlString script, bool addScriptTag = false)
        {
            AdicionarJavaScript(script.ToHtmlString(), addScriptTag);
        }

        /// <summary>
        /// Adicionar scripts
        /// </summary>
        public static void AdicionarMetaAoHead(string property, string content)
        {
            AdicionarMetaAoHead("property", property, content);
        }

        public static void AdicionarMetaAoHead(string atributo, string property, string content)
        {
            var controle = new HtmlMeta();
            controle.Attributes.Add(atributo, property);
            controle.Attributes.Add("content", content);

            AdicionarAoHead(controle);
        }

        public static void AdicionarLinkAoHead(string rel, string href)
        {
            var controle = new HtmlLink();
            controle.Attributes.Add("rel", rel);
            controle.Attributes.Add("href", href);

            AdicionarAoHead(controle);
        }

        public static void AdicionarTitleAoHead(string title)
        {
            const string ID = "DynamicHeadContent";
            var existe = (List<HtmlControl>)CallContext.GetData(ID);
            if (existe == null) existe = new List<HtmlControl>();

            var controle = new HtmlTitle();
            var titleexists = string.Empty;
            if (existe.Find(o => o is HtmlTitle) != null)
            {
                var controleaux = (HtmlTitle)existe.Find(o => o is HtmlTitle);
                titleexists = controleaux.Text;
                existe.Remove(controleaux);
            }

            controle.Text = !string.IsNullOrEmpty(titleexists) ? titleexists + " - " + title : title;

            existe.Add(controle);
            CallContext.SetData(ID, existe);
        }


        public static void AdicionarScriptAoHead(MvcHtmlString script)
        {
            AdicionarScriptAoHead(script.ToHtmlString());
        }

        public static void AdicionarScriptAoHead(string script)
        {
            var controle = new HtmlGenericControl("script");

            controle.Attributes.Add("type", "text/javascript");
            controle.InnerHtml = script;

            AdicionarAoHead(controle);
        }

        #endregion

        #region HeadRenderer - GetText

        public static MvcHtmlString GetTextForHead()
        {
            return GetDynamicContent("DynamicHeadContent");
        }

        public static MvcHtmlString GetFreeTextForHead()
        {
            var existe = (List<string>)CallContext.GetData("DynamicFreeHeadContent");
            if (existe == null) return MvcHtmlString.Create("");

            var valor = new StringBuilder();
            foreach (var item in existe)
            {
                valor.AppendLine(item);
            }

            return MvcHtmlString.Create(valor.ToString());
        }
        
        public static MvcHtmlString GetTextForScripts()
        {
            return GetDynamicContent("DynamicScriptContent");
        }

        public static MvcHtmlString GetTextForScriptsView()
        {


            var existe = (List<MvcHtmlString>)CallContext.GetData("DynamicScriptContentView");
            if (existe == null) return MvcHtmlString.Create(""); ;

            var valor = new StringBuilder();


            foreach (var item in existe)
            {
                valor.AppendLine(item.ToHtmlString());
            }


            return MvcHtmlString.Create(valor.ToString());

        }

        /// <summary>
        /// Retorna a lista de controles renderizados para a view
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString GetDynamicContent(string ID)
        {
            var existe = (List<HtmlControl>)CallContext.GetData(ID);
            if (existe == null) return MvcHtmlString.Create("");

            existe.Reverse();
            var auxiliar = new List<string>();

            var valor = new StringBuilder();
            foreach (var item in existe)
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    var htmlTextWriter = new System.Web.UI.HtmlTextWriter(stringWriter);

                    item.RenderControl(htmlTextWriter);
                    var itemStr = stringWriter.ToString();

                    if (IsInsere(itemStr, auxiliar))
                    {
                        valor.AppendLine(itemStr);
                        auxiliar.Add(itemStr);
                    }

                    htmlTextWriter.Close();
                    stringWriter.Close();
                }
            }
            return MvcHtmlString.Create(valor.ToString());
        }



        #endregion

        #region ReturnProperty
        /// <summary>
        /// ReturnProperty
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static string ReturnProperty(string meta)
        {
            if (meta.Contains("itemprop=\"name\""))
                return "itemprop=\"name\"";

            if (meta.Contains("itemprop=\"description\""))
                return "itemprop=\"description\"";

            if (meta.Contains("property=\"description\""))
                return "property=\"description\"";

            if (meta.Contains("property=\"copyright\""))
                return "property=\"copyright\"";

            if (meta.Contains("property=\"revisit-after\""))
                return "property=\"revisit-after\"";

            if (meta.Contains("property=\"author\""))
                return "property=\"author\"";

            if (meta.Contains("property=\"og:title\""))
                return "property=\"og:title\"";

            if (meta.Contains("property=\"og:site_name\""))
                return "property=\"og:site_name\"";

            if (meta.Contains("property=\"og:url\""))
                return "property=\"og:url\"";

            if (meta.Contains("property=\"og:description\""))
                return "property=\"og:description\"";

            if (meta.Contains("property=\"og:type\""))
                return "property=\"og:type\"";

            if (meta.Contains("property=\"og:locale\""))
                return "property=\"og:locale\"";

            if (meta.Contains("property=\"oarticle:author\""))
                return "property=\"oarticle:author\"";

            if (meta.Contains("property=\"article:publisher\""))
                return "property=\"article:publisher\"";

            if (meta.Contains("name=\"twitter:card\""))
                return "name=\"twitter:card\"";

            if (meta.Contains("name=\"twitter:site\""))
                return "name=\"twitter:site\"";

            if (meta.Contains("name=\"twitter:title\""))
                return "name=\"twitter:title\"";

            if (meta.Contains("name=\"twitter:description\""))
                return "name=\"twitter:description\"";

            if (meta.Contains("name=\"twitter:image\""))
                return "name=\"twitter:image\"";

            if (meta.Contains("name=\"twitter:url\""))
                return "name=\"twitter:url\"";

            if (meta.Contains("rel=\"canonical\""))
                return "rel=\"canonical\"";

            return string.Empty;
        }
        #endregion

        #region IsInsere
        /// <summary>
        /// IsInsere
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="auxiliar"></param>
        public static bool IsInsere(string meta, List<string> auxiliar)
        {

            var prop = ReturnProperty(meta);

            if (!string.IsNullOrEmpty(prop))
                return auxiliar.Find(o => o.Contains(prop)) == null;

            return true;
        }
        #endregion

        //REDES SOCIAIS

        #region Redes Sociais

        #region GerarFacebookShareLink

        public static string GerarFacebookShareLink(string pstrUrl)
        {
            return GerarFacebookShareLink(pstrUrl, false);
        }

        /// <summary>
        /// Metodo utilizado para criar o
        /// link de compartilhamento do facebook.
        /// </summary>
        /// <param name="pstrTitulo">Titulo</param>
        /// <param name="pstrUrl">Url</param>
        /// <param name="pstrImg">Imagem</param>
        /// <param name="pstrDescricao">Descrição</param>
        public static string GerarFacebookShareLink(string pstrUrl, bool Encode)
        {
            //string retorno = String.Empty;

            ////Remove html
            //pstrDescricao = Regex.Replace(pstrDescricao, @"<(.|\n)*?>", string.Empty);
            //string strEndereco = Convert.ToString(HttpContext.Current.Request.Url).Replace(Convert.ToString(HttpContext.Current.Request.Url.LocalPath), String.Empty);

            ////Facebook
            //retorno = String.Format("http://www.facebook.com/sharer.php?s=100&p[url]={0}&p[images][0]={1}&p[title]={2}&p[summary]={3}",
            //                                String.Concat(strEndereco, pstrUrl), String.Concat(strEndereco, pstrImg), String.Format("{0}", pstrTitulo),
            //                                pstrDescricao);

            if (Encode)
                pstrUrl = HttpUtility.UrlEncode(pstrUrl);

            return "https://www.facebook.com/sharer/sharer.php?u=" + pstrUrl;

            //return retorno;
        }

        #endregion

        #region GerarTwitterLink

        public static string GerarTwitterLink(string pstrTitulo, string pstrUrl)
        {
            pstrTitulo = pstrTitulo.Replace("%", "%25");
            return String.Format("https://twitter.com/share?url={0}&text={1}", pstrUrl, pstrTitulo);
        }
        #endregion

        #region GerarTGoogleLink

        public static string GerarGoogleLink(string pstrUrl)
        {
            return String.Format("https://plus.google.com/share?url={0}", pstrUrl);
        }
        #endregion

        #region GerarLinkedinLink

        public static string GerarLinkedinLink(string pstrUrl, string pstrTitle, string pstrSumario, string pstrSource)
        {
            //0 - url
            //1 - title
            //2 - sumario
            //3 - source
            return String.Format("http://www.linkedin.com/shareArticle?mini=true&url={0}&title={1}&summary={2}&source={3}",
                pstrUrl, pstrTitle, pstrSumario, pstrSource);
        }

        public static string GerarLinkedinLink(string pstrUrl)
        {
            //0 - url
            return String.Format("http://www.linkedin.com/shareArticle?url={0}", pstrUrl);
        }

        #endregion

        #region EncurtarUrl

        public static string EncurtarUrl(string url)
        {
            string key = System.Configuration.ConfigurationManager.AppSettings["SM.API.Google.ShortUrl"];
            string post = "{\"longUrl\": \"" + url + "\"}";
            string shortUrl = url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);

            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();
                            shortUrl = Regex.Match(json, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ApplicationLog.ErrorLog(ex);
                // if Google's URL Shortner is down...
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                shortUrl = string.Empty;
            }
            return shortUrl;
        }

        #endregion

        #endregion

        // ASSINATURA - DADOS CDN E REDES SOCIAIS

        #region ObterAssinatura
        /// <summary>
        /// Obtém a assinatura com
        /// os dados da cdn e redes sociais
        /// </summary>
        /// <returns>Conteúdo da Assinatura</returns>
        public static MLConteudoPublicado ObterAssinatura(string chave, decimal? codigoIdioma, MLPortal portal)
        {
            MLConteudoPublicado retorno = null;

            string cacheKey = String.Format("portal_{0}_conteudo_assinatura_{1}_{2}", portal.Codigo, chave, codigoIdioma);

            retorno = BLCachePortal.Get<MLConteudoPublicado>(cacheKey);

            if (retorno != null) return retorno;

            try
            {
                using (var command = Database.NewCommand("USP_MOD_CON_S_ASSINATURA", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CON_C_CHAVE", SqlDbType.VarChar, 50, chave);
                    command.NewCriteriaParameter("@CON_IDI_N_CODIGO", SqlDbType.Decimal, 18, codigoIdioma);
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do conteúdo
                        retorno = Database.FillModel<MLConteudoPublicado>(dataset.Tables[0].Rows[0]);
                    }

                    BLCachePortal.Add(cacheKey, retorno);                    
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }
        #endregion

        #region Visualizar

        public string Visualizar(Controller controller, decimal id, string TempTemplate, string TempLayout)
        {
            var tradutor = new BLTraducao();
            var portal = PortalAtual.Obter;

            var model = new BLPagina(portal.ConnectionString).ObterPaginaAdmin(id);

            //Utilizar o lytout /template selecionado no select da área admin
            if (!string.IsNullOrWhiteSpace(TempLayout))
                model.PaginaEdicao.NomeLayout = TempLayout;

            if (!string.IsNullOrWhiteSpace(TempTemplate))
                model.PaginaEdicao.NomeTemplate = TempTemplate;

            if (model == null || !model.Codigo.HasValue) return tradutor.Obter("Página não encontrada");

            if (model.PaginaEdicao.Codigo.HasValue)
            {
                // Renderizar conteudo em edição
                if (string.IsNullOrEmpty(model.PaginaEdicao.NomeLayout)) return tradutor.Obter("Página sem layout definido");
                if (string.IsNullOrEmpty(model.PaginaEdicao.NomeTemplate)) return tradutor.Obter("Página sem template definido");

                var objBLConteudoAdmin = new BLConteudo(portal.Diretorio, portal.ConnectionString);

                string nomeLayout = BLLayout.ObterCaminhoRelativo(portal, TempLayout ?? model.PaginaEdicao.NomeLayout);
                if (string.IsNullOrEmpty(nomeLayout)) return tradutor.Obter("Página sem layout definido");

                string nomeView = objBLConteudoAdmin.CarregarPaginaVisualizar(portal, model);
                
                var stringRetorno = BLConteudoHelper.RenderViewToString(controller, nomeView, nomeLayout, model);

                try { File.Delete(HttpContext.Current.Server.MapPath(nomeView)); }
                catch { }

                return stringRetorno;
            }
            else if (model.PaginaPublicada.Codigo.HasValue)
            {
                // Renderizar conteudo publicado
                if (string.IsNullOrEmpty(model.PaginaPublicada.NomeLayout)) return tradutor.Obter("Página sem layout definido");
                if (string.IsNullOrEmpty(model.PaginaPublicada.NomeTemplate)) return tradutor.Obter("Página sem template definido");


                var objBLConteudoAdmin = new BLConteudo(portal.Diretorio, portal.ConnectionString);

                string nomeLayout = BLLayout.ObterCaminhoRelativo(portal, model.PaginaPublicada.NomeLayout);
                if (string.IsNullOrEmpty(nomeLayout)) return tradutor.Obter("Página sem layout definido");

                BLConteudo.AdicionarJavaScript(new MvcHtmlString(@"<script src=""/Content/js/plugins/html2canvas/html2canvas.js""></script>"));
                BLConteudo.AdicionarJavaScript(new MvcHtmlString(@"<script src=""/content/areaconstrucao/areaconstrucao.js?" + Guid.NewGuid().ToString() + @""" type=""text/javascript""></script>"));
                BLConteudo.AdicionarJavaScript(string.Concat("$(document).ready(function() { screenShot(document.body,'", PortalAtual.Diretorio, "', ", model.Codigo, "); });"));

                string nomeView = objBLConteudoAdmin.CarregarPaginaPublicada(model, false);

                return BLConteudoHelper.RenderViewToString(controller, nomeView, nomeLayout, model);
            }
            else
            {
                // Renderizar branco
                return tradutor.Obter("página vazia");
            }
        }

        #endregion
    }

    public class BLConteudoHistorico : BLCRUD<MLConteudoHistorico>
    {
        #region Recuperar

        public void Recuperar(decimal id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var modelHistorico = CRUD.Obter<MLConteudoHistorico>(id, portal.ConnectionString);
                var modelEdicao = CRUD.Obter<MLConteudoEdicao>(modelHistorico.CodigoConteudo.Value, portal.ConnectionString);

                modelEdicao.Conteudo = modelHistorico.Conteudo;
                modelEdicao.Chave = modelHistorico.Chave;

                CRUD.Salvar(modelEdicao, portal.ConnectionString);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Visualizar

        public string Visualizar(Controller controller, string id)
        {
            var tradutor = new BLTraducao();
            var portal = PortalAtual.Obter;
            var model = new BLPagina(portal.ConnectionString).ObterPaginaHistorico(new Guid(id));

            if (model == null || !model.Codigo.HasValue) return tradutor.Obter("Página não encontrada");

            // Validar permissões
            // if (false) return

            if (model.Codigo.HasValue)
            {
                // Renderizar conteudo em edição
                if (string.IsNullOrEmpty(model.NomeLayout)) return tradutor.Obter("Página sem layout definido");
                if (string.IsNullOrEmpty(model.NomeTemplate)) return tradutor.Obter("Página sem template definido");

                var objBLConteudoAdmin = new BLConteudo(PortalAtual.Diretorio, portal.ConnectionString);

                string nomeLayout = BLLayout.ObterCaminhoRelativo(portal, model.NomeLayout);
                if (string.IsNullOrEmpty(nomeLayout)) return tradutor.Obter("Página sem layout definido");

                string nomeView = objBLConteudoAdmin.CarregarPaginaHistorico(portal, model, new Guid(id));

                var stringRetorno = BLConteudoHelper.RenderViewToString(controller, nomeView, nomeLayout, model);

                try { File.Delete(HttpContext.Current.Server.MapPath(nomeView)); }
                catch { }

                return stringRetorno;
            }
            else
            {
                // Renderizar branco
                return tradutor.Obter("página vazia");
            }
        }

        #endregion
    }
}
