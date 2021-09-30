using System;
using System.Data;
using Framework.Model;
using System.IO;
using System.Web.Mvc;
using Framework.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    public class MLArquivoPublico : MLArquivo
    {
        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }

        public string EnderecoDownload()
        {
            return string.Format("{0}/download/{1}/{2}", Portal.Url(), BLPortal.Atual.Diretorio, Codigo);
        }
    }


    /// <summary>
    /// ARQUIVO
    /// </summary>
    [Serializable]
    [Table("MOD_ARQ_ARQUIVOS")]
    [Auditing("/cms/arquivoadmin/arquivo", "CodigoPortal")]
    public class MLArquivo : BaseModel, ISearchable
    {
        [DataField("ARQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ARQ_D_DATA", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? Data { get; set; }

        [DataField("ARQ_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("ARQ_C_NOME", SqlDbType.VarChar, -1, IgnoreEmpty=true)]
        public string Nome { get; set; }

        [DataField("ARQ_C_PASTA_RELATIVA", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string PastaRelativa { get; set; }

        [DataField("ARQ_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [AllowHtml]
        [DataField("ARQ_C_DESCRICAO", SqlDbType.VarChar, -1)]
        public string Descricao { get; set; }

        [DataField("ARQ_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("ARQ_D_APROVACAO", SqlDbType.DateTime)]
        public DateTime? DataAprovacao { get; set; }

        [DataField("ARQ_USU_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioAprovacao { get; set; }

        [DataField("ARQ_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
        
        [DataField("ARQ_ACA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

        [DataField("ARQ_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [JoinField("ARQ_ACA_N_CODIGO", "MOD_ARQ_CATEGORIAS_ARQUIVO", "ACA_N_CODIGO", "ACA_C_NOME")]
        [DataField("ACA_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeCategoria { get; set; }

        [DataField("ARQ_N_REFERENCIA", SqlDbType.DateTime, IgnoreEmpty = true)]
        public decimal? CodigoReferencia { get; set; }


        // JOIN
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        #region Novas propriedades 19/05/2015
        [DataField("ARQ_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        [DataField("IDIOMAS_DISPONIVEIS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        [SubSelectField("IDIOMAS_DISPONIVEIS", @"SELECT CAST(IDI.IDI_N_CODIGO AS VARCHAR) + ':' + IDI.IDI_C_SIGLA + ':' + ISNULL((SELECT CAST(ARQ.ARQ_N_CODIGO AS VARCHAR) FROM MOD_ARQ_ARQUIVOS ARQ 
			                                            WHERE ARQ.ARQ_IDI_N_CODIGO = IDI.IDI_N_CODIGO AND 
			                                            (ARQ.ARQ_N_CODIGO = tabela.ARQ_N_CODIGO 
				                                            OR ARQ.ARQ_N_CODIGO_BASE = tabela.ARQ_N_CODIGO
				                                            OR ARQ.ARQ_N_CODIGO = tabela.ARQ_N_CODIGO_BASE
				                                            OR ARQ.ARQ_N_CODIGO_BASE = tabela.ARQ_N_CODIGO_BASE)
			                                            ), '') + ','
	                                            FROM FWK_IDI_IDIOMA IDI
	                                            WHERE IDI.IDI_B_ATIVO = 1
                                                AND IDI.IDI_N_CODIGO <> tabela.ARQ_IDI_N_CODIGO
	                                            FOR XML PATH('')")]
        public string IdiomasCadastrados { get; set; }
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
            return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(diretorioPortal)), Imagem);
        }
        #endregion

        #region ISearchable
        public string GetUrlModulo() { return "arquivo"; }

        public decimal GetCodigoRegistro() { return 0; }

        public decimal GetCodigoPortal() { return BLPortal.Atual.Codigo.GetValueOrDefault(); }

        public decimal GetCodigoIdioma() { return 0; }

        public string GetTitulo() { return Titulo; }

        public string GetChamada()
        {
            var txt = Descricao.HtmlUnescapeDecode().RemoveHtmlTags();

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

        public string GetTermoBusca() { return Descricao.HtmlUnescapeDecode().RemoveHtmlTags() + " " + Titulo; }

        public string GetUrl() { return Codigo.GetValueOrDefault().ToString(); }

        public string GetImagem() { return Imagem; }

        public DateTime? GetInicio() { return Data; }

        public DateTime? GetFim() { return null; }

        public bool GetAtivo() { return Ativo.GetValueOrDefault(); }
        #endregion
    }
}
