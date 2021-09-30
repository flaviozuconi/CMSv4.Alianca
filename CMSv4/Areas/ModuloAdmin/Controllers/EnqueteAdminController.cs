using Framework.Utilities;
using System;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class EnqueteAdminController : AdminBaseCRUDPortalController<MLEnquete, MLEnquete>
    {
        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public override ActionResult Item(decimal? id)
        {
            var model = new MLEnquete();
            if (id.HasValue)
            {
                var portal = PortalAtual.Obter;
                model = new BLModuloEnquete().Obter(new MLEnquete { Codigo = id, CodigoPortal = portal.Codigo }, portal.ConnectionString);
                ViewData["Opcoes"] = new BLModuloEnqueteOpcao().Listar(new MLEnqueteOpcao { CodigoEnquete = id }, "Ordem", "ASC", portal.ConnectionString);
            }

            return View(model);

        }


        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public override ActionResult Item(MLEnquete model)
        {

            model = new BLModuloEnquete().Salvar(model, Request.Form["OpcaoCodigo"] ?? string.Empty, Request.Form["OpcaoTitulo"] ?? string.Empty, PortalAtual.Obter);
            TempData["Salvo"] = model.Codigo.GetValueOrDefault(0) > 0;

            return RedirectToAction("Index");
        }

        #endregion

   
        #region opção

 
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public ActionResult Opcao(MLEnqueteOpcao model)
        {
            if (model.CodigoEnquete.HasValue)
                model.Codigo = new BLModuloEnqueteOpcao().Salvar(model, PortalAtual.Obter.ConnectionString);

            return PartialView("Opcao", model);
        }

        #endregion

        #region RemoverOpcao

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RemoverOpcao(decimal? codigo)
        {
            try
            {
                if (codigo.HasValue)
                    new BLModuloEnqueteOpcao().Excluir(codigo.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { Sucesso = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                if (ex is System.Data.SqlClient.SqlException && ((System.Data.SqlClient.SqlException)ex).Number == 547)
                    return Json(new { Sucesso = false, msg = T("A opção já foi respondida! Impossível a exclusão!") });
                else
                    return Json(new { Sucesso = false, msg = ex.Message });
            }
        }

        #endregion

        #region Resultado

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult Resultado(decimal codigo)
        {

            var resultado = BLModuloEnquete.ListarResultado(codigo, null, string.Empty, false);

            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = resultado.Total,
                    recordsFiltered = resultado.Total,
                    data = resultado.Resultados,
                    success = true
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

        #region Exportar


        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Exportar(decimal codigo)
        {
            try
            {

                var lista = BLModuloEnquete.ListarRelatorio(codigo);

                string fileName = "enquete-complete-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".csv";

                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "text/csv";

                Response.BufferOutput = true;
                Response.ContentEncoding = System.Text.ASCIIEncoding.GetEncoding("ISO-8859-1");

                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.AddHeader("pragma", "public");

                Response.Write(BLUtilitarios.ToCSV<MLEnqueteVotoRelatorio>(lista, ";"));

                Response.End();

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { Sucesso = false });
            }
            return Json(new { Sucesso = true });
        }

        #endregion

    }
}
