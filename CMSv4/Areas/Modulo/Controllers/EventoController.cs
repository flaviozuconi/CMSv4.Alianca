using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class EventoController : ModuloBaseController<MLModuloEventosEdicao, MLModuloEventosHistorico, MLModuloEventosPublicado>
    {
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
                var model = new MLModuloEventos();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloEventosEdicao>(new MLModuloEventosEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloEventos { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloEventosHistorico>(new MLModuloEventosHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloEventosPublicado>(new MLModuloEventosPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null || (model != null && !model.DataRegistro.HasValue))
                {
                    var usuario = BLUsuario.ObterLogado();

                    model = new MLModuloEventosEdicao();
                    model.NomeView = "Listagem";
                    model.Quantidade = 5;
                    model.Titulo = T("Eventos");
                    model.Repositorio = repositorio;
                    model.CodigoPagina = codigoPagina;
                    model.DataRegistro = DateTime.Now;

                    if (usuario != null)
                    {
                        model.CodigoUsuario = usuario.Codigo;
                    }

                    CRUD.Salvar(model as MLModuloEventosEdicao, portal.ConnectionString);
                }

                return PartialView("index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloEventos model)
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

                /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptCalendario(MLModuloEventos model)
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

        #region Calendario
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Calendario(MLModuloEventos model)
        {
            return View(model);
        }
        #endregion

        #region CalendarioAjax
        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult CalendarioAjaxRequest(int dia, int mes, int ano, string url,int idioma)
        {
            TempData["UrlLista"] = url;
            var modelidioma = CRUD.Obter<MLIdioma>(idioma);
            if(modelidioma != null && !string.IsNullOrEmpty(modelidioma.Sigla))
                TempData["mensagem"] = new BLTraducao(BLPortal.Atual).Obter(modelidioma.Sigla, "Nenhum registro encontrado");
            
            var lista = BLEventos.ListarDiaPublico(new DateTime(ano, mes, dia), idioma);
            var stringRetorno = BLConteudoHelper.RenderViewToString(this, "CalendarioAjax", String.Empty, lista);

            return Json(new { view = stringRetorno, TotalPaginas = 0 }, JsonRequestBehavior.AllowGet);
        }
        #endregion 

        #region ScriptItemListagem

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptItemListagem(List<MLEvento> model, int repositorio)
        {
            try
            {
                ViewBag.Repositorio = repositorio;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        //publico

        #region Destaque

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Destaque(MLModuloEventos model)
        {
            ViewData["lstEventos"] = BLEventos.ListarDestaque(model.Quantidade ?? 3, !(model is MLModuloEventosEdicao));

            return View("Destaque", model);
        }

        #endregion

        #region Destaque Rotator

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult DestaqueRotator(MLModuloEventos model)
        {
            var lista = BLEventos.ListarDestaque(model.Quantidade ?? 3, !(model is MLModuloEventosEdicao));
            if (lista.Count > 0) lista[0].Class = "active";
            ViewData["lstEventos"] = lista;
            return View("DestaqueRotator", model);
        }

        #endregion

        #region Listagem

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Listagem(MLModuloEventos model)
        {
            if (!model.Quantidade.HasValue)
            {
                model.Quantidade = 5;
            }

            var lstML = Listar(model.Quantidade, 1, Convert.ToInt32(BLIdioma.Atual.Codigo), false, true);
            double totalRows = 1;

            ViewData["lstEventos"] = lstML;

            if (lstML.Count > 0 && lstML[0].TotalRows.HasValue)
                totalRows = lstML[0].TotalRows.Value;

            ViewBag.TotalPaginas = Math.Ceiling(totalRows / model.Quantidade.Value);
            ViewBag.TotalRows = totalRows;

            ViewBag.EventosRealizados = Listar(6, 1,Convert.ToInt32(BLIdioma.Atual.Codigo), true, true);

            /*
            if (!model.TipoPaginacao.Equals("Abas", StringComparison.InvariantCultureIgnoreCase))
            {
                var lstML = Listar(model.Quantidade, 1, false);
                double totalRows = 1;

                ViewData["lstEventos"] = lstML;

                if (lstML.Count > 0 && lstML[0].TotalRows.HasValue)
                    totalRows = lstML[0].TotalRows.Value;

                ViewBag.TotalPaginas = Math.Ceiling(totalRows / model.Quantidade.Value);
                ViewBag.TotalRows = totalRows;
            }
            */

            return View("Listagem", model);
        }

        #endregion

        #region Listagem Com Filtro

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListagemComFiltro(MLModuloEventos model)
        {
            var portal = BLPortal.Atual;
            int? ano = null;

            var lstML = CRUD.Listar<MLEventoPublico>(new MLEventoPublico { CodigoIdioma = BLIdioma.Atual.Codigo, CodigoPortal = portal.Codigo, Ativo = true }, portal.ConnectionString).OrderBy( x => x.DataInicio).ToList();

            if (lstML.Count > 0)
            {
                ano = lstML.OrderByDescending(x => x.DataInicio.Value.Year).Select(x => x.DataInicio.Value.Year).Distinct().First();

                ViewData["lstEventos"] = lstML.Where(x => x.DataInicio.Value.Year == ano).ToList();
            }            
            else
            {
                ViewData["lstEventos"] = lstML;
            }
           
            ViewData["lstAnos"] = lstML;
           
            return View(model);
        }

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListagemComFiltro(int? pintAno, int? pintSemestre)
        {
            try
            {
                var lista = BLEventos.ListarPublicoComFiltro(pintAno, pintSemestre);         

                var strHtmlOutput = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "Evento", "ItemListagemComFiltro", lista);
                decimal totalPaginas = 0;               

                return Json(new { success = true, html = strHtmlOutput, total = totalPaginas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ItemListagemComFiltro(List<MLEventoPublico> model, string url)
        {
            ViewBag.url = url;
            return View(model);
        }

        [HttpGet, CheckPermissionAttribute(global::Permissao.Publico)]
        public JsonResult ajaxPesquisar(int? Ano, int? Semestre, string url)
        {
            ViewBag.url = url;
            var lstML = ListarComFiltro(Ano, Semestre);
            string strHtml = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "Evento", "ItemListagemComFiltro", lstML);
            return Json(new { success = true, html = strHtml }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Detalhe

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Detalhe(MLModuloEventos model)
        {
            try
            {
                string urlDetalhe = string.Empty;
                string connString = BLPortal.Atual.ConnectionString;

                if (model == null) model = new MLModuloEventos();

                if (RouteData.Values["extra1"] == null)
                    return Content("Url do evento não informada.");

                urlDetalhe = Convert.ToString(RouteData.Values["extra1"]);

                var conteudo = new CRUD
                                .Select<MLEvento>()
                                    .Equals(a => a.Url, urlDetalhe)
                                    .Equals(a => a.Ativo, true)
                                .First(connString);

                var seo = new CRUD
                            .Select<MLEventoSEO>()
                                .Equals(a => a.Codigo, conteudo.Codigo)
                            .First(connString);

                if (conteudo != null && !string.IsNullOrEmpty(conteudo.Conteudo))
                    conteudo.Conteudo = Microsoft.JScript.GlobalObject.unescape(conteudo.Conteudo);

                //Criar Seo para a página baseado no detalhe
                if (seo != null && seo.Codigo > 0)
                    BLConteudo.AdicionarSeo(seo);

                ViewData["modulo"] = model;

                return View(conteudo);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region ItemListagem
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ItemListagem(List<MLEventoPublico> model, string url)
        {
            ViewBag.UrlDetalhe = url;

            return View(model);
        }
        #endregion

        #region Listar
        public List<MLEventoPublico> Listar(int? quantidade, int pagina, int idioma, bool eventosPassados = false, bool usarPaginacao = true)
        {
            MLModuloEventos model = new MLModuloEventos();

            if (RouteData.Values["extra1"] != null)
                int.TryParse(RouteData.Values["extra1"].ToString(), out pagina);

            model.Pagina = pagina;
            model.Quantidade = quantidade ?? 10;

            ViewBag.PaginaAtual = model.Pagina;

            return BLEventos.ListarPublico(model,idioma, eventosPassados: eventosPassados, usarPaginacao: usarPaginacao);
        }

        public List<MLEventoPublico> ListarComFiltro(int? ano, int? semestre)
        {
            return BLEventos.ListarPublicoComFiltro(ano, semestre);
        }
        #endregion

        #region Agenda

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public void Agenda(decimal id)
        {
            /*
            int lembrete = 0;
            int.TryParse(Request.QueryString["lembrete"], out lembrete);
            */
            var evento = CRUD.Obter<MLEventoPublico>(id);
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("BEGIN:vCalendar");
            sb.AppendLine("PRODID:-//Microsoft Corporation//Outlook 11.0 MIMEDIR//EN");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine(string.Concat("DTSTART: ", evento.DataInicio.GetValueOrDefault().ToString("yyyyMMdd'T'HHmmss'Z'")));
            sb.AppendLine(string.Concat("DTEND: ", evento.DataInicio.GetValueOrDefault().AddDays(1).ToString("yyyyMMdd'T'HHmmss'Z'")));
            sb.AppendLine(string.Concat("LOCATION: ", evento.Local, " - ", evento.EnderecoCompleto));
            sb.AppendLine(string.Concat("DESCRIPTION: ", evento.Chamada));
            sb.AppendLine(string.Concat("SUMMARY: ", evento.Titulo));
            sb.AppendLine("PRIORITY: 3");
            /*
            if (lembrete > 0)
            {
                sb.AppendLine("BEGIN:VALARM");
                sb.AppendLine(string.Concat("TRIGGER: -PT", lembrete, "D"));
                sb.AppendLine("ACTION: DISPLAY");
                sb.AppendLine(string.Format("DESCRIPTION: ", evento.Chamada));
                sb.AppendLine("END:VALARM");
            }
            */
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:vCalendar");

            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", string.Format("attachment; filename=Evento {0}.vcs", evento.Codigo));
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
        #endregion

        #region Json

        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public JsonResult ListarDiasEventoMes(int mes, int ano, int idioma)
        {
            var listaEventos = BLEventos.ListarDiasCalendario(mes, ano, idioma);
            var listaDatasMes = new List<int>();

            for (int i = 0; i < listaEventos.Count; i++)
            {
                if (listaEventos[i].DataTermino.HasValue)
                {
                    for (DateTime dataEntre = listaEventos[i].DataInicio.Value; dataEntre <= listaEventos[i].DataTermino.Value; dataEntre = dataEntre.AddDays(1))
                    {
                        if (dataEntre.Month == mes && dataEntre.Year == ano && !listaDatasMes.Contains(dataEntre.Day))
                            listaDatasMes.Add(dataEntre.Day);
                    }
                }
                else
                {
                    DateTime dataInicio = listaEventos[i].DataInicio.Value;

                    if (dataInicio.Month == mes && dataInicio.Year == ano && !listaDatasMes.Contains(dataInicio.Day))
                        listaDatasMes.Add(dataInicio.Day);
                }
            }
            
            return Json(from e in listaDatasMes select new DateTime(ano, mes, e).ToString("dd-MM-yyyy"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public JsonResult ListarDestaques()
        {
            var portal = BLPortal.Atual;
            var listaEventosDestaque = new CRUD
                .Select<MLEvento>(a => a.DataInicio, a => a.Codigo, a => a.Titulo, a => a.Url, a => a.Imagem)
                    .GreaterThan(a => a.DataInicio, DateTime.Today)
                    .Equals(a => a.Ativo, true)
                    .Equals(a => a.CodigoPortal, portal.Codigo.GetValueOrDefault(1))
                    .Equals(a => a.Destaque, true)
                    .Top(10)
                    .ToList(portal.ConnectionString);

            return Json(listaEventosDestaque, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListagemAjax(int? Pagina, int? Quantidade, string url, int idioma)
        {
            var lista = Listar(Quantidade, Pagina.GetValueOrDefault(),idioma, false, true);
            int TotalRows = 1;

            //ViewBag.UrlDetalhe = url;
            TempData["Pagina"] = Pagina;

            if (lista.Count > 0) TotalRows = lista[0].TotalRows.Value;

            var stringRetorno = BLConteudoHelper.RenderViewToString(this, "ItemListagem", String.Empty, lista);
            double totalPaginas = Math.Ceiling((double)(TotalRows) / (Quantidade.Value));

            return Json(new { view = stringRetorno, TotalPaginas = totalPaginas }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListarMeses(int mes, int ano, bool? proximosMeses)
        {
            List<DateTime> meses = new List<DateTime>();

            int top = 6;
            var dataSelecionada = new DateTime(ano, mes, 1);

            if (proximosMeses.HasValue)
            {
                if (proximosMeses.Value)
                    meses.Add(dataSelecionada.AddMonths(1));
                else
                    meses.Add(dataSelecionada.AddMonths(-1));
            }
            else
            {
                for (int i = 0; i < top; i++)
                    meses.Add(dataSelecionada.AddMonths(i));
            }

            return Json(from e in meses 
                        select new {
                            mes_texto = string.Concat(e.ToString("MMMM").Substring(0, 1).ToUpper(), e.ToString("MMMM").Substring(1)),
                            mes = e.ToString("MM"),
                            ano = e.ToString("yyyy")
                        }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}

