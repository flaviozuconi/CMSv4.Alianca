using System;
using System.Collections.Generic;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;
using System.Web;
using System.Linq;
using System.Transactions;

namespace CMSv4.BusinessLayer
{
    public class BLCliente : BLCRUD<MLCliente>
    {
        private MLCliente Cliente { get; set; }
        private string NovaSenha { get; set; }
        private bool IsNovoRegistro { get; set; }
        private string ListaCodigoGrupo { get; set; }

        #region CRUD

        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            using (TransactionScope scope = new TransactionScope())
            {
               
                foreach (var item in ids)
                    CRUD.Excluir<MLClienteItemGrupo>("CodigoCliente", Convert.ToDecimal(item), PortalAtual.ConnectionString);

                base.Excluir(ids, connectionString);

                scope.Complete();
            }

            return 1;
        }

        #endregion

        #region Salvar

        public override decimal Salvar(MLCliente model, string listaCodigoGrupo)
        {
            InicializarVariaveis(model, listaCodigoGrupo);

            ConfiguracaoParaNovoRegistro();

            using (var scope = new TransactionScope())
            {
                SalvarCliente();

                SalvarGrupos();

                scope.Complete();
            }

            EnviarEmailParaNovoRegistro();

            return Cliente.Codigo.GetValueOrDefault(0);
        }
    
        private void InicializarVariaveis(MLCliente model, string listaCodigoGrupo)
        {
            Cliente = model;
            NovaSenha = BLUtilitarios.GetNewPassword();
            IsNovoRegistro = !model.Codigo.HasValue;
            ListaCodigoGrupo = listaCodigoGrupo;
        }

        private void ConfiguracaoParaNovoRegistro()
        {
            if (IsNovoRegistro)
            {
                Cliente.Senha = BLEncriptacao.EncriptarSenha(NovaSenha);
                Cliente.DataCadastro = DateTime.Now;
                Cliente.AlterarSenha = true;
            }
        }

        private void SalvarCliente()
        {
            Cliente.Ativo = Cliente.Ativo.GetValueOrDefault(false);
            Cliente.Codigo = CRUD.Salvar(Cliente);
        }

        private void SalvarGrupos()
        {
            if (!string.IsNullOrWhiteSpace(ListaCodigoGrupo))
            {
                var connectionString = PortalAtual.ConnectionString;
                var codigosIncluir = ListaCodigoGrupo.Split(',').ToList();

                CRUD.Excluir(new MLClienteItemGrupo() { CodigoCliente = Cliente.Codigo });

                foreach (var item in codigosIncluir)
                    Cliente.Grupos.Add(new MLClienteItemGrupo() {
                        CodigoCliente = Cliente.Codigo,
                        CodigoGrupo = Convert.ToDecimal(item)
                    });

                CRUD.Salvar(Cliente.Grupos, connectionString);
            }
        }

        private void EnviarEmailParaNovoRegistro()
        {
            if (IsNovoRegistro)
            {
                var tradutor = new BLTraducao();
                var emailBuilder = new BLEmailBuilder();
                var htmlEmail = ObterModeloDeEmailParaNovaSenha();

                emailBuilder
                    .Assunto(tradutor.ObterAdm("Nova Senha"))
                    .Destinatarios(Cliente.Email)
                    .Conteudo(htmlEmail)
                    .Async(true)
                    .Enviar();
            }
        }

        private string ObterModeloDeEmailParaNovaSenha()
        {
            return BLEmail.ObterModelo(BLEmail.ModelosPadrao.NovaSenha)
                .Replace("[[nome]]", Cliente.Nome)
                .Replace("[[senha]]", NovaSenha);
        }

        #endregion

        #endregion

        #region Obter Completo Login



