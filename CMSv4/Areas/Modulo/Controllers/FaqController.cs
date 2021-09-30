using Framework.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class FaqController : ModuloBaseController<MLModuloFaqEdicao, MLModuloFaqHistorico, MLModuloFaqPublicado>
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

                // Visualizar Publicado
                var portal = BLPortal.Atual;
                var model = CRUD.Obter(new MLModuloFaqPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloFaqPublicado();

                var categorias = CRUD.Listar(new MLFaqCategoria { Ativo = true, CodigoPortal = portal.Codigo.Value }).Where(w => !String.IsNullOrEmpty(model.Categorias) && model.Categorias.Split(',').Contains(w.Codigo.ToString())).ToList();
                var faq = CRUD.Listar(new MLFaq { Ativo = true, CodigoPortal = portal.Codigo  }, portal.ConnectionString);

                if (categorias != null && categorias.Count > 0)
                    categorias.OrderBy(f => f.Titulo);


                ViewBag.Categorias = categorias;
                ViewBag.Faqs = faq;

                return PartialView("Index", model);
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
                MLModuloFaqEdicao model = null;

                ViewBag.ModoEdicao = false;

                if (edicao.HasValue && edicao.Value)
                {
                    ViewBag.ModoEdicao = true;

                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloFaqEdicao>(new MLModuloFaqEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    model = CRUD.Obter<MLModuloFaqHistorico>(new MLModuloFaqHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloFaqPublicado>(new MLModuloFaqPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null)
                    model = new MLModuloFaqEdicao();

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
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloFaqEdicao>(new MLModuloFaqEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloFaqEdicao();

                ViewBag.Categorias = CRUD.Listar<MLFaqCategoria>(new MLFaqCategoria { Ativo = true, CodigoPortal = portal.Codigo }, portal.ConnectionString);

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
        public override ActionResult Editar(MLModuloFaqEdicao model)
        {
            try
            {
                if (Request.Form["Categorias"] != null)
                {
                    //O bind da model retorna apenas 1 item selecionado invés de todos os selecionados, isso porque entende que Categorias seja um array de string
                    //Define diretamente o valor real de Categorias
                    model.Categorias = Request.Form["Categorias"].Replace("multiselect-all,", "");
                }

                if (model.DataRegistro == null)
                    model.DataRegistro = DateTime.Now;

                if (model.CodigoUsuario == null || model.CodigoUsuario == 0)
                    model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;

                //Salvar
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                return Json(new { success = true, CodigoPagina = model.CodigoPagina, Repositorio = model.Repositorio, Categorias = model.Categorias, modulo = "faq", portaldiretorio = BLPortal.Atual.Diretorio });
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

                CRUD.Excluir<MLModuloFaqEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        public ActionResult Script(decimal? CodigoPagina, int? Repositorio, string Categorias)
        {
            ViewBag.codigoElementos = String.Concat(CodigoPagina, Repositorio);
            ViewBag.Categorias = Categorias;

            return PartialView();
        }

        #region Requisição Json

        public JsonResult ListarCategorias(String categorias)
        {
            return Json((from e in CRUD.Listar<MLFaqCategoria>(new MLFaqCategoria { Ativo = true }, PortalAtual.ConnectionString)
                         where !String.IsNullOrEmpty(categorias) && categorias.Split(',').Contains(e.Codigo.ToString())
                         orderby e.Titulo
                         select new { Id = e.Codigo.GetValueOrDefault().ToString(), Text = e.Titulo }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarFAQs(decimal codigoCategoria)
        {
            return Json((from e in CRUD.Listar<MLFaq>(new MLFaq { CodigoCategoria = codigoCategoria, Ativo = true }, PortalAtual.ConnectionString)
                         orderby e.Pergunta
                         select e), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}