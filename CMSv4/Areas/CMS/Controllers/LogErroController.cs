using CMSv4.BusinessLayer;
using Framework.Utilities;
using System;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class LogErroController : SecureController
    {
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
        }

        #region Listar

        [DataTableHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Listar(MLLogErro criterios)
        {            
            var lista = BLLogErro.Listar();

            return new DataTableResult()
            {
                Data = lista,
                TotalRows = lista.Count > 0 ? lista[0].Total.GetValueOrDefault(0) : 0,
            };
        }
        #endregion

        #region Excluir

        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Excluir()
        {
            ApplicationLog.ExcluirLogs();

            return RedirectToAction("Index");
        }
        #endregion
    }
}
