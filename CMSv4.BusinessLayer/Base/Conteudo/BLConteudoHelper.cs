using System;
using System.IO;
using System.Web.Mvc;

namespace CMSv4.BusinessLayer
{
    public class BLConteudoHelper
    {
        #region Render View

        /// <summary>
        /// Renderiza uma VIEW em string e retorna o conteúdo
        /// </summary>
        public static string RenderViewToString(Controller controller, string viewName, string masterName, object model)
        {
            try
            {
                controller.ViewData.Model = model;

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, masterName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion

        #region Render Partial View

        /// <summary>
        /// Renderiza uma VIEW em string e retorna o conteúdo
        /// </summary>
        public static string RenderPartialViewToString(Controller controller, string areaName, string controllerName, string viewName, object model)
        {
            var oldArea = controller.RouteData.Values["areaname"];
            var oldController = controller.RouteData.Values["controller"];

            try
            {
                controller.ViewData.Model = model;
                controller.RouteData.Values["areaname"] = areaName;
                controller.RouteData.Values["controller"] = controllerName;

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                controller.RouteData.Values["areaname"] = oldArea;
                controller.RouteData.Values["controller"] = oldController;
            }
        }

        #endregion
    }
}
