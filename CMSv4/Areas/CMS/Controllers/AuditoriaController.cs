using Framework.Utilities;
using Framework.Utilities.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class AuditoriaController : SecurePortalController
    {
        //
        // GET: /CMS/Auditoria/
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index(decimal? funcionalidade, string codigo, bool? pularPortal)
        {
            var model = new MLAuditoria { CodigoFuncionalidade = funcionalidade, CodigoReferencia = codigo, CodigoPortal = (pularPortal.GetValueOrDefault() ? null : PortalAtual.Codigo) };

            return View(model);
        }

        //
        // GET: /CMS/Auditoria/Completo
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Completo()
        {
            return View();
        }

        #region Listar

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Listar(MLAuditoria criterios)
        {
            try
            {
                //criterios.CodigoPortal = PortalAtual.Codigo;
                return CRUD.ListarJson(criterios, Request.QueryString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/ListarTodos
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarTodos(decimal? codigoPortal)
        {
            try
            {
                return CRUD.ListarJson(new MLAuditoriaRelatorio { CodigoPortal = codigoPortal, Acao = Request.QueryString["Acao"] }, Request.QueryString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

    }
}
