using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    public class BLTelefone
    {

        public static List<MLTelefoneCompleto> ListarTelefones(bool IsCache, decimal CodigoIdioma, decimal CodigoPortal, string strCodigoRegioes)
        {
            var portal = BLPortal.Atual;
            var cacheKey = string.Format("portal_{0}_resultado_telefones_{1}_{2}", portal.Codigo, CodigoIdioma, strCodigoRegioes);
            if (IsCache)
            {
                var cachedValue = BLCachePortal.Get<List<MLTelefoneCompleto>>(cacheKey);
                if (cachedValue != null) return cachedValue;
            }

            List<MLTelefoneCompleto> retorno = new List<MLTelefoneCompleto>();

            using (var command = Database.NewCommand("USP_MOD_L_TELEFONE_PUBLICO", portal.ConnectionString))
            {
                // Parametros                        
                command.NewCriteriaParameter("@IDIOMA", SqlDbType.Decimal, 18, CodigoIdioma);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, CodigoPortal);
                command.NewCriteriaParameter("@REG_C_REGIOES", SqlDbType.VarChar, 100, strCodigoRegioes);

                // Execucao
                retorno = Database.ExecuteReader<MLTelefoneCompleto>(command);

                BLCachePortal.Add(portal.Codigo.Value, cacheKey, retorno, 1);
                return retorno;
            }
        }
    }
}
