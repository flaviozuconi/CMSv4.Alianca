using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Modulo de Banners
    /// </summary>
    public class BLModuloBanner
    {
        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLBannerArquivoPublico> ListarPublico(MLModuloBanner model)
        {
            string strCache = $"banner-{model.CodigoBanner}";
            var retorno = BLCachePortal.Get<List<MLBannerArquivoPublico>>(strCache);

            if (retorno != null) return retorno;

            using (var command = Database.NewCommand("USP_MOD_BAN_L_BANNER_PUBLICO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@BAN_N_CODIGO", SqlDbType.Decimal, 18, model.CodigoBanner);
                
                // Execucao
                retorno = Database.ExecuteReader<MLBannerArquivoPublico>(command);
                BLCachePortal.Add(strCache, retorno);

                return retorno;
            }
        }

        #endregion
    }
}
