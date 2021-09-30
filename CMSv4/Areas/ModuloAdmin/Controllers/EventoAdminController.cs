using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using System.Web.Helpers;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class EventoAdminController : AdminBaseCRUDPortalController<MLEvento, MLEvento>
    {
        #region Calendario

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Calendario() => View();

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Modificar)]
        public override ActionResult Item(decimal? id)
        {
            return View(new BLModuloEventos().Obter(id, Request.QueryString["CodigoIdioma"], Request.QueryString["CodigoBase"]));
        }

        [HttpPost, ValidateAntiForgeryToken, JsonHandleError, ValidateInput(false), CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        public override ActionResult Item(MLEvento model)
        {
            TempData["Salvo"] = new BLModuloEventos().Salvar(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion

        #region ListarCalendario

        [HttpPost, JsonHandleError, CheckPermission(global::Permissao.Visualizar)]
        public JsonResult Listarcalendario(string start, string end, decimal? codigoIdioma)
        {

            DateTime dStart;
            DateTime dEnd;

            DateTime.TryParse(start, out dStart);
            DateTime.TryParse(end, out dEnd);

            var lista = BLModuloEventos.ListarCalendario(new MLEvento() { Ativo = true, CodigoPortal = PortalAtual.Codigo, CodigoIdioma = codigoIdioma, DataInicio = dStart, DataTermino = dEnd });

            var eventList = (from e in lista
                             select new
                             {
                                 id = e.Codigo.Value.ToString(),
                                 title = e.Titulo,
                                 start = formatarDataCalendario(e.DataInicio),
                                 end = formatarDataCalendario(e.DataTermino),
                                 allDay = e.DataInicio == (e.DataTermino ?? e.DataInicio) && e.DataInicio.Value.TimeOfDay.TotalSeconds == 0,
                                 className = (e.DataTermino <= DateTime.Now) ? "fc-white" : ((DateTime.Now > e.DataInicio && DateTime.Now < e.DataTermino) ? "fc-green" : "fc-blue")
                             }).ToArray();

            return Json(eventList, JsonRequestBehavior.AllowGet);
        }

        private string formatarDataCalendario(DateTime? data)
        {
            if (!data.HasValue)
            {
                return null;
            }

            bool possuiHoras = data.Value.TimeOfDay.TotalSeconds > 0;

            if (possuiHoras)
            {
                return data.Value.ToString("s");
            }
            else
            {
                return data.Value.ToString("yyyy-MM-dd");
            }
        }

        #endregion

        #region ValidarUrl

        /// <summary>
        /// Validar se a Url já esta cadastrada no sistema
        /// </summary>
        /// <param name="url">Url do Evento</param>
        /// <returns>Json</returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarUrl(string url, decimal? id)
        {
            try
            {
                var codigo = BLModuloEventos.Obter(url, id);

                if ((codigo.HasValue && codigo.Value > 0))
                    return Json(T("Já existe um evento cadastrado com essa Url"));
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Já existe um evento cadastrado com essa Url"));
            }
        }

        #endregion

        #region Seo

        [HttpPost, JsonHandleError, CheckPermission(global::Permissao.Modificar)]
        public ActionResult SeoOutros(decimal codigoConteudo, string seooutros)
        {
            CRUD.SalvarParcial<MLEventoSEO>(new MLEventoSEO() { Codigo = codigoConteudo, Outros = seooutros }, PortalAtual.ConnectionString);
            return Json(new { Sucesso = true });
        }

        #endregion
    }
}
