using System.Collections.Generic;
using System.Net.Mail;

namespace Framework.Utilities
{
    public class MLEmail
    {
        public MLEmail()
        {
            Destinatarios = new List<string>();
            Copia = new List<string>();
            CopiaOculta = new List<string>();
            Anexos = new List<Attachment>();
        }

        /// <summary>
        /// Assunto do e-mail
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        /// Conteúdo do corpo do e-mail, pode conter html
        /// </summary>
        public string Conteudo { get; set; }

        /// <summary>
        /// Lista de e-mail dos destinatários que receberão a mensagem
        /// </summary>
        public List<string> Destinatarios { get; set; }

        /// <summary>
        /// Lista de e-mail que receberão a mensagem em cópia
        /// </summary>
        public List<string> Copia { get; set; }

        /// <summary>
        /// Lista de e-mail que receberão a mensagem em cópia oculta (CCO)
        /// </summary>
        public List<string> CopiaOculta { get; set; }

        /// <summary>
        /// Lista de Attachment que serão enviados na mensagem.
        /// </summary>
        public List<Attachment> Anexos { get; set; }
    }
}
