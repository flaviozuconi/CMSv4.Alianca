using System;
using System.Collections.Generic;
using System.Web;
using Framework.Utilities;
using System.Transactions;
using Framework.DataLayer;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public class BLPortalInstalacao
    {
        #region InserirSeqInstalacao
        public static void InserirSeqInstalacao(decimal portal, List<MLPortalInstalacao.ETAPAS> etapas)
        {
            foreach (var i in etapas)
            {

                CRUD.Salvar<MLPortalInstalacao>(
                    new MLPortalInstalacao
                    {
                        CodigoPortal = portal,
                        Etapa = (int)i,
                        Status = false
                    }
               );
            }
        }
        #endregion

        #region GravarLogInstalacao
        public static void GravarLogInstalacao(MLPortalInstalacao logInicial, bool status, string msg)
        {
            logInicial.Status = status;
            logInicial.Mensagem = msg;
            logInicial.DataTermino = DateTime.Now;
            CRUD.SalvarParcial<MLPortalInstalacao>(logInicial);
        }
        #endregion

        #region CriarBancoDados
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomebanco"></param>
        /// <param name="connectionString"></param>
        public static bool CriarBancoDados(MLPortalInstalacao logInstalacao, string nomebanco, string connectionString)
        {
            try
            {
                var retorno = new List<MLPortalFile>();

                using (var command = Database.NewCommand("sp_helpfile", connectionString))
                {
                    // Execucao
                    retorno = Database.ExecuteReader<MLPortalFile>(command);
                }
                if (retorno.Count > 0)
                {
                    var data = retorno.Find(o => o.FileGroup == "PRIMARY");
                    var log = retorno.Find(o => o.FileGroup != "PRIMARY");

                    if (data != null)
                    {
                        var diretoriodata = Path.GetDirectoryName(data.FileName);
                        var diretoriolog = Path.GetDirectoryName(data.FileName);
                        if (log != null)
                            diretoriolog = Path.GetDirectoryName(log.FileName);

                        var templateCreateDatabase = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.ScriptCreateDataBase);

                        using (var reader = new StreamReader(templateCreateDatabase, Encoding.GetEncoding("ISO8859-1")))
                        {
                            var script = reader.ReadToEnd();
                            reader.Close();

                            script = script.Replace("#diretorio-data#", diretoriodata);
                            script = script.Replace("#diretorio-log#", diretoriolog);
                            script = script.Replace("#nome-banco#", nomebanco);

                            using (var command = Database.NewCommand("", connectionString))
                            {
                                // Execucao
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandTimeout = 180;

                                command.CommandText = script;
                                Database.ExecuteNonQuery(command);
                                GravarLogInstalacao(logInstalacao, true, "Banco criado com sucesso");
                                return true;
                            }
                        }
                    }
                    GravarLogInstalacao(logInstalacao, false, "Caminho do Arquivo MDF não encontrado no Banco de Dados");
                    return false;
                }
                GravarLogInstalacao(logInstalacao, false, "Caminho dos Arquivos MDF e LDF não encontrado no Banco de Dados");
            }
            catch (Exception ex)
            {
                GravarLogInstalacao(logInstalacao, false, string.Format("Erro na criação do Banco: {0}", ex.Message));
                ApplicationLog.ErrorLog(ex);
                return false;
            }
            return false;
        }
        #endregion

        #region CriarEstrutura
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomebanco"></param>
        /// <param name="connectionString"></param>
        public static bool CriarEstrutura(MLPortalInstalacao logInstalacao, string connectionString)
        {
            try
            {
                var gerarestrutura = false;
                using (var command = Database.NewCommand("SELECT NAME FROM sysobjects where name = 'CMS_POR_PORTAL'", connectionString))
                {
                    // Execucao
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 180;

                    var exists = Database.ExecuteScalar(command);
                    gerarestrutura = exists == null;
                }
                if (gerarestrutura)
                {
                    using (var scope = new TransactionScope(connectionString))
                    {

                        //Criar Tabelas
                        var templateCreateDatabase = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.ScriptTables);

                        using (var reader = new StreamReader(templateCreateDatabase, Encoding.GetEncoding("ISO8859-1")))
                        {
                            var script = reader.ReadToEnd();
                            reader.Close();

                            using (var command = Database.NewCommand("", connectionString))
                            {
                                // Execucao
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandTimeout = 180;

                                command.CommandText = script.Replace("\r\nGO\r\n", "\r\n");
                                Database.ExecuteNonQuery(command);

                            }
                        }

                        //Criar Procedures
                        var templateProc = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.ScriptProcedures);
                        using (var reader = new StreamReader(templateProc, Encoding.GetEncoding("ISO8859-1")))
                        {
                            var script = reader.ReadToEnd();
                            reader.Close();

                            using (var command = Database.NewCommand("", connectionString))
                            {
                                // Execucao
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandTimeout = 180;

                                command.CommandText = script.Replace("\r\nGO\r\n", "\r\n");
                                Database.ExecuteNonQuery(command);
                            }
                        }
                        scope.Complete();
                    }
                    GravarLogInstalacao(logInstalacao, true, "Estrutura do Banco de Dados criada com sucesso");
                    return true;
                }
                else
                {

                    using (var command = Database.NewCommand("SELECT POR_N_CODIGO FROM CMS_POR_PORTAL WHERE POR_N_CODIGO = @POR_N_CODIGO", connectionString))
                    {
                        // Execucao
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandTimeout = 180;

                        command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, logInstalacao.CodigoPortal.Value);

                        var exists = Database.ExecuteScalar(command);

                        if (exists != null)
                        {
                            GravarLogInstalacao(logInstalacao, false, "Codigo do portal já existe no campo de conteúdo");
                            return false;
                        }
                        GravarLogInstalacao(logInstalacao, true, "Estrutura já estava criada.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                GravarLogInstalacao(logInstalacao, false, string.Format("Erro na execução dos scripts de estrutura: {0}", ex.Message));
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region CriarDados
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomebanco"></param>
        /// <param name="connectionString"></param>
        public static bool CriarDados(decimal codigoportal, string connectionString)
        {
            try
            {
                //Criar Tabelas
                var templateCreateDatabase = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.ScriptDados);

                using (var reader = new StreamReader(templateCreateDatabase, Encoding.GetEncoding("ISO8859-1")))
                {
                    var script = reader.ReadToEnd();
                    reader.Close();

                    using (var command = Database.NewCommand("", connectionString))
                    {
                        // Execucao
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandTimeout = 180;

                        command.CommandText = script.Replace("\r\nGO\r\n", "\r\n");
                        Database.ExecuteNonQuery(command);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region InserirMenusDefault
        public static bool InserirMenusDefault(MLPortalInstalacao logInstalacao, string connectionString)
        {
            try
            {

                //Insere no banco ADM os menus Padrões

                CRUD.Excluir<MLMenuItem>("CodigoPortal", logInstalacao.CodigoPortal.Value);

                var menus = CRUD.Listar(new MLMenuItem { PadraoParaPortal = true });
                //foreach (var item in menus)
                //{
                //    CRUD.Salvar(new Framework.Utilities.MLMenuItemPortal
                //    {
                //        CodigoPortal = logInstalacao.CodigoPortal,
                //        CodigoMenu = item.CodigoMenu
                //    });
                //}
                GravarLogInstalacao(logInstalacao, true, "Menus inseridos");
                return true;
            }
            catch (Exception ex)
            {
                GravarLogInstalacao(logInstalacao, false, "Erro ao inserir menus: " + ex.Message);
                ApplicationLog.ErrorLog(ex);
                return false;
            }

        }
        #endregion

        #region CriarConteudo
        public static bool CriarConteudo(MLPortalInstalacao logInstalacao, MLPortal portal)
        {
            try
            {
                var dirConteudo = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.PastaConteudo);
                var file = Path.Combine(dirConteudo, string.Format("{0}.zip", logInstalacao.CodigoPortal));
                var filePadrao = false;

                if (!File.Exists(file))
                {
                    file = HttpContext.Current.Server.MapPath(BLConfiguracao.Instalacao.ArquivoConteudoDefault);

                    if (!File.Exists(file))
                    {
                        GravarLogInstalacao(logInstalacao, false, string.Format("Arquivo padrão não existe: {0}", BLConfiguracao.Instalacao.ArquivoConteudoDefault));
                        return false;
                    }
                    filePadrao = true;
                }
                var diretorioPortal = HttpContext.Current.Server.MapPath("/portal/" + portal.Diretorio);
                if (!Directory.Exists(diretorioPortal)) Directory.CreateDirectory(diretorioPortal);

                var descompactarOK = BLUtilitarios.DescompactarZip(file, diretorioPortal);

                if (!descompactarOK)
                {
                    GravarLogInstalacao(logInstalacao, false, string.Format("Erro ao descompactar o Arquivo: ", file));
                    return false;
                }

                if (filePadrao)
                {
                    GravarLogInstalacao(logInstalacao, true, "Conteúdo criado utilizando arquivo padrão.");
                    return true;
                }
                GravarLogInstalacao(logInstalacao, true, "Conteúdo criado utilizando arquivo próprio.");
                return true;

            }
            catch (Exception ex)
            {
                GravarLogInstalacao(logInstalacao, false, string.Format("Erro ao criar conteúdo: {0}", ex.Message));
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region ConnectionTest
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomebanco"></param>
        /// <param name="connectionString"></param>
        public static bool ConnectionTest(string connectionString)
        {
            try
            {
                //Criar Tabelas
                using (var command = Database.NewCommand("select getdate()", connectionString))
                {
                    // Execucao
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 180;

                    var obj = Database.ExecuteScalar(command);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ConnectionTestMaster
        /// <summary>
        /// 
        /// </summary>      
        public static string ConnectionTestMaster(string connectionString, string nomebanco)
        {
            try
            {
                using (var command = Database.NewCommand("select name from sysdatabases where name = @dbname", connectionString))
                {
                    // Execucao
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 180;

                    command.NewCriteriaParameter("@dbname", SqlDbType.VarChar, nomebanco);

                    var nome = Database.ExecuteScalar(command);

                    return nome == null ? "" : "Banco de Dados já existe na instância";
                }
            }
            catch
            {
                return "Esse usuário não tem permissão para criar novos bancos de dados";
            }
        }
        #endregion

        public delegate void MethodInvoker(decimal codigo, string tipo);

        public static void InstalarPortal(decimal codigo, string tipo)
        {
            // sleep for 1,6 min.  
            Thread.Sleep(30000);
            // conteudo do processamento assíncrono  

            try
            {
                var portal = CRUD.Obter<MLPortal>(codigo);
                var connectionstring = "server={0};uid={1};pwd={2};database={3}";
                var connConteudo = string.Format(connectionstring, portal.ServidorBD, portal.UsuarioBD, portal.SenhaBD, portal.NomeBD);
                var continua = true;


                var etapas = CRUD.Listar<MLPortalInstalacao>(new MLPortalInstalacao { CodigoPortal = codigo });

                var etapaCriarBanco = etapas.Find(o => o.Etapa == (int)MLPortalInstalacao.ETAPAS.CRIARBD);

                if (etapaCriarBanco != null && !etapaCriarBanco.Status.Value)
                {
                    continua = CriarBancoDados(etapaCriarBanco, portal.NomeBD, string.Format(connectionstring, portal.ServidorBD, portal.UsuarioBD, portal.SenhaBD, "master"));
                    if (!continua) return;
                }

                var etapaCriarEstrutura = etapas.Find(o => o.Etapa == (int)MLPortalInstalacao.ETAPAS.CRIARESTRUTURA);
                if (etapaCriarEstrutura != null && !etapaCriarEstrutura.Status.Value)
                {
                    continua = CriarEstrutura(etapaCriarBanco, connConteudo);
                    if (!continua) return;
                }

                /*var etapaGruposDefault = etapas.Find(o => o.Etapa == (int)MLPortalInstalacao.ETAPAS.GRUPOSDEFAULT);
                if (etapaGruposDefault != null && !etapaGruposDefault.Status.Value)
                {
                    InserirGruposDefault(etapaGruposDefault, connConteudo);
                }*/

                var etapaMenusDefault = etapas.Find(o => o.Etapa == (int)MLPortalInstalacao.ETAPAS.MENUSDEFAULT);
                if (etapaMenusDefault != null && !etapaMenusDefault.Status.Value)
                {
                    InserirMenusDefault(etapaMenusDefault, connConteudo);
                }

                var etapaConteudo = etapas.Find(o => o.Etapa == (int)MLPortalInstalacao.ETAPAS.CONTEUDO);
                if (etapaConteudo != null && !etapaConteudo.Status.Value)
                {
                    continua = CriarConteudo(etapaConteudo, portal);
                    if (!continua) return;
                }

                CRUD.SalvarParcial<MLPortal>(new MLPortal { Codigo = codigo, Manutencao = false });

                //Enviar email finalização do Portal

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

        }

        public static void IniciarInstalacao(decimal codigo, string tipo)
        {
            // create a delegate of MethodInvoker poiting to Executar  
            MethodInvoker simpleDelegate = new MethodInvoker(InstalarPortal);
            // Calling Executar Async  
            simpleDelegate.BeginInvoke(codigo, tipo, null, null);
        }
    }
}