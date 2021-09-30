using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    public class BLNewsLetter : BLCRUD<MLNewsletterAdmin>
    {
        #region Salvar

        public override decimal Salvar(MLNewsletterAdmin model, string connectionString = null)
        {
            var portal = PortalAtual.Obter;

            if (!model.CodigoPortal.HasValue)
                model.CodigoPortal = portal.Codigo;
            
            if (!string.IsNullOrEmpty(model.Assuntos))
                model.Assuntos = model.Assuntos.Replace("multiselect-all", "");
            
            model.ChaveSecreta = Guid.NewGuid();
            model.Codigo = base.Salvar(model, connectionString ?? portal.ConnectionString);

            return model.Codigo.Value;
        }

        public List<MLNewsletter> Listar(MLNewsletter criterios, string orderBy, string sortOrder, int pagina, double quantidade, out double retornoTotalRegistros, string buscaGenerica, string[] camposBuscaGenerica, string connectionString = "")
        {
            return CRUD.Listar<MLNewsletter>(criterios, orderBy, sortOrder, pagina, quantidade, out retornoTotalRegistros, buscaGenerica, new string[] { "Codigo", "Nome", "Assuntos", "Email" }, PortalAtual.Obter.ConnectionString);
        }

        #endregion

    }
}
