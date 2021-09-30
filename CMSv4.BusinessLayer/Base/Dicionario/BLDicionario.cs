using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    public class BLDicionario : BLCRUD<MLDicionarios>
    {
        public static List<MLDicionarios> ListarTermos(bool IsCache, decimal CodigoIdioma, decimal CodigoPortal, string strCodigoGrupos, string strBusca, string strLetraInicial)
        {
            var portal = BLPortal.Atual;
            var cacheKey = string.Format("portal_{0}_resultado_dicionarios", portal.Codigo);
            if (IsCache)
            {
                var cachedValue = BLCachePortal.Get<List<MLDicionarios>>(cacheKey);
                if (cachedValue != null) return cachedValue;
            }

            List<MLDicionarios> retorno = new List<MLDicionarios>();

            using (var command = Database.NewCommand("USP_MOD_L_DICIONARIO_PUBLICO", portal.ConnectionString))
            {
                // Parametros                        
                command.NewCriteriaParameter("@IDIOMA", SqlDbType.Decimal, 18, CodigoIdioma);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, CodigoPortal);
                command.NewCriteriaParameter("@DIC_C_GRUPOS", SqlDbType.VarChar, 100, strCodigoGrupos);
                command.NewCriteriaParameter("@DIC_C_BUSCA", SqlDbType.VarChar, 100, strBusca);
                command.NewCriteriaParameter("@DIC_C_LETRA", SqlDbType.VarChar, 1, strLetraInicial);

                // Execucao
                retorno = Database.ExecuteReader<MLDicionarios>(command);

                BLCachePortal.Add(portal.Codigo.Value, cacheKey, retorno, 1);
                return retorno;
            }
        }

        #region Salvar

        public override decimal Salvar(MLDicionarios model, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;
                model.CodigoIdioma = model.CodigoIdioma.HasValue ? model.CodigoIdioma : BLIdioma.CodigoAtual;
                model.CodigoPortal = model.CodigoPortal.HasValue ? model.CodigoPortal : portal.Codigo;

                return base.Salvar(model, portal.ConnectionString);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
