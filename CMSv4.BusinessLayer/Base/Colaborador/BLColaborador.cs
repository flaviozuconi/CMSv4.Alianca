using Framework.DataLayer;
using System;
using System.Data;
using System.Collections.Generic;
using CMSv4.Model;
using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLColaborador : BLCRUD<MLColaborador>
    {
        #region SalvarWCF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colaborador"></param>
        public static void SalvarWCF(MLColaborador colaborador)
        {
            //Valida se código de grupo cliente foi preenchido
            if (!colaborador.CodigoGrupo.HasValue)
            {
                throw new NullReferenceException("O Código de Grupo Cliente não foi preenchido.");
            }

            //Valida se o login do cliente foi preenchido
            if (string.IsNullOrEmpty(colaborador.Login))
            {
                throw new NullReferenceException("O Login do cliente não foi preenchido.");
            }

            try
            {
                using (var command = Database.NewCommand("USP_CMS_I_COLABORADOR_WCF"))
                {
                    // Parametros

                    //Obrigatórios
                    command.NewCriteriaParameter("@CLI_C_LOGIN", SqlDbType.VarChar, 200, colaborador.Login);
                    command.NewCriteriaParameter("@GCL_N_CODIGO", SqlDbType.Decimal, 18, colaborador.CodigoGrupo);

                    //Opcionais
                    if (!String.IsNullOrEmpty(colaborador.Matricula))
                    {
                        command.NewCriteriaParameter("@COL_C_MATRICULA", SqlDbType.VarChar, 100, colaborador.Matricula);
                    }

                    if (colaborador.Telefones != null && colaborador.Telefones.Count > 0)
                    {
                        command.NewCriteriaParameter("@COL_C_TELEFONE", SqlDbType.VarChar, 100, string.Join(",", colaborador.Telefones));
                    }

                    if (!String.IsNullOrEmpty(colaborador.Area))
                    {
                        command.NewCriteriaParameter("@COL_C_AREA", SqlDbType.VarChar, 200, colaborador.Area);
                    }

                    if (!String.IsNullOrEmpty(colaborador.LojaLotacao))
                    {
                        command.NewCriteriaParameter("@COL_C_LOJA_LOTACAO", SqlDbType.VarChar, 100, colaborador.LojaLotacao);
                    }

                    if (!String.IsNullOrEmpty(colaborador.SuperiorImediato))
                    {
                        command.NewCriteriaParameter("@COL_C_SUPERIOR_IMEDIATO", SqlDbType.VarChar, 200, colaborador.SuperiorImediato);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Cargo))
                    {
                        command.NewCriteriaParameter("@COL_C_CARGO", SqlDbType.VarChar, 100, colaborador.Cargo);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Sexo))
                    {
                        command.NewCriteriaParameter("@COL_C_CARGO", SqlDbType.Char, 1, colaborador.Sexo);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Cidade))
                    {
                        command.NewCriteriaParameter("@COL_C_CARGO", SqlDbType.VarChar, 100, colaborador.Cidade);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Estado))
                    {
                        command.NewCriteriaParameter("@COL_C_CARGO", SqlDbType.VarChar, 2, colaborador.Estado);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Nome))
                    {
                        command.NewCriteriaParameter("@CLI_C_NOME", SqlDbType.VarChar, 100, colaborador.Nome);
                    }

                    if (!String.IsNullOrEmpty(colaborador.Senha))
                    {
                        command.NewCriteriaParameter("@CLI_C_SENHA", SqlDbType.VarChar, 50, BLEncriptacao.EncriptarSenha(colaborador.Senha));
                    }

                    if (!String.IsNullOrEmpty(colaborador.Email))
                    {
                        command.NewCriteriaParameter("@CLI_C_EMAIL", SqlDbType.VarChar, 200, colaborador.Email);
                    }

                    if (colaborador.Status.HasValue)
                    {
                        command.NewCriteriaParameter("@CLI_B_STATUS", SqlDbType.Bit, colaborador.Status);
                    }

                    if (colaborador.DataNascimento.HasValue)
                    {
                        command.NewCriteriaParameter("@CLI_D_ANIVERSARIO", SqlDbType.DateTime, colaborador.DataNascimento);
                    }

                    // Execucao
                    Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region ListarPublico
        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        //public static List<MLColaboradorPublico> Listar(MLModuloColaborador filtro, decimal? codigoPagina = null, string nome = null)
        public static List<MLColaboradorPublico> Listar(MLModuloColaborador filtro, decimal? codigoPagina = null, string nome = null)
        {
            using (var command = Database.NewCommand("USP_MOD_COL_L_LISTAR", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, BLPortal.Atual.Codigo);

                if (codigoPagina.HasValue)
                    command.NewCriteriaParameter("@PAG_N_CODIGO", SqlDbType.Decimal, 18, codigoPagina.Value);

                if (!string.IsNullOrEmpty(filtro.LojaLotacao))
                    command.NewCriteriaParameter("@COL_C_LOJA_LOTACAO", SqlDbType.VarChar, 200, filtro.LojaLotacao);

                if (filtro.Restrito.HasValue)
                {
                    command.NewCriteriaParameter("@COL_B_RESTRITO", SqlDbType.Bit, filtro.Restrito.Value);
                    command.NewCriteriaParameter("@COL_C_GRUPO_CLIENTE", SqlDbType.VarChar, -1, filtro.GrupoCliente);
                }

                if (!string.IsNullOrEmpty(nome))
                    command.NewCriteriaParameter("@CLI_C_NOME", SqlDbType.VarChar, 100, nome);

                // Execucao
                return Database.ExecuteReader<MLColaboradorPublico>(command);
            }
        }
        #endregion

        #region ObterColaborador
        /// <summary>
        /// Obtem um colaborador
        /// </summary>
        public static MLColaboradorPublico ObterColaborador(decimal codigo)
        {
            MLColaboradorPublico retorno = null;

            try
            {
                using (var command = Database.NewCommand("USP_MOD_COL_S_OBTER"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CLI_N_CODIGO", SqlDbType.Decimal, 18, codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLColaboradorPublico();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLColaboradorPublico>(dataset.Tables[0].Rows[0]);
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
    }
}
