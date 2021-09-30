using System;
using System.Data;
using System.Transactions;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using CMSv4.Model;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Grupo
    /// </summary>
    public class BLGrupo : BLCRUD<MLGrupo>
    {
        #region Obter Completo

        /// <summary>
        /// Obtem um objeto de grupo e suas permissoes
        /// </summary>
        public static MLGrupo ObterCompleto(decimal codigoGrupo)
        {
            MLGrupo retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_FWK_S_GRUPO_COMPLETO"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@GRP_N_CODIGO", SqlDbType.Decimal, 18, codigoGrupo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLGrupo();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLGrupo>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[1].Rows)
                            {
                                retorno.Permissoes.Add(Database.FillModel<MLGrupoItemPermissao>(row));
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
        /// <param name="grupo">Grupo</param>
        public static void Atualizar(MLGrupo grupo)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var conexao = Database.NewTransactionalConnection())
                    {
                        var codigoGrupo = CRUD.Salvar<MLGrupo>(grupo);

                        foreach (var item in grupo.Permissoes)
                        {
                            var novoItem = new MLGrupoPermissao
                            {
                                CodigoFuncionalidade = item.CodigoFuncionalidade,
                                CodigoGrupo = codigoGrupo,
                                Visualizar = item.Visualizar ?? false,
                                Modificar = item.Modificar ?? false,
                                Excluir = item.Excluir ?? false
                            };

                            CRUD.Salvar<MLGrupoPermissao>(novoItem);
                        }

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Listar Por Usuario

        public static List<MLGrupo> ListarPorUsuario(decimal? codigoUsuario)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_L_GRUPO_USUARIO"))
                {
                    command.NewCriteriaParameter("@GXU_USU_N_CODIGO", codigoUsuario);

                    return Database.ExecuteReader<MLGrupo>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Salvar

        public decimal Salvar(MLGrupo model, List<MLGrupoItemPermissaoForm> permissoes, string connectionString = "")
        {
            using (var scope = new TransactionScope())
            {
                model.Ativo = model.Ativo.GetValueOrDefault(false);
                model.Codigo = base.Salvar(model);

                foreach (var item in permissoes)
                {
                    var novoItem = new MLGrupoPermissao
                    {
                        CodigoFuncionalidade = item.CodigoFuncionalidade,
                        CodigoGrupo = model.Codigo,
                        Visualizar = item.NivelPermissao.HasValue && item.NivelPermissao >= 1,
                        Modificar = item.NivelPermissao.HasValue && item.NivelPermissao >= 2,
                        Excluir = item.NivelPermissao.HasValue && item.NivelPermissao >= 3
                    };

                    CRUD.Salvar(novoItem);
                }

                scope.Complete();
            }

            return base.Salvar(model, connectionString);
        }

        #endregion
    }
}
