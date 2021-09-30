using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLFaq : BLCRUD<MLFaq>
    {
        #region Obter

        public MLFaqView Obter(decimal? id, string connectionString = "")
        {
            var model = new MLFaqView();

            /*Analisar a possibilidade de implementar um framework de Mapping. Ex.: AutoMapper*/
            if (id.HasValue)
            {
                var faq = this.Obter(new MLFaq { Codigo = id }, connectionString);
                model.Codigo = faq.Codigo;
                model.Ativo = faq.Ativo;
                model.CodigoCategoria = faq.CodigoCategoria;
                model.CodigoPortal = faq.CodigoPortal;
                model.DataCadastro = faq.DataCadastro;
                model.Pergunta = faq.Pergunta;
                model.Resposta = faq.Resposta.Unescape();
            }

            model.ListaCategorias = CRUD.Listar<MLFaqCategoria>(new MLFaqCategoria { Ativo = true, CodigoPortal = PortalAtual.Codigo }, connectionString);
            return model;
        }

        #endregion

        #region FaqCategoria

        public MLFaqCategoria ObterCategoria(decimal? id, string connectionString = "")
        {
            return CRUD.Obter<MLFaqCategoria>(new MLFaqCategoria { Codigo = id }, connectionString) ?? new MLFaqCategoria();
        }

        public decimal SalvarCategoria(MLFaqCategoria model, string connectionString = "")
        {
            model.Ativo = model.Ativo.GetValueOrDefault();
            model.CodigoPortal = model.CodigoPortal.GetValueOrDefault(PortalAtual.Codigo.Value);
            model.CodigoIdioma = model.CodigoIdioma.GetValueOrDefault(BLIdioma.CodigoAtual.Value);

            if (!(model.Codigo.GetValueOrDefault() > 0))
                model.DataCadastro = DateTime.Now;

            return CRUD.Salvar(model, PortalAtual.ConnectionString);
        }

        public int ExcluirCategoria(List<string> ids, string connectionString = "")
        {
            return CRUD.Excluir<MLFaqCategoria>(ids, connectionString);
        }

        #endregion
    }
}
