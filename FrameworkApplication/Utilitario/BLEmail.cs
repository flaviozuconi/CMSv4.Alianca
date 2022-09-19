using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Linq;

namespace Framework.Utilities
{
    /// <summary>
    /// Funcoes de Envio de Email
    /// </summary>
    public static class BLEmail
    {
        #region Enviar

        /// <summary>
        /// Realizar disparo de e-mails por lote, a cada X envios, aguardar Y minutos
        /// </summary>
        /// <param name="Emails">Lista com as informações dos e-mails a serem enviados</param>
        /// <param name="milesegundosIntervaloEntreLotes">Tempo que o envio ficará em espera até disparar o próximo lote</param>
        /// <param name="quantidadeEmailPorLote">Quantidade de e-mails que serão enviados em cada lote, após o envio, aguardar milesegundosIntervaloEntreLotes.</param>
        /// <param name="Async">bool que define se o envio será assincrono</param>
        /// <returns></returns>
        public static bool EnviarEmLote(List<MLEmail> Emails, int quantidadeEmailPorLote = 999, int milesegundosIntervaloEntreLotes = 120000, bool Async = true)
        {
            if (Emails.Count == 0)
                return false;

            var quantidadeLotes = (int)Math.Ceiling((decimal)Emails.Count / quantidadeEmailPorLote);
            var ultimoLote = quantidadeLotes - 1;

            for (int lote = 0; lote < quantidadeLotes; lote++)
            {
                var emailsLote = Emails.Skip(quantidadeEmailPorLote * lote).Take(quantidadeEmailPorLote).ToList();

                foreach (var item in emailsLote)
                    Enviar(item.Assunto, item.Destinatarios, item.Copia, item.CopiaOculta, item.Conteudo, item.Anexos);

                //Não precisa esperar se for último lote.
                if (lote != ultimoLote)
                    Thread.Sleep(milesegundosIntervaloEntreLotes);
            }

            return true;
        }

        public static bool Enviar(string assunto, string destinatario, string conteudo, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null)
        {
            return Enviar(assunto, new List<string> { destinatario }, null, null, conteudo, new List<string>(), sendReplyTo, throwException, useDefaultConfigFrom);
        }

        public static bool Enviar(string assunto, string destinatario, string copia, string conteudo, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null)
        {
            return Enviar(assunto, new List<string> { destinatario }, new List<string> { copia }, null, conteudo, new List<string>(), sendReplyTo, throwException, useDefaultConfigFrom);
        }

        public static bool Enviar(string assunto, List<string> destinatarios, List<string> copia, List<string> copiaOculta, string conteudo, List<string> anexos, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null)
        {
            var arquivos = new List<Attachment>();
            foreach (var item in anexos)
            {
                var arquivo = HttpContext.Current.Server.MapPath(item);
                if (File.Exists(arquivo))
                {
                    arquivos.Add(new Attachment(arquivo));
                }
            }

            return Enviar(assunto, destinatarios, copia, copiaOculta, conteudo, arquivos, sendReplyTo, throwException, useDefaultConfigFrom);
        }

        public static bool Enviar(string assunto, List<string> destinatarios, List<string> copia, List<string> copiaOculta, string conteudo, HttpFileCollectionBase anexos, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null)
        {
            var arquivos = new List<Attachment>();

            for (int i = 0; i < anexos.Count; i++)
            {
                arquivos.Add(new Attachment(anexos[i].InputStream, anexos[i].FileName));
            }

            return Enviar(assunto, destinatarios, copia, copiaOculta, conteudo, arquivos, sendReplyTo, throwException, useDefaultConfigFrom);
        }

        public static bool Enviar(string assunto, List<string> destinatario, string conteudo, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null)
        {
            return Enviar(assunto, destinatario, null, null, conteudo, new List<string>(), sendReplyTo, throwException, useDefaultConfigFrom);
        }

