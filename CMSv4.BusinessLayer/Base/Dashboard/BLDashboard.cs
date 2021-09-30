using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace CMSv4.BusinessLayer
{
    public class BLDashboard
    {
        #region Definir Idioma

        public void DefinirIdioma(string siglaIdioma)
        {
            if (!string.IsNullOrEmpty(siglaIdioma))
            {
                var idioma = new CRUD.Select<MLIdioma>().Equals(a => a.Sigla, siglaIdioma).First(PortalAtual.ConnectionString);
                if (idioma != null && idioma.Codigo.HasValue)
                {
                    ///IdiomaAdmin = idioma;

                    BLIdioma.AtualAdm = idioma;

                    Thread.CurrentThread.CurrentCulture = new CultureInfo(siglaIdioma);
                }
            }
        }

        #endregion
    }
}
