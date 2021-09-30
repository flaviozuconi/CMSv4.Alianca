using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLResultado
    {
        #region ListarAnos

      
        public static List<MLResultadoAno> ListarAnos(bool IsCache,decimal CodigoIdioma)
        {
            var portal = BLPortal.Atual;
            var cacheKey = string.Format("resultados_anos_{0}", CodigoIdioma);
            var cachedValue = BLCachePortal.Get<List<MLResultadoAno>>(cacheKey);

            if (cachedValue != null && IsCache)
                return cachedValue;

            using (var command = Database.NewCommand("USP_MOD_RES_L_ANOS", portal.ConnectionString))
            {
                // Parametros                        
                command.NewCriteriaParameter("@IDIOMA", SqlDbType.Decimal, 18, CodigoIdioma);
             
             
                // Execucao

                var lstRetorno = Database.ExecuteReader<MLResultadoAno>(command);
                BLCachePortal.Add(portal.Codigo.Value, cacheKey, lstRetorno, 1);

                return lstRetorno;
            }
        }

        #endregion

        #region ListarAnos


        public static List<MLResultadoCidade> ListarCidades()
        {
            var portal = BLPortal.Atual;
          
            using (var command = Database.NewCommand("USP_L_CIDADES", portal.ConnectionString))
            {
                // Execucao

                var lstRetorno = Database.ExecuteReader<MLResultadoCidade>(command);
              
                return lstRetorno;
            }
        }

        #endregion

    }
}
