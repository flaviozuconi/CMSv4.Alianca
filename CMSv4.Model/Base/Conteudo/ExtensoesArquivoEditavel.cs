using System.Collections.Generic;

namespace CMSv4.Model.Base.Conteudo
{
    public static class ArquivosEditaveis
    {
        public static readonly List<TipoArquivo> Tipos = new List<TipoArquivo>() {
            new TipoArquivo() { Extensao = ".css",    AceEditorMode = AdminCommon.AceEditorModes.CSS },
            new TipoArquivo() { Extensao = ".js",     AceEditorMode = AdminCommon.AceEditorModes.JavaScript },
            new TipoArquivo() { Extensao = ".xml",    AceEditorMode = AdminCommon.AceEditorModes.XML },
            new TipoArquivo() { Extensao = ".config", AceEditorMode = AdminCommon.AceEditorModes.XML },
            new TipoArquivo() { Extensao = ".htm",    AceEditorMode = AdminCommon.AceEditorModes.HTML },
            new TipoArquivo() { Extensao = ".html",   AceEditorMode = AdminCommon.AceEditorModes.HTML },
            new TipoArquivo() { Extensao = ".cshtml", AceEditorMode = AdminCommon.AceEditorModes.CSharp },
            new TipoArquivo() { Extensao = ".cs",     AceEditorMode = AdminCommon.AceEditorModes.CSharp },
            new TipoArquivo() { Extensao = ".sql",     AceEditorMode = AdminCommon.AceEditorModes.SQL },
            new TipoArquivo() { Extensao = ".txt",    AceEditorMode = AdminCommon.AceEditorModes.Text },
        };
    }

    public struct TipoArquivo
    {
        public string Extensao { get; set; }

        public string AceEditorMode { get; set; }
    }
}
