using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    [Serializable]
    public class MLModuloHtml : ISearchable
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("HTM_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("HTM_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("HTM_C_CONTEUDO_BUSCAR", SqlDbType.VarChar, -1)]
        public string ConteudoBusca { get; set; }

        [DataField("HTM_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("HTM_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        #region Busca

        /// <summary>
        /// MOD_C_URL da tabela CMS_MOD_MODULO
        /// </summary>
        /// <returns></returns>
        public string GetUrlModulo() { return "html"; }

        public decimal GetCodigoRegistro() { return 0; }

        public decimal GetCodigoPortal() { return BLPortal.Atual.Codigo.GetValueOrDefault(); }

        //Idioma não implementado
        public decimal GetCodigoIdioma() { return 0; }

        public string GetTitulo() { return this.Titulo; }

        public string GetChamada()
        {
            var txt = this.ConteudoBusca.HtmlUnescapeDecode().RemoveHtmlTags();

            if (txt.Length > 310)
            {
                var substr = txt.Substring(0, 310);

                return substr.Substring(0, substr.LastIndexOf(" "));
            }
            else
            {
                return txt;
            }
        }

        //Critérios de Busca (Buscar todos eventos que contenham o termo no titulo, na chamada, e no conteúdo
        public string GetTermoBusca() { return this.Conteudo.HtmlUnescapeDecode().RemoveHtmlTags() + " " + this.Titulo; }

        //Url amigável de acesso ao evento
        public string GetUrl() { return string.Concat(CodigoPagina, Repositorio); }

        public string GetImagem() { return string.Empty; }

        public DateTime? GetInicio() { return DateTime.Now; }

        /// <summary>
        /// Determina a data de exibição do registro de acordo com o Status
        /// Se o evento estiver inativo, utilizada data limite menos que a data atual para não exibir o registro
        /// </summary>
        /// <returns></returns>
        public DateTime? GetFim() { return null; }

        #endregion
    }

    [Table("MOD_HTM_HTML_EDICAO")]
    public class MLModuloHtmlEdicao : MLModuloHtml { }

    [Table("MOD_HTM_HTML_PUBLICADO")]
    public class MLModuloHtmlPublicado : MLModuloHtml { }
}