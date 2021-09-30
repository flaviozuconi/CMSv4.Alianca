using CMSv4.Model;
using Framework.Utilities;
using System;

namespace CMSv4.BusinessLayer
{
    public class BLDicionarioGrupo : BLCRUD<MLDicionarioGrupo>
    {
        #region Salvar

        public override decimal Salvar(MLDicionarioGrupo model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;
            try
            {
                model.CodigoIdioma = model.CodigoIdioma.HasValue ? model.CodigoIdioma : BLIdioma.CodigoAtual;
                model.CodigoPortal = model.CodigoPortal.HasValue ? model.CodigoPortal : portal.Codigo;

               return base.Salvar(model, portal.ConnectionString);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
