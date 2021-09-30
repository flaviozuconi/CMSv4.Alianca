using System;

namespace CMSv4.Model
{
    /// <summary>
    /// ARQUIVO / PASTA
    /// </summary>
    [Serializable]
    public class MLMultimidiaPasta
    {
        public string Nome { get; set; }

        public string CaminhoCompleto { get; set; }

        public int Nivel { get; set; }
    }
}
