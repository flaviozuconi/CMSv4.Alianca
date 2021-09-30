using System;
using Framework.Utilities;
using System.Web.Mvc;

namespace VM2.Areas.CMS.Helpers
{
    /// <summary>
    /// Classe para implementação de Módulos do CMS
    /// </summary>
    public abstract class ModuloBaseController<TModelEdicao, TModelHistorico, TModelPublicado> : SecurePortalController
    {

        /// <summary>
        /// Este método será usado para renderização do módulo na área pública do CMS
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public virtual ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                var model = definirValor<TModelPublicado>(codigoPagina, repositorio);

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                model = CRUD.Obter(model, BLPortal.Atual.ConnectionString);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        [CheckPermission(global::Permissao.Visualizar, "/cms/{0}/pagina")]
        public virtual ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;

                // Visualizar em Edição
                if (edicao.HasValue && edicao.Value)
                {
                    var model = CRUD.Obter(definirValor<TModelEdicao>(codigoPagina, repositorio), portal.ConnectionString);

                    if (model == null)
                        model = Activator.CreateInstance<TModelEdicao>();

                    return PartialView("Index",  model);
                }

                // Visualizar Histórico
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    var model = CRUD.Obter(definirValor<TModelHistorico>(codigoPagina, repositorio), portal.ConnectionString);

                    if (model == null)
                        model = Activator.CreateInstance<TModelHistorico>();

                    return PartialView("Index", model);
                }

                // Visualizar Publicado
                else
                {
                    var model = CRUD.Obter(definirValor<TModelPublicado>(codigoPagina, repositorio), portal.ConnectionString);

                    if (model == null)
                        model = Activator.CreateInstance<TModelPublicado>();

                    return PartialView("Index", model);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

     
        [HttpGet]
        [CheckPermission(global::Permissao.Modificar, "/cms/{0}/pagina")]
        public virtual ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = definirValor<TModelEdicao>(codigoPagina, repositorio);

                var portal = PortalAtual.Obter;
                model = CRUD.Obter(model, portal.ConnectionString);

                if (model == null) model = definirValor<TModelEdicao>(codigoPagina, repositorio);

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [CheckPermission(global::Permissao.Modificar, "/cms/{0}/pagina")]
        public virtual ActionResult Editar(TModelEdicao model)
        {
            //Salvar
            CRUD.Salvar(model, PortalAtual.ConnectionString);
            return Json(new { success = true });
        }

        [CheckPermission(global::Permissao.Modificar, "/cms/{0}/pagina")]
        public virtual ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                CRUD.Excluir<TModelEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        private T definirValor<T>(decimal? codigoPagina, int? repositorio, Guid? CodigoHistorico = null)
        {
            var model = Activator.CreateInstance<T>();
            model.GetType().GetProperty("CodigoPagina").SetValue(model, codigoPagina, null);
            model.GetType().GetProperty("Repositorio").SetValue(model, repositorio, null);

            if(CodigoHistorico.HasValue)
                model.GetType().GetProperty("CodigoHistorico").SetValue(model, CodigoHistorico, null);

            return model;
        }
    }

}
