using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class ScheduleController : Controller
    {
        //
        // GET: /Modulo/Schedule/

        public ActionResult Index()
        {
            return View();
        }

        #region Eventos Calendario Landing

        [HttpGet]
        [CheckPermission(global::Permissao.Publico)]
        //[OutputCache(Duration = 600, VaryByParam = "idioma")]
        public JsonResult Get(decimal idioma)
        {
           
            try
            {
                var cacheKey = "cache-get-schedule_" + idioma;
                var retorno =  BLCachePortal.Get<List<MLScheduleRetorno>>(cacheKey);
                if(retorno != null)
                {
                    return Json(retorno, JsonRequestBehavior.AllowGet);
                }

                BLIdioma.Atual = CRUD.Obter<MLIdioma>(idioma);
                var lista = CRUD.Listar(new MLSchedule());
                retorno = new List<MLScheduleRetorno>();
                
                foreach(var item in lista)
                {
                    if(retorno.Find(o=>o.Origem.Equals(item.Origem,StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        var destinos = lista.FindAll(o => o.Origem.Equals(item.Origem, StringComparison.InvariantCultureIgnoreCase));
                        var itemOrigem = new MLScheduleRetorno { Origem = item.Origem };
                        foreach(var dest in destinos)
                        {
                            itemOrigem.schedule.Add(Traduzir(dest));
                        }
                        retorno.Add(itemOrigem);
                    }
                }
                BLCachePortal.Add(cacheKey, retorno, 60);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }
        
        private MLSchedule Traduzir(MLSchedule schedule)
        {
            schedule.DeadLine = BLTraducao.T(schedule.DeadLine);
            schedule.TempoDisplay = schedule.Tempo + " " + BLTraducao.T("dias");
            schedule.Eta = BLTraducao.T(schedule.Eta);
            schedule.Ets = BLTraducao.T(schedule.Ets);
            schedule.DeadLineHorario = BLTraducao.T(schedule.DeadLineHorario);
            return schedule;
        }
    

        #endregion


    }
}
