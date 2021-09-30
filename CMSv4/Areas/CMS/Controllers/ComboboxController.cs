using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Framework.Utilities;

namespace CMSApp.Areas.CMS.Controllers
{
    public partial class ComboboxController : SecureController
    {
        #region Idioma

        /// <summary>
        /// Listagem
        /// </summary>
        public ActionResult Idioma(string selectedValue)
        {
            try
            {
                ViewData["selectedValue"] = selectedValue;
                return PartialView(CRUD.Listar<MLIdioma>(new MLIdioma { Ativo = true }));
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
