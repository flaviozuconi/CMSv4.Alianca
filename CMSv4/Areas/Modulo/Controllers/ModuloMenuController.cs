using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using System.IO;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ModuloMenuController : ModuloBaseController<MLModuloMenuEdicao, MLModuloMenuHistoricoModulo, MLModuloMenuPublicado>
    {
        //
        // GET: /Modulo/Menu/

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

                MLModuloMenuModulo model = CRUD.Obter<MLModuloMenuPublicado>(new MLModuloMenuPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

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
                var model = new MLModuloMenuModulo();
                

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloMenuEdicao>(new MLModuloMenuEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloMenuModulo { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloMenuHistoricoModulo>(new MLModuloMenuHistoricoModulo { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloMenuPublicado>(new MLModuloMenuPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloMenuPublicado();

                if (!model.CodigoMenu.HasValue)
                {
                    var menu = new  BLMenuModulo().ObterDefault(BLPortal.Atual.ConnectionString);

                    //Configuração Default
                    CRUD.Salvar
                        (
                            new MLModuloMenuEdicao()
                            {
                                CodigoPagina = codigoPagina,
                                CodigoMenu = menu.Codigo,
                                Repositorio = model.Repositorio,
                                View = "Horizontal",
                                DataRegistro = DateTime.Now
                            },
                            BLPortal.Atual.ConnectionString
                        );

                    model.CodigoMenu = menu.Codigo;
                }

                ViewData["controller"] = "modulomenu";
                ViewData["action"] = "Index";

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

                var model = CRUD.Obter<MLModuloMenuEdicao>(new MLModuloMenuEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloMenuEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                ViewData["menus"] = CRUD.Listar<MLMenuModulo>(new MLMenuModulo() { Ativo = true, CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);                

                return View("Editar", model);
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
        public override ActionResult Editar(MLModuloMenuEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                return Json(new { success = true, repositorio = model.Repositorio });
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

                CRUD.Excluir<MLModuloBannerEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloBanner model)
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

        #region Lista Publico
        /// <summary>
        /// Action da lista
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListarPublico(MLModuloMenuModulo model)
        {
            try
            {
                MLMenuCompletoModulo objML = new MLMenuCompletoModulo();

                if (model == null)
                    model = new MLModuloMenuModulo();

                if (model.CodigoMenu.HasValue && model.CodigoMenu.Value > 0)
                    objML = new BLMenuModulo().ObterCompleto(model.CodigoMenu.Value, BLPortal.Atual.ConnectionString, true);

                ViewBag.Classe = model.ClasseCSS;
                ViewData["urlatual"] = Convert.ToString(RouteData.Values["url"]);
                return View(model.View ?? "Horizontal", objML);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

    }
}
