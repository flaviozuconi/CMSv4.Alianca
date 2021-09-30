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
    public class ArquivosController : ModuloBaseController<MLModuloArquivosEdicao, MLModuloArquivosHistorico, MLModuloArquivosPublicado>
    {
        #region Index
        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloArquivosPublicado>(new MLModuloArquivosPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

                if (model == null || (!model.CodigoPagina.HasValue))
                {
                    model = new MLModuloArquivosPublicado()
                    {
                        CodigoPagina = codigoPagina,
                        Repositorio = repositorio
                    };
                }

                ViewBag.Action = string.IsNullOrEmpty(model.NomeView) ? "lista" : model.NomeView;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
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
                var model = new MLModuloArquivos();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloArquivosEdicao>(new MLModuloArquivosEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloArquivos { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloArquivosHistorico>(new MLModuloArquivosHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloArquivosPublicado>(new MLModuloArquivosPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloArquivos();

                //if (!string.IsNullOrEmpty(model.Conteudo))
                //    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);

                ViewData["editavel"] = false;

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
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
                var model = CRUD.Obter<MLModuloArquivosEdicao>(new MLModuloArquivosEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloArquivosEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                if (!string.IsNullOrEmpty(model.Categorias))
                {
                    var cats = model.Categorias.Split(',');
                    foreach (var cat in cats)
                        model.CodigosCategoriaArquivo.Add(decimal.Parse(cat));
                }

                ///TIPO DE VIEWS DISPONÍVEIS
                var lstViews = new List<string>() { "detalhe", "listagem", "listagemano", "destaque" };
                IEnumerable<SelectListItem> views = lstViews.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                });

                ViewBag.Views = views;

                ///PASTAS DISPONÍVEIS
                var lstPastas = CRUD.Listar(new MLArquivoCategoria { CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);
                IEnumerable<SelectListItem> pastas = lstPastas.Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });

                ViewBag.Pastas = pastas;

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
        public override ActionResult Editar(MLModuloArquivosEdicao model)
        {
            try
            {
                model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;

                foreach (decimal cat in model.CodigosCategoriaArquivo)
                    model.Categorias += cat.ToString() + ",";

                model.Categorias = model.Categorias.Substring(0, (model.Categorias.Length - 1));

                CRUD.Salvar(model, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
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

                CRUD.Excluir<MLModuloArquivosEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ListagemAjax

        [HttpPost]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListagemAjax(int? Pagina, int? Quantidade, string url, string categorias, bool ordenardata, bool ordenardesc)
        {
            var lista = Listar(Quantidade, Pagina.Value, categorias, ordenardata, ordenardesc);
            ViewBag.UrlDetalhe = url;
            int TotalRows = 1;

            if (lista.Count > 0) TotalRows = lista[0].TotalRows.Value;

            var stringRetorno = BLConteudoHelper.RenderViewToString(this, "ItemListagem", String.Empty, lista);
            double totalPaginas = Math.Ceiling((double)(TotalRows) / (Quantidade.Value));

            return Json(new { view = stringRetorno, TotalPaginas = totalPaginas });
        }

        #endregion

        #region ListagemAno
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListagemAno(MLModuloArquivos model)
        {
            return View(model);
        }
        #endregion

        #region ListagemAnoAjaxRequest
        [HttpGet, CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListagemAnoAjaxRequest(string categorias, string url, int ano)
        {
            var lista = BLArquivos.ListarAno(categorias, ano);

            TempData["UrlDetalhe"] = url;

            return Json(new { view = BLConteudoHelper.RenderViewToString(this, "ListagemAnoAjax", String.Empty, lista) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloArquivos model, int totalRows)
        {
            try
            {
                ViewBag.TotalRows = totalRows;
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
        public ActionResult ScriptAno(MLModuloArquivos model)
        {
            return PartialView(model);
        }

        #endregion

        /////ÁREA PÚBLICA

        #region Destaque
        /// <summary>
        /// Lista os arquivos Destaque
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Destaque(MLModuloArquivos model)
        {

            try
            {
                var lstML = BLArquivos.ListarDestaque(model, true);
                ViewData["lstArquivos"] = lstML;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Listagem
        /// <summary>
        /// Lista os arquivos
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Listagem(MLModuloArquivos model)
        {

            try
            {
                var lstML = Listar(model.Quantidade, 1, model.Categorias, model.OrdenarData.GetValueOrDefault(), model.OrdenarDesc.GetValueOrDefault());

                ViewData["lstArquivos"] = lstML;

                if (lstML.Count > 0)
                    ViewBag.TotalRows = lstML[0].TotalRows ?? 0;
                else
                    ViewBag.TotalRows = 0;

                ViewBag.TotalPaginas = Math.Ceiling((double)(ViewBag.TotalRows ?? 1) / (model.Quantidade.HasValue ? model.Quantidade.Value : 1));

                return View("Listagem", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        [CheckPermission(global::Permissao.Publico)]
        public List<MLArquivoPublico> Listar(int? quantidade, int pagina, string categorias, bool ordenardata, bool ordenardesc)
        {
            MLModuloArquivosEdicao model = new MLModuloArquivosEdicao();

            if (Convert.ToString(RouteData.Values["extra1"]) != string.Empty && int.TryParse(RouteData.Values["extra1"].ToString(), out pagina))
                model.Pagina = pagina;
            else
                model.Pagina = pagina;

            model.Quantidade = quantidade ?? 10;
            model.Categorias = categorias;
            model.OrdenarData = ordenardata;
            model.OrdenarDesc = ordenardesc;
            ViewBag.PaginaAtual = model.Pagina;

            return BLArquivos.ListarPublico(model, !(model is MLModuloArquivosEdicao));
        }

        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ItemListagem(List<MLArquivoPublico> model, string url, int repositorio)
        {
            ViewBag.UrlDetalhe = url;
            ViewBag.Repositorio = repositorio;

            return View("ItemListagem", model);
        }

        //#region DownloadFile
        ///// <summary>
        ///// DownloadFile
        ///// </summary>
        //[CheckPermission(global::Permissao.Publico)]
        //public FileResult DownloadFile(string Pasta, string NomeArquivo)
        //{
        //    try
        //    {
        //        Pasta = System.Text.RegularExpressions.Regex.Replace((Pasta ?? string.Empty), "[^a-zA-Z0-9_.]+", string.Empty, System.Text.RegularExpressions.RegexOptions.Compiled);
        //        var file = Server.MapPath("~/Portal/" + BLPortal.Atual.Diretorio + "/arquivos/" + Pasta + "/" + NomeArquivo);

        //        return File(file, "application/octet-stream", NomeArquivo.Replace(".resource", ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLog.ErrorLog(ex);
        //    }
        //    return null;
        //}
        //#endregion

        #region DownloadFile
        /// <summary>
        /// DownloadFile
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public FileResult DownloadFile(decimal codigoarquivo)
        {
            try
            {
                var portal = BLPortal.Atual;
                var arquivo = CRUD.Obter<MLArquivo>(codigoarquivo, portal.ConnectionString);

                if (arquivo == null || !arquivo.CodigoCategoria.HasValue)
                {
                    ApplicationLog.ErrorLog(new Exception(string.Format("O arquivo {0} ocasionou o erro", codigoarquivo)));
                }
                else
                {
                    var pasta = CRUD.Obter<MLArquivoCategoria>(arquivo.CodigoCategoria.Value, portal.ConnectionString);
                    //Pasta = System.Text.RegularExpressions.Regex.Replace((Pasta ?? string.Empty), "[^a-zA-Z0-9_.]+", string.Empty, System.Text.RegularExpressions.RegexOptions.Compiled);
                    if (pasta != null)
                    {
                        var file = Server.MapPath(string.Concat("~/Portal/", portal.Diretorio, "/arquivos/", pasta.Nome.Replace("/", ""), "/", arquivo.Nome));
                        if (System.IO.File.Exists(file))
                        {
                            return File(file, "application/octet-stream", arquivo.Nome.Replace(".resource", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return null;
        }


        //Utilizado para fazer downloads pelo nome do arquivo, na url colocar /arquivos/{nome da pasta}?file={Nome do arquivo mais extensão}
        [CheckPermission(global::Permissao.Publico)]
        public FileResult DownloadFileNome(string nomepasta)
        {
            try
            {
                var portal = BLPortal.Atual;

                string Filename = Request.QueryString["file"];

                if (!string.IsNullOrWhiteSpace(Filename))
                {
                    var file = Server.MapPath(string.Concat("~/Portal/", "idesa", "/arquivos/", nomepasta, "/", Filename));

                    if (System.IO.File.Exists(file))
                    {
                        return File(file, "application/octet-stream", Filename);
                    }
                    else
                    {
                        ApplicationLog.ErrorLog(new Exception(string.Format("O arquivo {0} nao foi encontrado!", Filename)));
                    }
                }
                else
                {
                    ApplicationLog.ErrorLog(new Exception(string.Format("O arquivo {0} ocasionou o erro", Filename)));
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return null;
        }


        #endregion
    }
}