using System;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// Classe para implementação de Módulos do CMS
    /// </summary>
    public abstract class BLModuloBase
    {
        /// <summary>
        /// CMS executa esse método quando um módulo precisar publicar o seu conteúdo
        /// </summary>
        /// <returns>
        /// Se o módulo for estático, retornar o conteúdo HTML.
        /// Se for dinâmico, retornar sintaxe RAZOR para carregamento do módulo
        /// </returns>
        public static string Publicar(string urlModulo, decimal codigoPagina, int repositorio, Guid? codigoHistorico)
        {
            return string.Format(@"
                        @{{ Html.RenderAction(""index"", ""{0}"", new {{ area=""Modulo"", codigoPagina = ""{1}"", repositorio = ""{2}"" }}); }}",
                        urlModulo,
                        codigoPagina,
                        repositorio
                        );
        }

        /// <summary>
        /// CMS executa esse método quando o módulo for trocado de região na página
        /// </summary>
        public static void Mover(decimal codigoPagina, int repositorioOrigem, int repositorioDestino) { }

        /// <summary>
        /// CMS executa esse método quando o módulo precisar copiar de PUBLICACAO para EDICAO.
        /// </summary>
        public static void CarregarConteudo(decimal codigoPagina, int repositorio) { }

        /// <summary>
        /// CMS carrega o conteúdo histórico (quando disponível), substituindo a versão de edição
        /// atual do módulo na página
        /// </summary>
        public static void CarregarHistorico(decimal codigoPagina, int repositorio, Guid codigoHistorico) { }

        /// <summary>
        /// CMS executa esse método quando uma página está sendo duplicada
        /// </summary>
        public static void DuplicarConteudo(decimal codigoPagina, decimal codigoNovaPagina, int repositorio, bool publicado) { }

        /// <summary>
        /// CMS executa esse método quando o usuário escolher exportar o conteúdo da página
        /// </summary>
        public virtual System.Xml.XmlNode ExportarConteudo(decimal codigoPagina, int repositorio) { return null; }

        /// <summary>
        /// CMS executa esse método quando o usuário escolher importar o conteúdo da página 
        /// a partir de um XML
        /// </summary>
        public virtual void ImportarConteudo(decimal codigoPagina, int repositorio, System.Xml.XmlNode conteudo) { }

    }
}
