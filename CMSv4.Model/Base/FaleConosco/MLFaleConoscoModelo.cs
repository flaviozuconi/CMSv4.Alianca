using System;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLFaleConoscoModelo
    {
        public string Nome { get; set; }

        public string Conteudo { get; set; }

        public bool IsEdicao { get; set; }
    }
}
