using System.Collections.Generic;
using System.Net.Mail;

namespace Framework.Utilities
{
    public class BLEmailBuilder
    {

        private string AssuntoValue { get; set; }
        private List<string> DestinatariosValue { get; set; }
        private List<string> CopiaValue { get; set; }
        private List<string> CopiaOcultaValue { get; set; }
        private string ConteudoValue { get; set; }
        private List<Attachment> AnexosValue { get; set; }
        private bool AsyncValue { get; set; }
        private string SendReplyToValue { get; set; }
        private bool ThrowExceptionValue = true;

        public BLEmailBuilder()
        {
            DestinatariosValue = new List<string>();
            CopiaValue = new List<string>();
            CopiaOcultaValue = new List<string>();
            AnexosValue = new List<Attachment>();
            ThrowExceptionValue = true;
            AsyncValue = false;
        }

        public BLEmailBuilder Assunto(string assuntoValue)
        {
            AssuntoValue = assuntoValue;
            return this;
        }

        public BLEmailBuilder Destinatarios(List<string> destinatariosValue)
        {
            DestinatariosValue = destinatariosValue;
            return this;
        }

        public BLEmailBuilder Destinatarios(string destinatarioValue)
        {
            DestinatariosValue = new List<string>() { destinatarioValue };
            return this;
        }

        public BLEmailBuilder Copia(List<string> copiaValue)
        {
            CopiaValue = copiaValue;
            return this;
        }

        public BLEmailBuilder CopiaOculta(List<string> copiaOcultaValue)
        {
            CopiaOcultaValue = copiaOcultaValue;
            return this;
        }

        public BLEmailBuilder Conteudo(string conteudoValue)
        {
            ConteudoValue = conteudoValue;
            return this;
        }

        public BLEmailBuilder Anexos(List<Attachment> anexosValue)
        {
            AnexosValue = anexosValue;
            return this;
        }

        public BLEmailBuilder SendReplyTo(string sendReplyToValue)
        {
            SendReplyToValue = sendReplyToValue;
            return this;
        }

        public BLEmailBuilder Async(bool asyncValue)
        {
            AsyncValue = asyncValue;
            return this;
        }

        public bool Enviar()
        {
            return BLEmail.Enviar
            (
                AssuntoValue,
                DestinatariosValue,
                CopiaValue,
                CopiaOcultaValue,
                ConteudoValue,
                AnexosValue,   
                SendReplyToValue,
                ThrowExceptionValue,
                string.Empty,
                AsyncValue
            );
        }
    }
}
