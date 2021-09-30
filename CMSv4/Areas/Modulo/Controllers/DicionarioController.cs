using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class DicionarioController : ModuloBaseController<MLModuloDicionarioEdicao, MLModuloDicionarioHistorico, MLModuloDicionarioPublicado>
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
                MLModuloDicionario model = null;

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloDicionarioEdicao>(new MLModuloDicionarioEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    model = CRUD.Obter<MLModuloDicionarioHistorico>(new MLModuloDicionarioHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloDicionarioPublicado>(new MLModuloDicionarioPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null)
                    model = new MLModuloDicionarioEdicao();

                ViewBag.Termos = BLDicionario.ListarTermos(false, BLIdioma.CodigoAtual.Value, BLPortal.Atual.Codigo.Value, model.Grupos, string.Empty, string.Empty);

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
                var model = CRUD.Obter<MLModuloDicionarioEdicao>(new MLModuloDicionarioEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloDicionarioEdicao();
                else
                    ViewBag.isEdicao = true;

                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                ViewData["grupos-todas"] = CRUD.Listar(new MLDicionarioGrupo { CodigoIdioma = BLIdioma.CodigoAtual, CodigoPortal = BLPortal.Atual.Codigo }, PortalAtual.ConnectionString);
                if (!string.IsNullOrEmpty(model.Grupos))
                    ViewData["grupos-selecionadas"] = model.Grupos.Split(',').ToList();

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
        public override ActionResult Editar(MLModuloDicionarioEdicao model)
        {
            try
            {

                if (Request["Grupos"] != null)
                    model.Grupos = Request["Grupos"].Replace("multiselect-all", "").TrimStart(',');
                else
                    model.Grupos = string.Empty;

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

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloDicionario model)
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

        #region Atualiza Listagem Termos

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult AtualizaListagem(string strTermo, string strLetra)
        {
          
            try
            {
                if ((!String.IsNullOrEmpty(strTermo) && strTermo.Length >= 3) || !String.IsNullOrEmpty(strLetra))
                {
                    if (strLetra == "Todos")
                    {
                        strLetra = "";
                    }

                    List<MLDicionarios> lstML = BLDicionario.ListarTermos(false, BLIdioma.CodigoAtual.Value, BLPortal.Atual.Codigo.Value, String.Empty, strTermo, strLetra);
                    var retorno = String.Empty;

                    if (lstML.Count > 0)
                        retorno = BLConteudoHelper.RenderViewToString(this, "ListagemTermo", null, lstML);
                    else
                        if (!String.IsNullOrEmpty(strTermo))
                            retorno = "Não foi encontrado nenhum resultado para: '" + strTermo + "'";
                        else
                            retorno = "Não foi encontrado nenhum resultado com a letra: '" + strLetra + "'";

                    return Json(new { success = true, html = retorno.ToString() });
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, html = "" });
            }
            return Json(new { success = false, html = "" });
        }

        #endregion

    }
}
