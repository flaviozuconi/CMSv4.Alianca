using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class AniversariantesController : ModuloBaseController<MLModuloAniversarianteEdicao, MLModuloAniversarianteHistorico, MLModuloAniversariantePublicado>
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
                MLModuloAniversariantePublicado model = CRUD.Obter<MLModuloAniversariantePublicado>(new MLModuloAniversariantePublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

                if (model != null)
                    ViewBag.ListaAniversariantes = BLAniversariante.Listar(model, codigoPagina: codigoPagina);
                else
                    model = new MLModuloAniversariantePublicado();
                
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
                MLModuloAniversariante model = null;

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloAniversarianteEdicao>(new MLModuloAniversarianteEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    model = CRUD.Obter<MLModuloAniversarianteHistorico>(new MLModuloAniversarianteHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloAniversariantePublicado>(new MLModuloAniversariantePublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model != null)
                    ViewBag.ListaAniversariantes = BLAniversariante.Listar(model, codigoPagina: codigoPagina);
                
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
                var blPagina = new BLPagina(portal.ConnectionString);
                var model = CRUD.Obter<MLModuloAniversarianteEdicao>(new MLModuloAniversarianteEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloAniversarianteEdicao();
                else
                    ViewBag.isEdicao = true;

                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                ViewData["GruposCliente"] = CRUD.Listar<MLGrupoCliente>(new MLGrupoCliente { Ativo = true, CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);

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
        public override ActionResult Editar(MLModuloAniversarianteEdicao model)
        {
            try
            {
                model.Grupos = Request.Form["listaGrupoCliente"];

                if (!model.Restrito.GetValueOrDefault())
                    model.Grupos = String.Empty;

                //Salvar
                CRUD.Salvar(model, PortalAtual.ConnectionString);

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

                CRUD.Excluir<MLModuloAniversarianteEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

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
        public ActionResult Script(MLModuloAniversariante model)
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

        #region Listagem
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ItemListagem(List<MLColaboradorPublico> model)
        {
            return View(model);
        }
        #endregion

        #region Json
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

        [HttpGet]
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ListarAniversariantesMes(decimal? codigoPagina, int? repositorio, int mes)
        {
            var model = CRUD.Obter<MLModuloAniversariantePublicado>(new MLModuloAniversariantePublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, PortalAtual.Obter.ConnectionString);
            var actionRender = BLConteudoHelper.RenderViewToString(this, "ItemListagem", String.Empty, BLAniversariante.Listar(model, mes: mes, codigoPagina: codigoPagina));

            return Json(new { view = actionRender }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
