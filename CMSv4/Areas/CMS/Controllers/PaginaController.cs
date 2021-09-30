using CMSv4.BusinessLayer;
using CMSv4.BusinessLayer.DataTableFilter;
using CMSv4.BusinessLayer.Pagina;
using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSApp.Areas.CMS.Controllers
{

    public class PaginaController : SecurePortalController
    {
        //
        // GET: /CMS/Pagina/ 
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            var portal = PortalAtual.Obter;

            ViewData["secoes"] = BLSecao.ListarAdmin(portal, BLUsuario.ObterLogado());
            ViewData["idiomas"] = BLIdioma.Listar();

            return View();
        }

        //
        // GET: /CMS/Pagina/
        [HttpGet, CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Propriedades(decimal? codigoPagina, int? repositorio, string urlModulo)
        {
            var portal = PortalAtual.Obter;
            var modulo = CRUD.Obter(new MLModulo() { Url = urlModulo }) ?? new MLModulo();
            ViewData["codigoPagina"] = codigoPagina;
            ViewData["repositorio"] = repositorio;
            ViewData["urlModulo"] = urlModulo;
            ViewData["nomeModulo"] = modulo.Nome;
            //ViewData["abrirEdicao"] = 0;


            return PartialView();
        }

        #region Listar

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Listar(MLPagina criterios, bool? Ativo)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var usuario = BLUsuario.ObterLogado();
                var filter = new DataTableFilter(Request.QueryString).Get();

                criterios.CodigoPortal = portal.Codigo;

                var lista = new BLPagina(portal.ConnectionString).ListarAdmin(
                    filter.SearchedValue,
                    portal.Codigo,
                    criterios.CodigoSecao,
                    usuario.GruposToString(),
                    Ativo,
                    filter.OrderBy, 
                    filter.Sort,
                    filter.Start, 
                    filter.Length,
                    criterios.CodigoIdioma
                );

                return new DataTableResult()
                {
                    Data = lista,
                    TotalRows = (lista.Count > 0 ? lista[0].TotalRows : 0) ?? 0
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Item

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id, decimal? duplicar)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var blPagina = new BLPagina(portal.ConnectionString);

                var model = new MLPaginaAdmin();
                if (id > 0)
                    model = blPagina.ObterPaginaAdmin(id.Value);
                else if (duplicar.HasValue)
                {
                    model = blPagina.ObterPaginaAdmin(duplicar.Value);
                    model.Url = string.Empty;
                    model.Nome = string.Empty;
                    ViewData["duplicar"] = true;
                }

                if (Request.QueryString["codigoIdioma"] != null)
                    model.CodigoIdioma = Convert.ToDecimal(Request.QueryString["codigoIdioma"]);

                if (Request.QueryString["codigoPai"] != null)
                    model.CodigoPai = Convert.ToDecimal(Request.QueryString["codigoPai"]);

                var T = new BLTraducao(portal);

                if (model.Codigo.HasValue && !BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return View("erro550");

                /// Validar permissão de usuário 
                ViewData["layouts"] = BLLayout.ListarArquivos(portal);
                ViewData["templates"] = BLTemplate.ListarArquivos(portal);
                ViewData["secoes"] = BLSecao.ListarAdmin(portal, BLUsuario.ObterLogado());
                ViewData["idiomas"] = BLIdioma.Listar();

                //Se for tradução de uma página
                if ((!id.HasValue || id == 0) && !string.IsNullOrWhiteSpace(Request.QueryString["codigoPai"]))
                {
                    ViewBag.TraducaoPagina = true;

                    model = blPagina.ObterPaginaAdmin(Convert.ToDecimal(Request.QueryString["codigoPai"]));
                    model.Url = string.Empty;
                    model.Nome = string.Empty;
                    model.CodigoPai = Convert.ToDecimal(Request.QueryString["codigoPai"]);

                    //Forçar idioma da nova página para o idioma informado por query string.
                    model.CodigoIdioma = Convert.ToDecimal(Request.QueryString["codigoIdioma"]);

                    ViewData["duplicar"] = true;

                    return View("Nova", model);
                }

                if (!model.Codigo.HasValue || (!model.PaginaEdicao.Codigo.HasValue && !model.PaginaPublicada.Codigo.HasValue) || duplicar.HasValue)
                {
                    if (!duplicar.HasValue)
                    {
                        model.CodigoIdioma = PortalAtual.Obter.CodigoIdioma;
                    }

                    return View("Nova", model);
                }
                else if (model.PaginaEdicao != null && model.PaginaEdicao.Codigo.HasValue)
                {
                    model.PaginaEdicao.DataEdicao = DateTime.Now;
                    CRUD.SalvarParcial(model.PaginaEdicao, portal.ConnectionString);
                }

                //Permissões da Página
                ViewData["Permissoes"] = blPagina.ObterPermissoes(id.Value);
                ViewData["GruposCliente"] = CRUD.Listar(new MLGrupoCliente { Ativo = true, CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);

                ViewData["Templates"] = (from e in BLTemplate.ListarArquivos(PortalAtual.Obter) select new MLTemplate { Nome = Path.GetFileNameWithoutExtension(e.Nome) }).ToList();
                ViewData["Layouts"] = (from e in BLLayout.ListarArquivos(PortalAtual.Obter) select new MLLayout { Nome = Path.GetFileNameWithoutExtension(e) }).ToList();

                //Assuntos 
                ViewData["AssuntosPermissoes"] = CRUD.Listar(new MLAssuntosXPaginas { CodigoPagina = model.Codigo }, portal.ConnectionString);
                ViewData["Assuntos"] = CRUD.Listar(new MLAssuntos { IsAtivo = true, CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);

                ViewData["Modulos"] = BLModulo.Listar(PortalAtual.ConnectionString);
                ViewData["TemplatePagina"] = CRUD.Obter(new MLTemplate() { Nome = model?.PaginaEdicao.NomeTemplate });

                return View("Item", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(MLPaginaAdmin model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var portal = PortalAtual.Obter;
                    string listaCodigoGrupoCliente = Request.Form["listaCodigoGrupoCliente"];
                    string listaAssuntos = Request.Form["listaAssuntos"];
                    string urlLogin = Request.Form["UrlLogin"];

                    if (!model.Restrito.HasValue || !model.Restrito.Value) listaCodigoGrupoCliente = string.Empty;

                    var T = new BLTraducao(portal);
                    if (!model.CodigoPortal.HasValue) model.CodigoPortal = portal.Codigo;
                    if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                    model.LogDataAlteracao = DateTime.Now;
                    model.LogUsuarioAlteracao = BLUsuario.ObterLogado().Codigo;

                    // Salvar
                    using (var scope = new TransactionScope(portal.ConnectionString))
                    {
                        var codigo = CRUD.Salvar(model, portal.ConnectionString);

                        model.PaginaEdicao.Codigo = codigo;
                        model.PaginaEdicao.UsuarioEditor = BLUsuario.ObterLogado().Codigo;
                        model.PaginaEdicao.DataEdicao = DateTime.Now;

                        var existeEdicao = CRUD.Obter<MLPaginaEdicao>(model.Codigo.Value, portal.ConnectionString);

                        model.Seo.Codigo = codigo;

                        if (Request["SeoOutros"] != null)
                        {
                            model.Seo.Outros = Request["SeoOutros"];
                        }

                        var template = Request.Form["NomeTemplate"];
                        var layout = Request.Form["NomeLayout"];

                        if (!string.IsNullOrWhiteSpace(template))
                            model.PaginaEdicao.NomeTemplate = template;

                        if (!string.IsNullOrWhiteSpace(layout))
                            model.PaginaEdicao.NomeLayout = layout;

                        CRUD.Salvar(model.Seo, portal.ConnectionString);

                        if (existeEdicao.Codigo.HasValue)
                        {
                            var edicao = CRUD.CopiarValores(model.PaginaEdicao, new MLPaginaEdicaoSimples());
                            CRUD.Salvar(edicao, portal.ConnectionString);
                        }
                        else
                        {
                            var existeProducao = CRUD.Obter<MLPaginaPublicada>(model.Codigo.Value, portal.ConnectionString);

                            if (existeProducao.Codigo.HasValue)
                            {
                                model.PaginaEdicao.NomeLayout = existeProducao.NomeLayout;
                                model.PaginaEdicao.NomeTemplate = existeProducao.NomeTemplate;
                                model.PaginaEdicao.TemplateCustomizado = existeProducao.TemplateCustomizado;

                                CRUD.Salvar(model.PaginaEdicao, portal.ConnectionString);
                            }
                            else
                            {
                                return Json(new { success = false, msg = "Página não encontrada" });
                            }
                        }

                        //Assuntos
                        CRUD.Excluir(new MLAssuntosXPaginas { CodigoPagina = codigo });

                        if (!string.IsNullOrEmpty(listaAssuntos))
                        {
                            foreach (var item in listaAssuntos.Split(','))
                            {
                                var novoItem = new MLAssuntosXPaginas
                                {
                                    CodigoPagina = codigo,
                                    CodigoAssunto = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar(novoItem, portal.ConnectionString);
                            }
                        }


                        //Permissões Públicas
                        CRUD.Excluir<MLPaginaPermissao>("CodigoPagina", model.Codigo.Value, portal.ConnectionString);

                        if (!string.IsNullOrEmpty(listaCodigoGrupoCliente))
                        {
                            foreach (var item in listaCodigoGrupoCliente.Split(','))
                            {
                                var novoItem = new MLPaginaPermissao
                                {
                                    CodigoPagina = codigo,
                                    CodigoGrupo = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar(novoItem, portal.ConnectionString);
                            }
                        }

                        scope.Complete();
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

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ItemPagina(decimal? id, string TempTemplate, string TempLayout)
        {
            try
            {
                if (!id.HasValue) id = 0;

                var portal = PortalAtual.Obter;
                var model = new BLPagina(portal.ConnectionString).ObterPaginaAdmin(id.Value);

                #region Definir idioma
                //IdiomaAdmin = new CRUD.Select<MLIdioma>().Equals(a => a.Codigo, model.CodigoIdioma).First(portal.ConnectionString);
                //BLIdioma.Atual = CRUD.Select<MLIdioma>().Equals(a => a.Codigo, model.CodigoIdioma).Get(portal.ConnectionString);

                //Define cultura seguindo sigla de idioma da página
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(BLIdioma.Atual.Sigla);
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(IdiomaAdmin.Sigla);
                #endregion

                if (model.Codigo.HasValue && !BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal))
                {
                    return View("erro550");
                }

                ViewBag.TempTemplate = TempTemplate;
                ViewBag.TempLayout = TempLayout;

                return View("ItemPagina", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }


        #endregion

        #region ObterModuloRepositorio

        /// <summary>
        /// Obter o módulo que está inserido no repositório
        /// </summary>
        /// <param name="CodigoPagina"></param>
        /// <param name="Repositorio"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ObterModuloRepositorio(decimal CodigoPagina, int Repositorio)
        {
            return Json(CRUD.Obter(new MLPaginaModuloEdicao() { CodigoPagina = CodigoPagina, Repositorio = Repositorio }) ?? new MLPaginaModuloEdicao());
        }

        #endregion

        #region Editar

        /// <summary>
        /// Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpGet]
        [Compress]
        public ActionResult Editar(decimal id)
        {
            try
            {
                BLConteudo.CopiarPublicadaParaEdicao(id, false, PortalAtual.ConnectionString);

                return Item(id, null);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult Editar(decimal id, bool? recuperar)
        {
            try
            {
                BLConteudo.CopiarPublicadaParaEdicao(id, recuperar ?? false, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        /// <summary>
        /// Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult EditarHistorico(string id)
        {
            try
            {
                BLConteudo.CopiarHistoricoParaEdicao(Guid.Parse(id), PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Publicar

        /// <summary>
        /// Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Publicar(decimal id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                //var objConteudo = new BLConteudo(portal.Diretorio, portal.ConnectionString);
                //var model = objConteudo.Publicar(id);

                var paginaPublicar = new BLPaginaPublicar(portal.ConnectionString, portal.Diretorio);
                var model = paginaPublicar.Start(id);

                // Reset Cache
                BLCachePortal.ResetarCachePortal(portal.Codigo.Value);

                ViewData["Publicada"] = true;

                return Item(id, null);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Nova Pagina

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Nova(decimal? secao)
        {
            return View(new MLPaginaAdmin { CodigoSecao = secao, CodigoIdioma = PortalAtual.Obter.CodigoIdioma });
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult Nova(MLPaginaAdmin model, decimal? duplicar, bool? publicado)
        {
            try
            {
                var portal = PortalAtual.Obter;

                if (model.Codigo.HasValue) //realizar associação de página ao inves de criar um novo registro
                {
                    CRUD.SalvarParcial<MLPaginaAdmin>(new MLPaginaAdmin
                    {
                        Codigo = model.Codigo,
                        CodigoPai = model.CodigoPai
                    }, portal.ConnectionString);

                    return Redirect("/cms/" + portal.Diretorio + "/pagina/item/" + model.Codigo);
                }

                if (ModelState.IsValid)
                {
                    model.Excluida = model.Excluida.GetValueOrDefault(false);

                    // Salvar
                    var T = new BLTraducao(portal);
                    if (!model.CodigoPortal.HasValue) model.CodigoPortal = portal.Codigo;
                    if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                    model.LogPreencher(model.Codigo.HasValue);

                    using (var scope = new TransactionScope(portal.ConnectionString))
                    {
                        var codigo = CRUD.Salvar<MLPaginaAdmin>(model, portal.ConnectionString);

                        model.PaginaEdicao.Codigo = codigo;
                        model.PaginaEdicao.UsuarioEditor = BLUsuario.ObterLogado().Codigo;
                        model.PaginaEdicao.DataEdicao = DateTime.Now;
                        model.PaginaEdicao.ApresentarNaBusca = true;

                        model.LogDataCadastro = DateTime.Now;
                        model.LogUsuarioCadastro = BLUsuario.ObterLogado().Codigo;

                        CRUD.Salvar<MLPaginaEdicao>(model.PaginaEdicao, portal.ConnectionString);

                        if (duplicar.HasValue && publicado.HasValue)
                        {
                            // Modulos
                            var modulos = BLModulo.Listar(portal.ConnectionString);

                            if (publicado.Value == true)
                            {
                                var modulosPagina = CRUD.Listar<MLPaginaModuloPublicado>(new MLPaginaModuloPublicado { CodigoPagina = duplicar }, portal.ConnectionString);

                                foreach (var item in modulosPagina)
                                {
                                    CRUD.Salvar<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao
                                        {
                                            CodigoModulo = item.CodigoModulo,
                                            CodigoPagina = codigo,
                                            Repositorio = item.Repositorio
                                        }, portal.ConnectionString);

                                    System.Reflection.Assembly assembly;

                                    try
                                    {
                                        var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                                        if (moduloEncontrado == null
                                            || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                                            || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                                        assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                                        var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                                        var method = moduloType.GetMethod("DuplicarConteudo");
                                        method.Invoke(null, new object[] { duplicar, codigo, item.Repositorio, publicado });
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

                            }
                            else
                            {
                                var modulosPagina = CRUD.Listar<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = duplicar }, portal.ConnectionString);

                                foreach (var item in modulosPagina)
                                {
                                    CRUD.Salvar<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao
                                    {
                                        CodigoModulo = item.CodigoModulo,
                                        CodigoPagina = codigo,
                                        Repositorio = item.Repositorio
                                    }, portal.ConnectionString);

                                    System.Reflection.Assembly assembly;

                                    try
                                    {
                                        var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                                        if (moduloEncontrado == null
                                            || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                                            || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                                        assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                                        var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                                        var method = moduloType.GetMethod("DuplicarConteudo");
                                        method.Invoke(null, new object[] { duplicar, codigo, item.Repositorio, publicado });
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

                            }

                        }

                        scope.Complete();

                        return Redirect("/cms/" + portal.Diretorio + "/pagina/item/" + codigo);
                    }
                }

                return View("Nova", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region SeoOutros

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult SeoOutros(decimal codigopagina, string seooutros)
        {
            var portal = PortalAtual.Obter;

            try
            {
                CRUD.SalvarParcial<MLPaginaSeo>(new MLPaginaSeo
                {
                    Codigo = codigopagina,
                    Outros = seooutros
                }, portal.ConnectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            return new RedirectResult(string.Format("/cms/{0}/pagina/editar/{1}", portal.Diretorio, codigopagina));
        }

        #endregion

        // LIXEIRA

        #region Lixeira

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Index
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Lixeira()
        {
            return View();
        }

        #endregion

        #region ListarLixeira

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarLixeira(MLPagina criterios, int? page, int? limit, string sort, string buscaGenerica)
        {
            try
            {
                int start;
                int length;
                int.TryParse(Request.QueryString["start"], out start);
                int.TryParse(Request.QueryString["length"], out length);

                length = 5;

                // Busca lista no banco de dados

                var lista = new BLPagina(PortalAtual.ConnectionString).ListarAdmin(
                    buscaGenerica, PortalAtual.Codigo, null,
                    BLUsuario.ObterLogado().GruposToString(), false,
                    Request.QueryString["order[{0}][data]"], Request.QueryString["order[{0}][dir]"],
                    start, length, criterios.CodigoIdioma);

                // Retorna os resultados

                //var response = new ActionResult();
                //response.Data = lista;
                //response.Total = (int)total;

                //return response;
                return null;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Excluir(List<string> ids)
        {
            var portal = PortalAtual.Obter;
            var T = new BLTraducao(portal);
            if (!BLUsuario.ObterLogado().CheckPortal(portal.Codigo)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

            BLPagina.Excluir(ids);

            return Json(new { Sucesso = true });
        }

        /// <summary>
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult ExcluirPagina(decimal id)
        {
            var portal = PortalAtual.Obter;
            var T = new BLTraducao(PortalAtual.Obter);
            if (!BLUsuario.ObterLogado().CheckPortal(portal.Codigo)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

            BLPagina.Excluir(id);

            return Json(new { Sucesso = true });
        }

        #endregion

        #region Ativar

        /// <summary>
        /// Ativar registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Ativar(decimal id)
        {
            try
            {
                var model = CRUD.Obter<MLPagina>(id, PortalAtual.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                CRUD.SalvarParcial(new MLPagina
                {
                    Codigo = id,
                    Ativo = true,
                    LogDataAlteracao = DateTime.Now,
                    LogUsuarioAlteracao = BLUsuario.ObterLogado().Codigo
                }, PortalAtual.ConnectionString);

                return Item(id, null);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        // HISTORICO

        #region Historico

        // GET: /CMS/Pagina/
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Historico(decimal id)
        {
            return View(id);
        }

        // GET: /CMS/Pagina/
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult SalvarHistorico(string codigoHistorico)
        {
            try
            {
                BLConteudo.CopiarHistoricoParaEdicao(new Guid(codigoHistorico), PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Listar Historico

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult ListarHistorico(MLPaginaHistorico criterios)
        {
            try
            {

                var model = CRUD.Obter<MLPagina>(criterios.Codigo.Value, PortalAtual.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return null;

                return CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        // MODULOS

        #region NovoModulo

        /// <summary>
        /// Novo Modulo
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpGet]
        public ActionResult NovoModulo(decimal codigoPagina, int repositorio)
        {
            try
            {
                var model = CRUD.Obter<MLPagina>(codigoPagina, PortalAtual.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return null;

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;

                var modulos = BLModulo.Listar(PortalAtual.ConnectionString);

                return PartialView(modulos);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Novo Modulo
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult NovoModulo(decimal codigoPagina, int? repositorio, decimal codigoModulo)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var model = CRUD.Obter<MLPagina>(codigoPagina, portal.ConnectionString);
                var T = new BLTraducao(portal);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                CRUD.Salvar<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao
                {
                    CodigoPagina = codigoPagina,
                    Repositorio = repositorio,
                    CodigoModulo = codigoModulo
                }, portal.ConnectionString);

                var modulo = BLModulo.Obter(codigoModulo, portal.ConnectionString);

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;
                ViewData["codigoModulo"] = codigoModulo;
                ViewData["urlModulo"] = modulo.Url;
                ViewData["editavel"] = modulo.Editavel;
                ViewData["codigoLista"] = modulo.CodigoLista;

                ViewData["abrirEdicao"] = 1;

                return PartialView("~/Areas/CMS/Views/Pagina/Modulo.cshtml");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        /// <summary>
        /// Novo Modulo
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult Modulo(decimal codigoPagina, int? repositorio, string urlModulo)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var model = CRUD.Obter<MLPagina>(codigoPagina, portal.ConnectionString);
                var T = new BLTraducao(portal);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;
                ViewData["urlModulo"] = urlModulo;
                ViewData["editavel"] = true;

                return PartialView();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region RemoverModulo

        /// <summary>
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/RemoverModulo/id
        /// </remarks>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RemoverModulo(decimal codigoPagina, int repositorio)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLPagina>(codigoPagina, portal.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                using (var command = Database.NewCommand("USP_CMS_D_PAGINA_MODULO", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, codigoPagina);
                    command.NewCriteriaParameter("@REP_N_NUMERO", SqlDbType.Int, repositorio);

                    // Execucao
                    Database.ExecuteNonQuery(command);
                }

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;

                var modulos = BLModulo.Listar(portal.ConnectionString);

                return PartialView("~/Areas/CMS/Views/Pagina/NovoModulo.cshtml", modulos);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region MoverModulo

        /// <summary>
        /// Mover registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/MoverModulo/id
        /// </remarks>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult MoverModulo(decimal codigoPagina, int repositorioOrigem, int repositorioDestino)
        {
            try
            {
                var model = CRUD.Obter<MLPagina>(codigoPagina, PortalAtual.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                var moduloOrigem = CRUD.Obter<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorioOrigem }, PortalAtual.ConnectionString);
                var moduloEncontrado = BLModulo.Obter(moduloOrigem.CodigoModulo, PortalAtual.ConnectionString);

                var moduloDestino = CRUD.Obter<MLPaginaModuloEdicao>(new MLPaginaModuloEdicao { CodigoPagina = codigoPagina, Repositorio = repositorioDestino }, PortalAtual.ConnectionString);
                var moduloEncontradoDestino = moduloEncontrado;
                var isModuloIgual = true;
                var isContainerPreechido = false;

                //Verifica se existe um modulo no destino e se o modulo não é do mesmo tipo
                if (moduloDestino != null && moduloDestino.CodigoModulo.HasValue)
                {
                    isContainerPreechido = true;
                    if (moduloDestino.CodigoModulo != moduloOrigem.CodigoModulo)
                    {
                        moduloEncontradoDestino = BLModulo.Obter(moduloDestino.CodigoModulo, PortalAtual.ConnectionString);
                        isModuloIgual = false;
                    }
                }

                using (var command = Database.NewCommand("USP_CMS_U_PAGINA_MODULO_MOVER", PortalAtual.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, codigoPagina);
                    command.NewCriteriaParameter("@REP_N_NUMERO_ORIGEM", SqlDbType.Int, repositorioOrigem);
                    command.NewCriteriaParameter("@REP_N_NUMERO_DESTINO", SqlDbType.Int, repositorioDestino);

                    // Execucao
                    Database.ExecuteNonQuery(command);
                }

                System.Reflection.Assembly assembly;
                try
                {
                    assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                    var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                    var method = moduloType.GetMethod("Mover");
                    method.Invoke(null, new object[] { codigoPagina, repositorioOrigem, repositorioDestino, isContainerPreechido, isModuloIgual, PortalAtual.ConnectionString });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }
                finally
                {
                    assembly = null;
                }

                if (isContainerPreechido && !isModuloIgual)
                {
                    System.Reflection.Assembly assemblyDestino;
                    try
                    {
                        assemblyDestino = System.Reflection.Assembly.Load(moduloEncontradoDestino.NomeAssembly);
                        var moduloType = assemblyDestino.GetType(moduloEncontradoDestino.NomeBusinesLayer);

                        var method = moduloType.GetMethod("Mover");
                        method.Invoke(null, new object[] { codigoPagina, repositorioDestino, repositorioOrigem, false, false, PortalAtual.ConnectionString });
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


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Toolbar

        /// <summary>
        /// Toolbar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Toolbar(string urlModulo, decimal codigoPagina, int repositorio, bool editavel)
        {
            try
            {
                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;
                ViewData["urlModulo"] = urlModulo;
                ViewData["editavel"] = editavel;

                return PartialView("~/areas/modulo/views/shared/toolbar.cshtml");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Acoes

        /// <summary>
        /// Toolbar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Acoes()
        {
            try
            {

                return PartialView();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        // SCREENSHOT

        #region ScreenShot

        /// <summary>
        /// ScreenShot
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ScreenShot(string file, decimal pagina)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(file.Replace("data:", "").Replace(",", "")))
                {
                    return null;
                }

                var diretorio = BLConfiguracao.Pastas.ImagensPortal(PortalAtual.Diretorio) + "/screenshots/";
                var pasta = Server.MapPath(diretorio);

                if (!Directory.Exists(pasta))
                    Directory.CreateDirectory(pasta);

                var fileNameWitPath = Path.Combine(pasta, pagina + ".png");
                var baseImg = System.Text.RegularExpressions.Regex.Match(file, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var baseBytes = Convert.FromBase64String(baseImg);
                var webImage = new System.Web.Helpers.WebImage(baseBytes);

                webImage.Resize(400, 600, true);
                webImage.Save(fileNameWitPath);
                BLReplicar.Arquivo(fileNameWitPath);

                return null;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        public ActionResult ImagemMiniatura(string idPagina)
        {
            if (!String.IsNullOrEmpty(idPagina))
            {
                var urlPath = Server.MapPath(String.Format("{0}/screenshots/{1}.png", BLConfiguracao.Pastas.ImagensPortal(PortalAtual.Diretorio), idPagina));
                if (System.IO.File.Exists(urlPath))
                {
                    return File(urlPath, "image/png");
                }
            }

            return File("/content/img/image-not-found.png", "image/png");
        }

        // OUTROS

        #region ListarPaginaspublicadas

        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarPaginaspublicadas(string busca, decimal CodigoPortal, decimal? param1)
        {
            var retorno = BLPagina.ListarPaginasPublicadas(busca, CodigoPortal, param1, BLUsuario.ObterLogado().GruposToString(), PortalAtual.ConnectionString, 15);
            return Json(new { success = true, json = retorno });
        }

        #endregion

        /// <summary>
        /// Validar se a Url já esta cadastrada no sistema
        /// </summary>
        /// <param name="url">Url do Evento</param>
        /// <returns>Json</returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public JsonResult ValidarUrl(string url, decimal? id)
        {
            bool urlValida = false;

            try
            {
                //VALIDAR URL DE PAGINAS
                var portal = PortalAtual.Obter;
                var listaUrl = CRUD.Listar<MLPagina>(new MLPagina
                {
                    Url = url,
                    Excluida = false,
                    CodigoPortal = portal.Codigo
                }, portal.ConnectionString);

                if (listaUrl != null && listaUrl.Count > 0)
                {
                    var modelComUrlIgual = listaUrl.Find(a => a.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));

                    if (modelComUrlIgual != null)
                    {
                        if (id.HasValue)
                            urlValida = listaUrl.Find(a => a.Codigo == id) != null;
                        else
                            urlValida = false;
                    }
                    else
                    {
                        urlValida = true;
                    }
                }
                else
                    urlValida = true;

                //VALIDAR URL REGISTRADA NO GLOBAL.ASAX
                if (urlValida)
                {
                    var listaUrlsRota = (from e in RouteTable.Routes.OfType<Route>()
                                         where e.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase)
                                         select e.Url).ToList();

                    urlValida = listaUrlsRota.Count == 0;
                }

                return Json(urlValida, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public JsonResult DefinirHome(decimal? portal, string url)
        {
            try
            {
                var portalAtual = PortalAtual.Obter;

                CRUD.SalvarParcial<MLPortalPublico>(new MLPortalPublico
                {
                    Codigo = portal,
                    UrlHome = url
                }, portalAtual.ConnectionString);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = "Não foi possível definir esta página como a principal." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region GerarSitemap

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult GerarSitemap()
        {
            try
            {
                var arquivo = string.Format("/Portal/{0}/{1}", BLPortal.Atual.Diretorio, "sitemap.xml");
                var xml = BLSitemap.Paginas(PortalAtual.Obter.Codigo.Value);

                using (var novoArquivo = new StreamWriter(Server.MapPath(arquivo)))
                {
                    novoArquivo.Write(xml);
                    novoArquivo.Close();
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "text/xml";

                Response.BufferOutput = true;
                Response.ContentEncoding = System.Text.ASCIIEncoding.GetEncoding("ISO-8859-1");

                Response.AddHeader("content-disposition", "attachment; filename=" + "paginas-" + PortalAtual.Obter.Diretorio + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".xml");
                Response.AddHeader("pragma", "public");

                Response.Write(xml);

                Response.End();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw ex;
            }

            return Json(new { success = true });
        }

        #endregion
    }
}
