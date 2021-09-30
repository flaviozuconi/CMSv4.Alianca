using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using System.Web;
using System.Web.Security;
using Framework.DataLayer;
using Framework.Utilities;

namespace Framework.Utilities
{
    /// <summary>
    /// Usuario
    /// </summary>
    public class BLUsuario
    {
        // LOGIN

        #region Autenticar Usuário

        /// <summary>
        ///     Autenticar o Usuário
        /// </summary>
        public static bool? AutenticarUsuario(string login, string senha)
        {
            MLUsuario objUsuario = null;

            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha)) return false;

                objUsuario = ObterCompletoLogin(login, senha);

                if (objUsuario == null || string.IsNullOrEmpty(objUsuario.Login)) return false;

                FormsAuthentication.SignOut();
                FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("USU" + Convert.ToString(objUsuario.Codigo), false, 60));
                FormsAuthentication.SetAuthCookie("USU" + Convert.ToString(objUsuario.Codigo), false);

                #region Cookie

                //cookie adicional para manter logado o usuário, por causa do relay do servidor, pode perder a autenticação em poucos minutos
                MLCookie authCookie = new MLCookie();

                authCookie.Nome = "CMS_ADMIN_AUX";
                authCookie.Expires = DateTime.Now.AddDays(1);
                authCookie.Valores = new List<MLCookieValores>() { new MLCookieValores() {
                    Chave = "key",
                    Valor = BLEncriptacao.EncriptarAes(objUsuario.Login + "|" + DateTime.Now.AddDays(1).ToString() + "|" + ObterIPUsuario())
                }};

                BLCookie.Criar(authCookie);

                #endregion

                HttpContext.Current.Items["ctxUsuario"] = objUsuario;

                return true;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return false;
        }

        /// <summary>
        /// Realiza a autenticação do usuário por Cookie para quando houver modificação da aplicação (Relay) de cms
        /// </summary>
        private static bool AutenticarUsuarioCookie()
        {
            var cookie = BLCookie.Carregar("CMS_ADMIN_AUX");

            //validar se o cookie existe e tem valor
            if (cookie == null | cookie.Valores.Count == 0 || string.IsNullOrWhiteSpace(cookie.Valores[0].Valor))
                return false;
            
            var value = BLEncriptacao.DesencriptarAes(cookie.Valores[0].Valor).Split('|');

            //formato da chave Login|Data|IP
            if (value == null || value.Length < 3)
                return false;

            var usuario = ObterCompleto(value[0]);

            if (usuario == null || string.IsNullOrEmpty(value[1]))
                return false;

            if (DateTime.Now > Convert.ToDateTime(value[1]))
                return false;

            if (ObterIPUsuario() != value[2])
                return false;

            FormsAuthentication.SignOut();
            FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket("USU" + Convert.ToString(usuario.Codigo), false, 60));
            FormsAuthentication.SetAuthCookie("USU" + Convert.ToString(usuario.Codigo), false);

            HttpContext.Current.Items["ctxUsuario"] = usuario;

            return true;
            
        }

        private static string ObterIPUsuario()
        {
            var context = System.Web.HttpContext.Current;
            var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');

                if (addresses.Length != 0)
                    return addresses[0];
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        #endregion

        #region Autenticar Usuário por Token

        /// <summary>
        ///     Autenticar o Usuário via token de alteração de senha,
        ///     após realizar o login, o token é zerado
        /// </summary>
        public static decimal? AutenticarUsuarioToken(string token, out AutenticacaoTokenStatus retorno)
        {
            retorno = AutenticacaoTokenStatus.Ok;

            var usuarioTokens = CRUD.Listar<MLUsuario>(new MLUsuario { TokenNovaSenha = token });

            if (usuarioTokens == null || usuarioTokens.Count == 0 || !usuarioTokens[0].Codigo.HasValue)
                retorno = AutenticacaoTokenStatus.TokenInvalido;
            else if (!usuarioTokens[0].Ativo.GetValueOrDefault())
                retorno = AutenticacaoTokenStatus.UsuarioInativo;
            else if (!usuarioTokens[0].AlterarSenha.GetValueOrDefault())
                retorno = AutenticacaoTokenStatus.AcessoNegado;

            if (retorno != AutenticacaoTokenStatus.Ok)
                return null;

            return usuarioTokens[0].Codigo;
        }

        #endregion

        #region Obter Completo Login

        /// <summary>
        /// Obtem um objeto de usuario e seus grupos
        /// </summary>
        public static MLUsuario ObterCompleto(string login)
        {
            MLUsuario retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_FWK_S_USUARIO_LOGIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@USU_C_LOGIN", SqlDbType.VarChar, 200, login);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLUsuario();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Preenche dados do menu
                        retorno = Database.FillModel<MLUsuario>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[1].Rows)
                            {
                                retorno.Grupos.Add(Database.FillModel<MLUsuarioItemGrupo>(row));
                            }
                        }

                        if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[2].Rows)
                            {
                                retorno.Funcionalidades.Add(Database.FillModel<MLUsuarioItemFuncionalidade>(row));
                            }
                        }

                        if (dataset.Tables.Count > 3 && dataset.Tables[3].Rows.Count > 0)
                        {
                            // Preenche itens do menu
                            foreach (DataRow row in dataset.Tables[3].Rows)
                            {
                                retorno.Portais.Add(Database.FillModel<MLUsuarioItemPortal>(row));
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

        /// <summary>
        /// Obtem um objeto de usuario e seus grupos
        /// </summary>
        public static MLUsuario ObterCompletoLogin(string login, string senha)
        {
            MLUsuario retorno = null;

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

        #region Efetuar Logoff

        /// <summary>
        ///     Efetua o logoff do usuário
        /// </summary>
        public static void Logoff()
        {
            FormsAuthentication.SignOut();
            if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session.Clear();

            var cookie = BLCookie.Carregar("CMS_ADMIN_AUX");

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-5d);
                cookie.Valores = new List<MLCookieValores>();

                BLCookie.Criar(cookie);
            }
                
        }

        #endregion

        // OBTER

        #region Obter Completo

        /// <summary>
        /// Obtem um objeto de usuario e seus grupos
        /// </summary>
        public static MLUsuario ObterCompleto(decimal? codigo)
        {
            MLUsuario retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_FWK_S_USUARIO_COMPLETO"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@USU_N_CODIGO", SqlDbType.Decimal, 18, codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLUsuario();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Model
                        retorno = Database.FillModel<MLUsuario>(dataset.Tables[0].Rows[0]);

                        if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                        {
                            // Grupos
                            foreach (DataRow row in dataset.Tables[1].Rows)
                            {
                                retorno.Grupos.Add(Database.FillModel<MLUsuarioItemGrupo>(row));
                            }
                        }

                        if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                        {
                            // Funcionalidade
                            foreach (DataRow row in dataset.Tables[2].Rows)
                            {
                                retorno.Funcionalidades.Add(Database.FillModel<MLUsuarioItemFuncionalidade>(row));
                            }
                        }

                        if (dataset.Tables.Count > 3 && dataset.Tables[3].Rows.Count > 0)
                        {
                            // Portal
                            foreach (DataRow row in dataset.Tables[3].Rows)
                            {
                                retorno.Portais.Add(Database.FillModel<MLUsuarioItemPortal>(row));
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

        #region Obter Logado

        /// <summary>
        ///     Retorna o usuário logado atualmente no sistema
        /// </summary>
        /// <returns>Usuário Logado</returns>
        public static MLUsuario ObterLogado()
        {
            ArmazenarLogado();

            if (HttpContext.Current.Request.IsAuthenticated && HttpContext.Current.Items["ctxUsuario"] != null && HttpContext.Current.User.Identity.Name.StartsWith("USU"))
            {
                return (MLUsuario)HttpContext.Current.Items["ctxUsuario"];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Armazenar Logado

        /// <summary>
        ///     Armazena o usuário logado no contexto da aplicacao
        /// </summary>
        public static void ArmazenarLogado()
        {
            // Se não estiver autenticado, não faz nada
            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                //Quando o usuário perder a sessão, verificar se existe cookie adicional.
                //Pode ocorrer a troca do servidor da aplicação (relay), evitar perda de sessão.

                //Em teste, comentado para não subir em produção.
                //var autenticado = AutenticarUsuarioCookie();

                //if(!autenticado)
                    return;
            }

            // Armazena usuário no contexto e na sessão

            decimal codigoUsuarioLogado;

            if (HttpContext.Current.User.Identity.Name.StartsWith("USU"))
            {
                codigoUsuarioLogado = Convert.ToInt32(HttpContext.Current.User.Identity.Name.Replace("USU", ""));
                MLUsuario objUsuario = null;
                var nomeSession = "ctxUsuario" + codigoUsuarioLogado.ToString();

                if (HttpContext.Current.Session != null
                    && HttpContext.Current.Session[nomeSession] != null) objUsuario = (MLUsuario)HttpContext.Current.Session[nomeSession];

                if (HttpContext.Current.Items["ctxUsuario"] == null && objUsuario != null) HttpContext.Current.Items["ctxUsuario"] = objUsuario;

                if (objUsuario == null)
                {
                    objUsuario = ObterCompleto(codigoUsuarioLogado);

                    HttpContext.Current.Items["ctxUsuario"] = objUsuario;
                    if (HttpContext.Current.Session != null) HttpContext.Current.Session[nomeSession] = objUsuario;
                }
            }
        }

        #endregion

        // ATUALIZAR

        #region Atualizar com Grupos

        /// <summary>
        /// Atualizar
        /// </summary>
        public static void Salvar(MLUsuario model)
        {
            if (!model.Codigo.HasValue)
            {
                model.Senha = Guid.NewGuid().ToString().Split('-')[0];
                model.DataCadastro = DateTime.Now;
                model.AlterarSenha = true;
            }

            using (var scope = new TransactionScope())
            {
                var codigo = CRUD.Salvar<MLUsuario>(model);

                CRUD.Excluir<MLUsuarioItemGrupo>("CodigoUsuario", codigo);

                foreach (var item in model.Grupos)
                {
                    if (item.UsuarioAssociado.HasValue && item.UsuarioAssociado.Value)
                    {
                        var novoItem = new MLUsuarioGrupo
                        {
                            CodigoUsuario = codigo,
                            CodigoGrupo = item.CodigoGrupo
                        };

                        CRUD.Salvar<MLUsuarioGrupo>(novoItem);
                    }
                }

                CRUD.Excluir<MLUsuarioItemPortal>("CodigoUsuario", codigo);

                foreach (var item in model.Portais)
                {
                    if (item.UsuarioAssociado.HasValue && item.UsuarioAssociado.Value)
                    {
                        var novoItem = new MLUsuarioItemPortal
                        {
                            CodigoUsuario = codigo,
                            CodigoPortal = item.CodigoPortal
                        };

                        CRUD.Salvar(novoItem);
                    }
                }


                scope.Complete();
            }
        }

        #endregion

        // APROVADORES
        #region Listar Aprovadores
        /// <summary>
        /// Obtem os usuários de um grupo
        /// </summary>
        public static List<MLUsuario> ListarAprovadores(decimal codigoPortal, decimal codigoGrupo)
        {
            
            try
            {
                using (var command = Database.NewCommand("USP_ADM_L_APROVADORES"))
                {
                    command.NewCriteriaParameter("@POR_N_CODIGO", codigoPortal);
                    command.NewCriteriaParameter("@GRP_N_CODIGO", codigoGrupo);
                    return Database.ExecuteReader<MLUsuario>(command);
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
