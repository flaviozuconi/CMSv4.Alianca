using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer
{
    public class BLCategoriaAgrupador : BLCRUD<MLCategoriaAgrupador>
    {
        #region Salvar

        public override decimal Salvar(MLCategoriaAgrupador model, string connectionString = "")
        {
            try
            {                
                var portal = PortalAtual.Obter;

                model.Ativo = model.Ativo.GetValueOrDefault(false);
                model.CodigoPortal = portal.Codigo;
                model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