        /// <summary>
        /// Obtem um objeto de usuario e seus grupos
        /// </summary>
        public static MLClienteCompleto ObterCompleto(string login)
        {
            MLClienteCompleto retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados
                using (var command = Database.NewCommand("USP_CMS_S_CLIENTE_LOGIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CLI_C_LOGIN", SqlDbType.VarChar, 200, login.Trim());

                    // Execucao
                    retorno = LerDataSetObterCliente(Database.ExecuteDataSet(command));
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }

        public static MLClienteCompleto ObterCompleto(decimal Codigo)
        {
            MLClienteCompleto retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados
                using (var command = Database.NewCommand("USP_CMS_S_CLIENTE_COMPLETO"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@CLI_N_CODIGO", SqlDbType.Decimal, 18, Codigo);

                    // Execucao
                    retorno = LerDataSetObterCliente(Database.ExecuteDataSet(command));
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }

        public static MLClienteCompleto LerDataSetObterCliente(DataSet dataset)
        {
            MLClienteCompleto retorno = new MLClienteCompleto();

            if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                // Preenche dados do menu
                retorno = Database.FillModel<MLClienteCompleto>(dataset.Tables[0].Rows[0]);

                if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    retorno.Grupos = Database.FillList<MLClienteItemGrupo>(dataset.Tables[1]);
            }

            return retorno;
        }

        /// <summary>
        /// Obtem um objeto de usuario e seus grupos
        /// </summary>
        public static MLCliente ObterCompletoLogin(string login, string senha)
        {
            MLCliente retorno = null;

            try
            {
                retorno = ObterCompleto(login);

                if (!retorno.Ativo.HasValue) return null;
                if (retorno.Ativo.Value == false) return null;

                if (retorno.Senha != BLEncriptacao.EncriptarSenha(senha))
                {
                    return null;
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

        #region Listar Admin

        /// <summary>
        /// Quantidade de clientes cadastrados no portal
        /// Lista dos últimos clientes que realizaram login na área pública.
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public static List<MLClienteGrid> ListarAdmin
        (
            string buscaGenerica,
            decimal? codigoPortal,

            string orderBy,
            string sortOrder,
            int start,
            int length
        )
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_CMS_L_CLIENTES_ADMIN"))
            {
                // Parametros
                command.NewCriteriaParameter("@CRITERIO", SqlDbType.VarChar, -1, buscaGenerica);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, codigoPortal);

                command.NewCriteriaParameter("@ORDERBY", SqlDbType.VarChar, 100, orderBy);
                command.NewCriteriaParameter("@ASC", SqlDbType.Bit, (sortOrder == "asc"));
                command.NewCriteriaParameter("@START", SqlDbType.Int, start);
                command.NewCriteriaParameter("@LENGTH", SqlDbType.Int, length);

                return Database.ExecuteReader<MLClienteGrid>(command);
            }
        }

        #endregion

        #region ListarClientesPorGrupo

        public static List<MLCliente> ListarClientesPorGrupo(decimal CodigoGrupo)
        {
            using (var command = Database.NewCommand("USP_CMS_L_CLIENTE_CONVIDADO", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@GCL_N_CODIGO", SqlDbType.Decimal, 18, CodigoGrupo);

                return Database.ExecuteReader<MLCliente>(command);
            }
        }

        #endregion

        #region Enviar Nova Senha

        /// <summary>
        /// Gerar nova senha para o cliente e enviar por e-mail
        /// </summary>
        /// <param name="id">Código do cliente (PK).</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <returns>bool</returns>
        public static bool EnviarNovaSenha(decimal id, string assunto)
        {
            try
            {
                var model = CRUD.Obter<MLCliente>(id);
                string senha = BLUtilitarios.GetNewPassword();

                model.AlterarSenha = true;
                model.Senha = BLEncriptacao.EncriptarSenha(senha);

                CRUD.SalvarParcial(model);

                // enviar email
                return BLEmail.Enviar(assunto, model.Email, BLEmail.ObterModelo(BLEmail.ModelosPadrao.NovaSenha)
                    .Replace("[[nome]]", model.Nome)
                    .Replace("[[senha]]", senha)
                );
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }

        #endregion

        #region Obter Logado

        /// <summary>
        ///     Retorna o usuário logado atualmente no sistema
        /// </summary>
        /// <returns>Usuário Logado</returns>
        public static MLClienteCompleto ObterLogado(HttpContext context = null)
        {
            BLClienteAutenticacao.ArmazenarLogado(context);

            if (context == null)
                context = HttpContext.Current;


            if (context.Request.IsAuthenticated && context.Items["ctxUsuarioPublico"] != null && context.User.Identity.Name.StartsWith("CLI"))
            {
                return (MLClienteCompleto)context.Items["ctxUsuarioPublico"];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
