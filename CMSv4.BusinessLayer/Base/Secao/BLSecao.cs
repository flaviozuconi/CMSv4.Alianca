using System;
using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using Framework.Utilities;
using System.Transactions;
using CMSv4.Model;

namespace CMSv4.BusinessLayer
{
    public class BLSecao
    {
        #region Obter Completo

        /// <summary>
        /// Obtem um objeto completo
        /// </summary>
        public static MLSecao ObterCompleto(decimal id, string connectionString)
        {
            try
            {
                var retorno = new MLSecao();

                using (var command = Database.NewCommand("USP_CMS_S_SECAO_COMPLETO", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@SEC_N_CODIGO", SqlDbType.Decimal, 18, id);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLSecao>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[1].Rows)
                            {
                                retorno.Grupos.Add(Database.FillModel<MLSecaoItemGrupo>(row));
                            }
                        }

                        if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[2].Rows)
                            {
                                retorno.Permissao.Add(Database.FillModel<MLSecaoPermissao>(row));
                            }
                        }
                    }

                }

                return retorno;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }


        }

        #endregion

        #region Salvar Completo

        /// <summary>
        /// Atualizar
        /// </summary>
        public static void SalvarCompleto(MLSecao model, string connectionString)
        {
            using (var scope = new TransactionScope(connectionString))
            {
                var codigo = CRUD.Salvar<MLSecao>(model);
                CRUD.Excluir<MLSecaoItemGrupo>("CodigoSecao", model.Codigo.Value);
                foreach (var item in model.Grupos)
                {
                    
                        var novoItem = new MLSecaoItemGrupo
                        {
                            CodigoSecao = codigo,
                            CodigoGrupo = item.CodigoGrupo
                        };

                        CRUD.Salvar<MLSecaoItemGrupo>(novoItem);
                    
                }

                CRUD.Excluir<MLSecaoPermissao>("CodigoSecao", model.Codigo.Value);
                foreach (var item in model.Permissao)
                {
                    var novoItem = new MLSecaoPermissao
                    {
                        CodigoSecao = codigo,
                        CodigoGrupoCliente = item.CodigoGrupoCliente
                    };

                    CRUD.Salvar<MLSecaoPermissao>(novoItem);
                }
                scope.Complete();
            }
        }

        #endregion

        #region ListarAdmin

        /// <summary>
        /// Listar Admin
        /// </summary>
        public static List<MLSecao> ListarAdmin(MLPortal portal, MLUsuario usuario)
        {
            try
            {
                using (var command = Database.NewCommand("USP_CMS_L_SECAO_ADMIN", portal.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@GRUPOS_DO_USUARIO", SqlDbType.VarChar, -1, usuario.GruposToString());
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, portal.Codigo);

                    // Execucao
                    return Database.ExecuteReader<MLSecao>(command);
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
