using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    public class BLListaConfig : BLCRUD<MLListaConfig>
    {

        #region ListarPorUsuario

        /// <summary>
        /// Listar as Listas de Conteúdo autorizadas para o usuário
        /// </summary>
        public List<MLListaConfig> ListarPorUsuario(MLPortal portal, string gruposUsuario)
        {
            using (var command = Database.NewCommand("USP_MOD_LIS_L_LISTA_MENUADMIN", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, portal.Codigo);
                command.NewCriteriaParameter("@GRP_N_CODIGOS", SqlDbType.VarChar, 8000, gruposUsuario);

                // Execucao
                return Database.ExecuteReader<MLListaConfig>(command);
            }
        }

        #endregion

        #region ValidarPermissaoLista

        public bool ValidarPermissaoLista(decimal idLista)
        {
            var lista = this.ListarPorUsuario(PortalAtual.Obter, BLUsuario.ObterLogado().GruposToString());
            return lista?.Find(o => o.Codigo == idLista)?.CodigoPortal == PortalAtual.Codigo;
        }

        #endregion

        public override decimal Salvar(MLListaConfig model, string connectionString = "")
        {
            if (!model.Codigo.HasValue)
                model.Codigo = base.Listar(new MLListaConfig(), connectionString).Count + 1;

            return base.Salvar(model, connectionString);
        }

    }

    public class BLListaConfigCategoria : BLCRUD<MLListaConfigCategoria> {

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            int qtdeExcluidos = 0;
            foreach (var item in ids)
            {
                var id = Convert.ToDecimal(item);
                var model = CRUD.Obter<MLListaConfigCategoria>(id, connectionString);

                if (new BLListaConfig().ValidarPermissaoLista(model.CodigoLista.Value))
                    qtdeExcluidos += base.Excluir(Convert.ToDecimal(item), connectionString);
            }
            return qtdeExcluidos; 
        }
    }
}
