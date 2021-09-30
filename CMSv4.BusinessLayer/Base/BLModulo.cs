using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CMSv4.BusinessLayer
{
    public class BLModulo
    {
        public static MLModulo Obter(decimal? codigoModulo, string connectionString)
        {
            return CRUD.Obter(new MLModulo { Codigo = codigoModulo }, connectionString);
        }

        public static List<MLModulo> Listar(string connectionString)
        {
            return CRUD.Listar<MLModulo>(new MLModulo() { Ativo = true }, connectionString)
                .OrderBy(x => x.Nome)
                .ToList();
        }

        public static List<MLModulo> Listar(MLPortal portal)
        {
            using (var command = Database.NewCommand("USP_MOD_L_MODULOS", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);

                // Execucao
                return Database.ExecuteReader<MLModulo>(command);
            }
        }
    }
}
