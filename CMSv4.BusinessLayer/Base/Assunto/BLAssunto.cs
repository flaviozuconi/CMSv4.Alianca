using CMSv4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer
{
    public class BLAssunto : BLCRUD<MLAssuntos>
    {
        #region Salvar Parcial

        public override decimal SalvarParcial(MLAssuntos model, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;

                model.IsAtivo = model.IsAtivo.GetValueOrDefault(false);
                model.CodigoPortal = portal.Codigo;
                model.Codigo = base.SalvarParcial(model, connectionString);

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
