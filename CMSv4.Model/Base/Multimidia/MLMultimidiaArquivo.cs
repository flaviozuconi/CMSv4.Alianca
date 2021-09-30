using System;
using System.Data;
using System.Web.Mvc;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_MUL_MULTIMIDIA_ARQUIVOS")]
    [Auditing("/cms/multimidiaadmin/arquivo", "CodigoPortal")]
    public class MLMultimidiaArquivo : BaseModel
    {
        [DataField("MAR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MAR_MTI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoTipo { get; set; }

        [DataField("MAR_D_DATA", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? Data { get; set; }

        [DataField("MAR_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("MAR_C_NOME", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string Nome { get; set; }

        [DataField("MAR_C_PASTA_RELATIVA", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string PastaRelativa { get; set; }

        [DataField("MAR_C_HTML_OUTPUT", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string HtmlOutput { get; set; }

        [DataField("MAR_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [AllowHtml]
        [DataField("MAR_C_DESCRICAO", SqlDbType.VarChar, -1)]
        public string Descricao { get; set; }

        [DataField("MAR_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("MAR_D_APROVACAO", SqlDbType.DateTime)]
        public DateTime? DataAprovacao { get; set; }

        [DataField("MAR_USU_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioAprovacao { get; set; }

        [DataField("MAR_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("MAR_MCA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

        [DataField("MAR_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        #region Novas propriedades 19/05/2015
        [DataField("MAR_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("IDI_C_NOME", SqlDbType.VarChar, 100)]
        [JoinField("MAR_IDI_N_CODIGO", "FWK_IDI_IDIOMA", "IDI_N_CODIGO", "IDI_C_NOME")]
        public string Idioma { get; set; }

        #endregion

        #region Propriedades Auxiliares
        public string DataString
        {
            get
            {
                if (Data.HasValue) return Data.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
                return "";
            }
        }

        /// <summary>
        /// Retorna o caminho do arquivo fisíco
        /// </summary>
        public string ThumbArquivo(string diretorioPortal)
        {
            return System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(diretorioPortal)), Imagem);
        }
        #endregion
    }
}
