using System;
using System.Data;
using Framework.DataLayer;
using System.Transactions;
using Framework.Utilities;
using CMSv4.Model;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLFuncionalidade : BLCRUD<MLFuncionalidade>
    {
        #region Salvar

        public decimal Salvar(MLFuncionalidade model, List<MLFuncionalidadeItemPermissaoForm> permissoes)
        {
            using (var scope = new TransactionScope())
            {
                if (!model.Ativo.HasValue)
                    model.Ativo = false;

                model.Codigo = CRUD.Salvar(model);

                foreach (var item in permissoes)
                {
                    var novoItem = new MLGrupoPermissao
                    {
                        CodigoFuncionalidade = model.Codigo,
                        CodigoGrupo = item.CodigoGrupo,
                        Visualizar = item.NivelPermissao.HasValue && item.NivelPermissao >= 1,
                        Modificar = item.NivelPermissao.HasValue && item.NivelPermissao >= 2,
                        Excluir = item.NivelPermissao.HasValue && item.NivelPermissao >= 3
                    };

                    CRUD.Salvar(novoItem);
                }

                scope.Complete();
            }

            return model.Codigo.GetValueOrDefault(0);
        }

        #endregion

        #region Obter Completo

        /// <summary>
        /// Obtem um objeto de funcionalidade e seus grupos
        /// </summary>
        public static MLFuncionalidade ObterCompleto(decimal codigo)
        {
            MLFuncionalidade retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_FWK_S_FUNCIONALIDADE_COMPLETO"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@FUN_N_CODIGO", SqlDbType.Decimal, 18, codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLFuncionalidade();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLFuncionalidade>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[1].Rows)
                            {
                                retorno.Permissoes.Add(Database.FillModel<MLFuncionalidadeItemPermissao>(row));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }

        #endregion

        #region Atualizar com Permissoes

        /// <summary>
        /// Atualizar
        /// </summary>
        public static void Atualizar(MLFuncionalidade funcionalidade)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var codigo = CRUD.Salvar(funcionalidade);

                    foreach (var item in funcionalidade.Permissoes)
                    {
                        var novoItem = new MLGrupoPermissao
                        {
                            CodigoFuncionalidade = codigo,
                            CodigoGrupo = item.CodigoGrupo,
                            Visualizar = item.Visualizar ?? false,
                            Modificar = item.Modificar ?? false,
                            Excluir = item.Excluir ?? false
                        };

                        CRUD.Salvar<MLGrupoPermissao>(novoItem);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion
    }
}
