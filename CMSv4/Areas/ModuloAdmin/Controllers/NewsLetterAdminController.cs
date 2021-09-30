using Framework.Utilities;
using System;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class NewsLetterAdminController : AdminBaseCRUDPortalController<MLNewsletterAdmin, MLNewsletterAdmin>
    {
        #region Item

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]
        public override ActionResult Item(MLNewsletterAdmin model)
        {
            TempData["Salvo"] = new BLNewsLetter().Salvar(model) > 0;

            return RedirectToAction("Index");
        }

        #endregion

        #region Exportar

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Exportar(MLNewsletter criterios, string buscaGenerica)
        {

            double total;
            string orderBy = "Codigo";
            string sortOrder = "DESC";

            // Busca lista no banco de dados

            criterios.CodigoPortal = PortalAtual.Obter.Codigo;

            var lista = new BLNewsLetter().Listar(criterios, orderBy, sortOrder, 1, 9000, out total, buscaGenerica, new string[] { "Codigo", "Nome", "Assuntos", "Email" }, PortalAtual.Obter.ConnectionString);

            // Retorna os resultados

            string fileName = "newsletter-complete-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".csv";

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/csv";

            Response.BufferOutput = true;
            Response.ContentEncoding = System.Text.ASCIIEncoding.GetEncoding("ISO-8859-1");

            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.AddHeader("pragma", "public");

            Response.Write(BLUtilitarios.ToCSV<MLNewsletter>(lista, ";"));

            Response.End();

            return Json(new { Sucesso = true });
        }

        #endregion
    }
}
