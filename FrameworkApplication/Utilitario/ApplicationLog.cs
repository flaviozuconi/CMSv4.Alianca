using System;
using System.Data;
using Framework.DataLayer;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utilities
{
    /// <summary>
    /// Armazena informações da aplicação para auditoria e log
    /// </summary>
    public class ApplicationLog
    {
        #region ErrorLog

        /// <summary>
        /// Log de erros
        /// </summary>
        public static void ErrorLog(Exception ex)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_I_LOG_ERRO"))
                {
                    var sb = new StringBuilder();

                    // Parametros
                    var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    var stackTrace = ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace;

                    command.NewCriteriaParameter("@LOG_C_ERRO", SqlDbType.VarChar, 255, message);
                    command.NewCriteriaParameter("@LOG_C_STACK", SqlDbType.VarChar, -1, stackTrace);

                    // Execucao
                    Database.ExecuteScalar(command);

                    #region Enviar Email
                    //Está acontecendo um flood spam no fim de semana e foi pedido para retirar isso
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                    {
                        try
                        {
                            sb.Append(string.Concat("-<u>Portal</u>: ", BLPortal.Atual.Diretorio));
                            sb.Append("<br/>");
                        }
                        catch { }

                        try
                        {
                            sb.Append(string.Concat("-<u>URL</u>: ", System.Web.HttpContext.Current.Request.Url.OriginalString));
                            sb.Append("<br/>");
                        }
                        catch { }

                        try
                        {
                            var browser = System.Web.HttpContext.Current.Request.Browser;

                            sb.Append("-<u>Browser</u>: <table border=\"1\" cellspacing=\"3\" cellpadding=\"2\">");

                            sb.Append("<tr><td>browser</td><td>" + browser["browser"] + "</td></tr>");
                            sb.Append("<tr><td>layoutEngine</td><td>" + browser["layoutEngine"] + "</td></tr>");
                            sb.Append("<tr><td>version</td><td>" + browser["version"] + "</td></tr>");
                            sb.Append("<tr><td>type</td><td>" + browser["type"] + "</td></tr>");

                            sb.Append("</table>");
                            sb.Append("<br/>");
                        }
                        catch { }

                        try
                        {
                            var form = System.Web.HttpContext.Current.Request.Form;
                            if (form != null && form.AllKeys.Length > 0)
                            {
                                sb.Append("-<u>Data</u>: ");
                                foreach (var data in form.AllKeys)
                                {
                                    sb.Append(string.Concat(data, ": ", form[data], ", "));
                                }

                                sb.Append("<br/>");
                            }
                        }
                        catch { }

                        sb.Append("-<u>Exception</u>:");
                        sb.Append("<br/>");
                        sb.Append(string.Concat("<b>", ex.Message, "</b>"));
                        sb.Append("<br/><br/>");
                        sb.Append(ex.StackTrace);

                        if (ex.InnerException != null)
                        {
                            sb.Append("<br/><br/>");
                            sb.Append("-<u>InnerException</u>:");
                            sb.Append("<br/>");
                            sb.Append(string.Concat("<b>", ex.InnerException.Message, "</b>"));
                            sb.Append("<br/><br/>");
                            sb.Append(ex.InnerException.StackTrace);
                        }

                        eet = new EnvioEmailDelegate(EnvioEmail);
                        eet.BeginInvoke(sb.ToString(), null, null);
                    }
                    #endregion
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Log

        /// <summary>
        /// Gravar Log similar à console.log();
        /// </summary>
        public static void Log(string mensagem)
        {
            try
            {
                using (var command = Database.NewCommand("USP_FWK_I_LOG_ERRO"))
                {
                    var sb = new StringBuilder();

                    // Parametros
                    command.NewCriteriaParameter("@LOG_C_ERRO", SqlDbType.VarChar, 255, "Log");
                    command.NewCriteriaParameter("@LOG_C_STACK", SqlDbType.VarChar, -1, mensagem);

                    // Execucao
                    Database.ExecuteScalar(command);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region EnvioEmail
        private delegate void EnvioEmailDelegate(string mensagem);
        private static EnvioEmailDelegate eet;

        private static void EnvioEmail(string mensagem)
        {
            try
            {
                string assuntoWebconfig = BLConfiguracao.Obter<string>("CMS.LogErro.Assunto", "Log de Erros");
                string assunto = string.Format("{0} - {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), assuntoWebconfig);
                string destinatario = BLConfiguracao.Obter<string>("CMS.LogErro.Destinatario", "homologacao@vm2.com.br");

                if (mensagem.IndexOf("http://localhost") > -1) //identificar no log quando for email de log de erro vindo local
                {
                    assunto = string.Concat("[LOCALHOST] ", assuntoWebconfig);
                }

                if (destinatario.IndexOf(",") > -1)
                {
                    BLEmail.Enviar(assunto, destinatario.Split(',').ToList(), new List<string>(), new List<string>(), mensagem, new List<string>(), throwException: false, useDefaultConfigFrom: "VM2");
                }
                else
                {
                    BLEmail.Enviar(assunto, destinatario, mensagem, throwException: false, useDefaultConfigFrom: "VM2");
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Listar

        public static List<MLLogErro> Listar(int? pagina, int? quantidade)
        {
            using (var command = Database.NewCommand("USP_FWK_L_LOG_ERRO"))
            {
                // Parametros
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, quantidade);
                command.NewCriteriaParameter("@PAGINA", SqlDbType.Int, pagina);

                // Execucao
                return Database.ExecuteReader<MLLogErro>(command);
            }
        }

        #endregion

        #region ExcluirLogs
        public static void ExcluirLogs()
        {
            using (var command = Database.NewCommand("USP_FWK_D_LOG_ERRO"))
            {
                // Execucao
                Database.ExecuteNonQuery(command);
            }
        }
        #endregion
    }
}
