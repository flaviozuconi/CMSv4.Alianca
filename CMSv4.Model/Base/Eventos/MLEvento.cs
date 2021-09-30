using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Table("MOD_EVE_EVENTOS")]
    public class MLEventoPublico : MLEvento
    {
        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }
    }

    /// <summary>
    /// Evento
    /// </summary>
    [Serializable]
    [Table("MOD_EVE_EVENTOS")]
    public class MLEvento : BaseModel
    {
        public MLEvento()
        {
            Seo = new MLEventoSEO();
        }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("EVE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("EVE_C_URL", SqlDbType.VarChar, 200)]
        public string Url { get; set; }
        
        [Required]
        [DataField("EVE_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("EVE_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("EVE_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [Required]
        [DataField("EVE_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("EVE_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("EVE_C_IMAGEM", SqlDbType.VarChar, 5)]
        public string Imagem { get; set; }
               
        [DataField("EVE_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("EVE_C_LOCAL", SqlDbType.VarChar, 100)]
        public string Local { get; set; }

        [DataField("EVE_C_LOGRADOURO", SqlDbType.VarChar, 300)]
        public string Logradouro { get; set; }

        [DataField("EVE_C_NUMERO", SqlDbType.VarChar, 10)]
        public string Numero { get; set; }

        [DataField("EVE_C_COMPLEMENTO", SqlDbType.VarChar, 20)]
        public string Complemento { get; set; }

        [DataField("EVE_C_BAIRRO", SqlDbType.VarChar, 100)]
        public string Bairro { get; set; }

        [DataField("EVE_C_CIDADE", SqlDbType.VarChar, 100)]
        public string Cidade { get; set; }

        [DataField("EVE_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }
        
        [DataField("EVE_B_INSCRICAO", SqlDbType.Bit)]
        public bool? IsInscricao { get; set; }

        [DataField("EVE_D_INICIO_INSCRICAO", SqlDbType.DateTime)]
        public DateTime? DataInicioInscricao { get; set; }

        [DataField("EVE_D_TERMINO_INSCRICAO", SqlDbType.DateTime)]
        public DateTime? DataTerminoInscricao { get; set; }
        
        [DataField("EVE_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("EVE_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("EVE_C_HORA", SqlDbType.VarChar, 70)]
        public string Hora { get; set; }

        [DataField("EVE_IDI_N_CODIGO", SqlDbType.Decimal,18)]
        public decimal? CodigoIdioma { get; set; }

        #region Novas propriedades 19/05/2015
        [DataField("EVE_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }
        #endregion

        public string Class { get; set; }

        public string NomeImagem 
        {
            get
            {
                if(!String.IsNullOrEmpty(this.Imagem))
                    return string.Concat("capa", Imagem);

                return String.Empty;
            }
        }

        public string DataString
        {
            get
            {
                /*
                if (DataTermino.HasValue)
                    return string.Concat(DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy")), " - ", DataTermino.Value.ToString(BLTraducao.T("dd/MM/yyyy")));
                */
                return DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
            }
        }

        public string CidadeEstado
        {
            get
            {
                if (!string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Estado))
                    return string.Concat(Cidade, "/", Estado);
                else if (!string.IsNullOrEmpty(Cidade) && string.IsNullOrEmpty(Estado))
                    return Cidade;
                else if (string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Estado))
                    return Estado;

                return string.Empty;
            }
        }

        public string EnderecoCompleto
        {
            get
            {
                return ObterEnderecoCompleto(true);
            }
        }

        public string EnderecoGoogleMaps
        {
            get
            {
                return System.Web.HttpUtility.UrlEncode(ObterEnderecoCompleto(false)).Replace("+", "%20");
            }
        }

        private string ObterEnderecoCompleto(bool IncluirComplemento)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Logradouro))
            {
                sb.Append(Logradouro.Trim());
                sb.Append(", ");
            }

            if (!string.IsNullOrEmpty(Numero))
            {
                sb.Append(Numero.Trim());
                sb.Append(", ");
            }

            if (IncluirComplemento && !string.IsNullOrEmpty(Complemento))
            {
                sb.Append(Complemento.Trim());
                sb.Append(", ");
            }

            if (!string.IsNullOrEmpty(Bairro))
            {
                sb.Append(Bairro.Trim());
                sb.Append(", ");
            }

            if (!string.IsNullOrEmpty(Cidade))
            {
                sb.Append(Cidade.Trim());
                sb.Append(" - ");
            }

            if (!string.IsNullOrEmpty(Estado))
            {
                sb.Append(Estado.Trim());
            }

            return sb.ToString();
        }

        public MLEventoSEO Seo { get; set; }

        //Implementação da Interface de Busca

        #region Busca

        /// <summary>
        /// MOD_C_URL da tabela CMS_MOD_MODULO
        /// </summary>
        /// <returns></returns>
        public string GetUrlModulo() { return "evento"; }

        public decimal GetCodigoRegistro() { return this.Codigo.Value; }

        public decimal GetCodigoPortal() { return this.CodigoPortal.Value; }

        //Idioma não implementado
        public decimal GetCodigoIdioma() { return 0; }

        public string GetTitulo() { return this.Titulo; }

        public string GetChamada() { return this.Chamada; }

        //Critérios de Busca (Buscar todos eventos que contenham o termo no titulo, na chamada, e no conteúdo
        public string GetTermoBusca() { return this.Conteudo.HtmlUnescapeDecode().RemoveHtmlTags() + " " + this.Titulo + " " + this.Chamada; }

        //Url amigável de acesso ao evento
        public string GetUrl() { return this.Url; }

        public string GetImagem() { return this.Imagem; }

        public DateTime? GetInicio() { return DateTime.Now; }

        /// <summary>
        /// Determina a data de exibição do registro de acordo com o Status
        /// Se o evento estiver inativo, utilizada data limite menos que a data atual para não exibir o registro
        /// </summary>
        /// <returns></returns>
        public DateTime? GetFim()
        {
            if (!Ativo.GetValueOrDefault())
                return DateTime.Now.AddDays(-5);
            return null;
        }

        #endregion
    }

    [Table("CMS_EVE_EVENTO_SEO")]
    public class MLEventoSEO : MLConteudoSeo
    {
        [DataField("EVE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }
    }
}
