using CMSv4.Model.Base;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer.Base
{
    public class BLProgramacaoProposta
    {
        #region Listar
        public List<MLProgramacaoPropostaDataTable> Listar(string filtro, int? start, int? length, int? orderBy, bool isAsc)
        {
            using (var command = Database.NewCommand("USP_MOD_L_PROGRAMACAO_PROPOSTA", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                
                command.NewCriteriaParameter("@FILTRO", SqlDbType.VarChar, 200, filtro);
                command.NewCriteriaParameter("@START", SqlDbType.Int, start);
                command.NewCriteriaParameter("@LENGTH", SqlDbType.Int, length);
                command.NewCriteriaParameter("@ORDERBY", SqlDbType.Int, orderBy);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, isAsc);

                // Execucao
                return Database.ExecuteReader<MLProgramacaoPropostaDataTable>(command);
            }
        }
        #endregion
    }
}
