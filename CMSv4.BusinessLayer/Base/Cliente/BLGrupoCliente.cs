using CMSv4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer
{
    public class BLGrupoCliente : BLCRUD<MLGrupoCliente>
    {
        #region Salvar

        public override decimal Salvar(MLGrupoCliente model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;

            model.Ativo = model.Ativo.GetValueOrDefault(false);
            model.DefaultCadastroPublico = model.DefaultCadastroPublico.GetValueOrDefault(false);
            model.CodigoPortal = portal.Codigo;

            return base.Salvar(model, connectionString);
        }

        #endregion
    }
}
