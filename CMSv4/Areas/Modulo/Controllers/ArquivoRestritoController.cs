using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ArquivoRestritoController : ModuloBaseController<MLModuloArquivoRestritoEdicao, MLModuloArquivoRestritoHistorico, MLModuloArquivoRestritoPublicado>
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

                var model = CRUD.Obter(new MLModuloArquivoRestritoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio });

                if (model == null || (!model.CodigoPagina.HasValue))
                {
                    model = new MLModuloArquivoRestritoPublicado()
                    {
                        CodigoPagina = codigoPagina,
                        Repositorio = repositorio
                    };
                }

                ViewBag.Action = string.IsNullOrEmpty(model.NomeView) ? "DownloadFerramenta" : model.NomeView;

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
                var model = new MLModuloArquivoRestrito();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter(new MLModuloArquivoRestritoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio });
                    if (model == null) model = new MLModuloArquivoRestrito { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloArquivoRestritoHistorico>(new MLModuloArquivoRestritoHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio });
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloArquivoRestritoPublicado>(new MLModuloArquivoRestritoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio });
                }

                if (model == null) model = new MLModuloArquivoRestrito();
                
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
                var model = CRUD.Obter<MLModuloArquivoRestritoEdicao>(new MLModuloArquivoRestritoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio });

                if (model == null) model = new MLModuloArquivoRestritoEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                if (!string.IsNullOrEmpty(model.Categorias))
                {
                    var cats = model.Categorias.Split(',');
                    foreach (var cat in cats)
                        model.CodigosCategoriaArquivo.Add(decimal.Parse(cat));
                }

                ///TIPO DE VIEWS DISPONÍVEIS
                var lstViews = new List<string>() { "DownloadFerramenta" };
                IEnumerable<SelectListItem> views = lstViews.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                });

                ViewBag.Views = views;

                ///PASTAS DISPONÍVEIS
                var lstPastas = CRUD.Listar(new MLArquivoRestritoCategoria { CodigoPortal = PortalAtual.Codigo }, portal.ConnectionString);
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
        public override ActionResult Editar(MLModuloArquivoRestritoEdicao model)
        {
            try
            {
                foreach (decimal cat in model.CodigosCategoriaArquivo)
                    model.Categorias += cat.ToString() + ",";

                model.Categorias = model.Categorias.Substring(0, (model.Categorias.Length - 1));
				model.Quantidade = 1;

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

                CRUD.Excluir<MLModuloArquivoRestritoEdicao>(codigoPagina.Value, repositorio.Value);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region DownloadFerramenta
        /// <summary>
        /// DownloadFerramenta
        /// </summary>
        /// <returns></returns>
        [CheckPermission(Permissao.Publico)]
        public ActionResult DownloadFerramenta(MLModuloArquivoRestrito model)
        {
            try
            {
                var objML = BLArquivoRestrito.Obter(model.Categorias);
                ViewData["objArquivo"] = objML;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
		#endregion

		#region DownloadArquivo
		/// <summary>
		/// Realiza o download do arquivo se houver um usuário cadastrado.
		/// </summary>
		/// <param name="codigo"></param>
		/// <returns>{ success: [bool], arquivo: [string] }</returns>
		[CheckPermission(Permissao.Publico)]
		public JsonResult DownloadArquivo(string codigo)
		{
			//Define o arquivo que será baixado.-------------------------------
			MLCliente clienteLogado = BLCliente.ObterLogado();

			if (clienteLogado != null && clienteLogado.Codigo.HasValue)
			{
				MLArquivoRestrito arquivo = BLArquivoRestrito.Obter(codigo);
				MLArquivoRestritoCategoria categoria = CRUD.Obter<MLArquivoRestritoCategoria>(arquivo.CodigoCategoria.Value);

				return Json(new {
					success = true,
					arquivo = Path.Combine(categoria.PastaRelativaArquivo(PortalAtual.Diretorio), arquivo.ArquivoUrl)
				});
			}
			else
				return Json(new { success = false });
			//-----------------------------------------------------------------
		}
		#endregion

		#region Script

		/// <summary>
		/// Adicionar Script
		/// </summary>
		[CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloArquivoRestrito model)
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
    }
}
