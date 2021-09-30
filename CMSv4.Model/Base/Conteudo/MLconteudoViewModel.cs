using System.IO;

namespace CMSv4.Model
{
    public class MLConteudoViewModel
    {
        public MLConteudoViewModel()
        {
            Editavel = false;
        }

        public string Nome { get; set; }
        public string Diretorio { get; set; }
        public string Conteudo { get; set; }
        public string AceEditorMode { get; set; }
        public string ArquivoFisico { get; set; }
        public FileInfo FileInfo { get; set; }
        public bool Editavel { get; set; }
    }
}
