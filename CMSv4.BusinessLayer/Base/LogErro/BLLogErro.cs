using Framework.Utilities;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLLogErro
    {
        #region Listar

        public static List<MLLogErro> Listar()
        {
            // Pagina
            int start = 0;
            int length = 0;
            int.TryParse(HttpContextFactory.Current.Request.QueryString["start"], out start);
            int.TryParse(HttpContextFactory.Current.Request.QueryString["length"], out length);

            if (length == 0) length = 10;

            return ApplicationLog.Listar(start, length);
        }

        #endregion
    }
}
