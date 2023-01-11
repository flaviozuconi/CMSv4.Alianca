using System;
using System.Data;
using Framework.Model;
using System.Web.Mvc;
using System.Collections.Generic;

namespace CMSv4.Model
{

    public class MLBannerArquivoView : MLBanner
    {
        public MLBannerArquivoView()
        {
            ListaArquivos = new List<MLBannerArquivo>();
            ListaArquivosEmDisco = new List<MLBannerArquivo>();
        }

        public List<MLBannerArquivo> ListaArquivos { get; set; }
        public List<MLBannerArquivo> ListaArquivosEmDisco { get; set; }
        public string DiretorioGaleria { get; set; }
    }

    /// <summary>
    /// BANNER ARQUIVO
    /// </summary>
    [Serializable]    
    [Table("MOD_BAN_BANNERS_ARQUIVOS")]
    [Auditing("/cms/banneradmin", new string[] { "CodigoBanner" }, "CodigoPortal")]
    public class MLBannerArquivo
    {
        [DataField("BAA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("BAA_BAN_N_CODIGO", SqlDbType.Int)]
        public decimal? CodigoBanner { get; set; }

        [DataField("BAA_N_TIPO", SqlDbType.TinyInt)]
        public byte? CodigoTipo { get; set; }

        public decimal? CodigoPortal { get; set; }

        [DataField("BAA_C_IMAGEM", SqlDbType.VarChar, 250, IgnoreEmpty = true)]
        public string Imagem { get; set; }

        [DataField("BAA_C_IMAGEM_MOBILE", SqlDbType.VarChar, 250)]
        public string ImagemMobile { get; set; }

        [DataField("BAA_C_IMAGEM_TABLET", SqlDbType.VarChar, 250)]
        public string ImagemTablet { get; set; }

        [AllowHtml]
        [DataField("BAA_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("BAA_N_TEMPO", SqlDbType.Int)]
        public int? Tempo { get; set; }

        [DataField("BAA_N_ORDEM", SqlDbType.Int, IgnoreEmpty=true)]
        public int? Ordem { get; set; }

        [DataField("BAA_C_URL", SqlDbType.VarChar, 500)]
        public string Url { get; set; }

        [DataField("BAA_B_NOVA_JANELA", SqlDbType.Bit)]
        public bool? NovaJanela { get; set; }

        [DataField("BAA_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("BAA_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("BAA_B_ATIVO", SqlDbType.Bit)]        
        public bool? Ativo { get; set; }

        [DataField("BAA_C_TAGGA", SqlDbType.VarChar,50)]
        public string TagGA { get; set; }

        [AllowHtml]
        [DataField("BAA_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("BAA_C_WIDTH", SqlDbType.VarChar, 100)]
        public string Width { get; set; }

        [DataField("BAA_C_HEIGTH", SqlDbType.VarChar, 100)]
        public string Heigth { get; set; }

        [DataField("BAA_BAA_N_CODIGO_HOVER", SqlDbType.Decimal, 18)]
        public decimal? CodigoBannerHover { get; set; }

        #region CSS
        [DataField("BAA_C_CSS_CLASS_1", SqlDbType.VarChar, 200)]
        public string CssClass1 { get; set; }
        #endregion

        [DataField("BAA_C_VIEW", SqlDbType.VarChar, 250)]
        public string View { get; set; }

        public enum Tipo
        {
            ArquivoItem = 1,
            ArquivoItemIframe = 2
        }

        public string MontarNomeImagem
        {
            get
            {
                if (!string.IsNullOrEmpty(Titulo))
                {
                    return string.Format("{0} - {1}", Titulo, Imagem);
                }
                else if (!string.IsNullOrEmpty(Imagem))
                {
                    return string.Format("{0}", Imagem);
                }
                return string.Empty;
            }
        }
    }

    [Table("MOD_BAN_BANNERS_ARQUIVOS")]
    public class MLBannerArquivoPublico : MLBannerArquivo
    {
        [DataField("BAA_C_IMAGEM_HOVER", SqlDbType.VarChar, 250)]
        public string ImagemBannerHover { get; set; }

        public string Class { get; set; }

        public string Target
        {
            get
            {
                if (this.NovaJanela.HasValue && this.NovaJanela.Value)
                {
                    return "_blank";
                }

                return "_top";
            }
        }
    }

}