        /// <summary>
        /// Envia a mensagem por email conforme as configurações de 
        /// SMTP no WEB.CONFIG
        /// </summary>
        /// <param name="anexos">Enviar o caminho virtual dos arquivos</param>
        /// <returns></returns>
        public static bool Enviar(string assunto, List<string> destinatarios, List<string> copia, List<string> copiaOculta, string conteudo, List<Attachment> anexos, string sendReplyTo = "", bool throwException = true, string useDefaultConfigFrom = null, bool async = false)
        {
            try
            {
                if (destinatarios == null || destinatarios.Count == 0) return false;

                var mensagem = new MailMessage();
                var smtp = new SmtpClient();                

                #region SmtpClient
                var _default = useDefaultConfigFrom ?? BLConfiguracao.Obter<string>("SMTP.DEFAULT", "VM2");
                var from = BLConfiguracao.Obter<string>(string.Format("SMTP.{0}.FROM", _default), "&quot;Homologacao VM2&quot; &lt;homologacao@vm2.com.br&gt;");
                var host = BLConfiguracao.Obter<string>(string.Format("SMTP.{0}.HOST", _default), "smtp.vm2.com.br");
                var port = BLConfiguracao.Obter<int>(string.Format("SMTP.{0}.PORT", _default), 587);
                var credentialsUsername = BLEncriptacao.DesencriptarAes(BLConfiguracao.Obter<string>(string.Format("SMTP.{0}.CREDENTIALS.USERNAME", _default), "JHqwvXE3mYQKycOK62jd75Ivlgz6qxbEi9JAGTaI+P4="));
                var credentialsPassword = BLEncriptacao.DesencriptarAes(BLConfiguracao.Obter<string>(string.Format("SMTP.{0}.CREDENTIALS.PASSWORD", _default), "EJXYKfOUkdU25NisEQJmFg=="));                                
                var defaultCredentials = BLConfiguracao.Obter<bool>(string.Format("SMTP.{0}.DEFAULTCREDENTIALS", _default), false);

                mensagem.From = new MailAddress(from);

                smtp.Host = host;
                smtp.Port = port;
                smtp.UseDefaultCredentials = defaultCredentials;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(credentialsUsername, credentialsPassword);


                smtp.Timeout = 999999;
                #endregion

                
                //Reply To
                if (!string.IsNullOrEmpty(sendReplyTo))
                {
                    mensagem.ReplyToList.Add(sendReplyTo);
                }

                #region Destinatarios

                //Destinatario
                foreach (var item in destinatarios)
                {
                    // validar email
                    if (BLUtilitarios.ValidarEmail(item.Trim()))
                    {
                        mensagem.To.Add(new MailAddress(item.Trim()));
                    }
                }

                // Copia
                if (copia != null)
                {
                    foreach (var item in copia)
                    {
                        // validar email
                        if (BLUtilitarios.ValidarEmail(item.Trim()))
                        {
                            mensagem.CC.Add(new MailAddress(item.Trim()));
                        }
                    }
                }


                // Copia Oculta
                if (copiaOculta != null)
                {
                    foreach (var item in copiaOculta)
                    {
                        // validar email
                        if (BLUtilitarios.ValidarEmail(item.Trim()))
                        {
                            mensagem.Bcc.Add(new MailAddress(item.Trim()));
                        }
                    }
                }

                #endregion

                #region Anexos

                if (anexos != null)
                {
                    // Anexos
                    foreach (var item in anexos)
                    {
                        mensagem.Attachments.Add(item);
                    }
                }

                #endregion

                mensagem.Subject = string.IsNullOrEmpty(assunto) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:SS") : assunto;
                mensagem.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

                mensagem.Body = conteudo;
                mensagem.IsBodyHtml = true;

                // Enviar

                // Enviar
                if (async)
                {
                    System.Threading.Tasks.Task.Run(() => Send(mensagem, smtp));
                }
                else
                {
                    smtp.Send(mensagem);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    ApplicationLog.ErrorLog(ex);
                    throw;
                }

                return false;
            }
        }

        private static void Send(MailMessage mensagem, SmtpClient smtp)
        {
            try
            {
                smtp.Send(mensagem);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }

        #endregion

        #region ObterModelo

        /// <summary>
        /// Retorna o conteudo do modelo de email armazenado em um arquivo
        /// Se a extensão não for enviada procura arquivos na extensão HTM
        /// </summary>
        public static string ObterModelo(string nomeModelo)
        {
            return ObterModelo(nomeModelo, "", null);
        }

        public static string ObterModelo(string nomeModelo, HttpContext context)
        {
            return ObterModelo(nomeModelo, "", context);
        }

        /// <summary>
        /// Retorna o conteudo do modelo de email armazenado em um arquivo
        /// Se a extensão não for enviada procura arquivos na extensão HTM
        /// </summary>
        /// <param name="pasta">Indica uma pasta prioritária para procurar o arquivo</param>
        public static string ObterModelo(string nomeModelo, string pasta, HttpContext context)
        {
            const string PastaPadrao = "/content/email/";
            const string PastaPadraoAdmin = "/areas/admin/content/email/";

            if (string.IsNullOrEmpty(Path.GetExtension(nomeModelo))) nomeModelo += ".htm";

            // Verifica conteudo se o usuario enviou caminho virtual completo
            var conteudo = BLUtilitarios.ObterConteudoArquivo(nomeModelo, context);
            if (!string.IsNullOrEmpty(conteudo)) return conteudo;

            // Senao, procura na pasta informada no critério
            conteudo = BLUtilitarios.ObterConteudoArquivo(Path.Combine(pasta, Path.GetFileName(nomeModelo)), context);
            if (!string.IsNullOrEmpty(conteudo)) return conteudo;

            // Senao, procura na pasta de emails do site
            conteudo = BLUtilitarios.ObterConteudoArquivo(Path.Combine(PastaPadrao, Path.GetFileName(nomeModelo)), context);
            if (!string.IsNullOrEmpty(conteudo)) return conteudo;

            // Senao, procura na pasta de emails da area administrativa
            return BLUtilitarios.ObterConteudoArquivo(Path.Combine(PastaPadraoAdmin, Path.GetFileName(nomeModelo)), context);
        }

        #endregion

        /// <summary>
        /// Lista de Modelos Padrão de Emails
        /// </summary>
        public class ModelosPadrao
        {
            public const string AlterarSenha = "/templates/email-usuario-alterar-senha.html";
            public const string AlterarSenhaEnUs = "/templates/email-usuario-alterar-senha-eng.html";
            public const string NovaSenha = "/templates/email-usuario-nova-senha.html";
            public const string EmailCompartilhar = "/templates/email-compartilhar.html";
            public const string EmailConfirmacaoCadastro = "/templates/email-confirmacao.html";
            public const string EmailErroMovidesk = "/templates/email-erro-movidesk.html";
        }
    }

}
