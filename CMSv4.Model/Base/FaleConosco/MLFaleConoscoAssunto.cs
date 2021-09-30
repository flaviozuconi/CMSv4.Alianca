using Framework.Utilities;
using System;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary>
    /// Lista de assuntos para o Fale Conosco
    /// </summary>
    public class MLFaleConoscoListaAssuntos
    {
        private List<MLFaleConoscoAssunto> _lista = new List<MLFaleConoscoAssunto>();

        public void Add(string assunto, params string[] destinatarios)
        {
            if (destinatarios.Length == 1) _lista.Add(new MLFaleConoscoAssunto { Assunto = assunto, Destinatarios = destinatarios[0] });
            if (destinatarios.Length > 1) _lista.Add(new MLFaleConoscoAssunto { Assunto = assunto, Destinatarios = string.Join(",", destinatarios)});
        }

        public List<MLFaleConoscoAssunto> Lista { get { return _lista; } }
    }

    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLFaleConoscoAssunto
    {
        public string Assunto { get; set; }

        private string _destinatarios;
        public string Destinatarios
        {
            get
            {
                return Assunto + "|" + _destinatarios;
            }
            set
            {
                _destinatarios = BLEncriptacao.EncriptarQueryString(value);
            }
        }

        public string DestinatariosDecifrado
        {
            get
            {
                if (string.IsNullOrEmpty(_destinatarios)) return "";
                return BLEncriptacao.EncriptarQueryString(_destinatarios);
            }
        }
    
    }
}
