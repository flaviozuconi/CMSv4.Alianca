using Framework.Utilities;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public static class ThumbModulos
    {
        public static IDictionary<string, string> Diretorios = new Dictionary<string, string>()
        {
            { "Banner", BLConfiguracao.Pastas.ModuloBanner(BLPortal.Atual.Diretorio) },
            { "Eventos", BLConfiguracao.Pastas.ModuloEvento(BLPortal.Atual.Diretorio) },
            { "ImagemArquivos", BLConfiguracao.Pastas.ModuloArquivosImagens(BLPortal.Atual.Diretorio) },
            { "Listas", BLConfiguracao.Pastas.ModuloListas(BLPortal.Atual.Diretorio) },
            { "Clientes", BLConfiguracao.Pastas.ClientesPortal(BLPortal.Atual.Diretorio) },
            { "Catalogo", BLConfiguracao.Pastas.ModuloGenerico(BLPortal.Atual.Diretorio, "Catalogo") }
        };
    }
}
