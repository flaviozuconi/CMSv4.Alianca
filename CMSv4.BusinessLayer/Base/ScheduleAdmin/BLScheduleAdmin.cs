using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer.Base
{
    public class BLScheduleAdmin
    {
        #region Listar
        public List<MLScheduleDataTable> Listar(string filtro, int? start, int? length, int? orderBy, bool isAsc)
        {
            using (var command = Database.NewCommand("USP_MOD_L_SCHEDULE", BLPortal.Atual.ConnectionString))
            {
                // Parametros

                command.NewCriteriaParameter("@FILTRO", SqlDbType.VarChar, 200, filtro);
                command.NewCriteriaParameter("@START", SqlDbType.Int, start);
                command.NewCriteriaParameter("@LENGTH", SqlDbType.Int, length);
                command.NewCriteriaParameter("@ORDERBY", SqlDbType.Int, orderBy);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, isAsc);

                // Execução
                return Database.ExecuteReader<MLScheduleDataTable>(command);
            }
        }
        #endregion
    }
}
