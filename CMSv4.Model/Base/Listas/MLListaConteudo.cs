using System;
using System.Data;
using Framework.Model;
using System.Collections.Generic;
using System.Globalization;
using Framework.Utilities;

namespace CMSv4.Model
{
    #region Lista Conteudo
    public abstract class MLListaConteudoPrototype
    {
        public MLListaConteudoPrototype()
        {
            Relacionados = new List<MLListaConteudoRelacionados>();
        }

        [DataField("LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoLista { get; set; }

        [DataField("LIT_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("LIT_D_PUBLICACAO", SqlDbType.DateTime)]
        public DateTime? DataPublicacao { get; set; }

        [DataField("LIT_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LIT_C_URL", SqlDbType.VarChar, 250)]
        public string Url { get; set; }
        
        [DataField("LIT_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("LIT_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("LIT_C_IMAGEM", SqlDbType.VarChar, 250, IgnoreEmpty = true)]
        public string Imagem { get; set; }

        [DataField("LIT_N_REFERENCIA", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoReferencia { get; set; }

        [DataField("LIT_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("LIT_C_EXTRA1", SqlDbType.VarChar, 250)]
        public string Extra1 { get; set; }

        [DataField("LIT_C_EXTRA2", SqlDbType.VarChar, 250)]
        public string Extra2 { get; set; }

        [DataField("LIT_C_EXTRA3", SqlDbType.VarChar, 250)]
        public string Extra3 { get; set; }

        [DataField("LIT_C_MAX1", SqlDbType.VarChar, -1)]
        public string Max1 { get; set; }

        [DataField("LIT_C_MAX2", SqlDbType.VarChar, -1)]
        public string Max2 { get; set; }

        [DataField("LIT_D_SOLICITACAO", SqlDbType.DateTime)]
        public DateTime? DataSolicitacao { get; set; }

        [DataField("LIT_USU_N_CODIGO_SOLICITADO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioSolicitado { get; set; }

        [DataField("LIT_D_APROVACAO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataAprovacao { get; set; }

        [DataField("LIT_USU_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoUsuarioAprovacao { get; set; }

        [DataField("LIT_N_VISUALIZACOES", SqlDbType.Int)]
        public int? Visualizacoes { get; set; }

        [DataField("LIT_C_GUID", SqlDbType.UniqueIdentifier, IgnoreEmpty = true)]
        public Guid? GUID { get; set; }

        [DataField("LIT_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("LIT_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }


        [DataField("LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }


        [JoinField("LIT_IDI_N_CODIGO", "FWK_IDI_IDIOMA", "IDI_N_CODIGO", "IDI_C_NOME")]
        [DataField("IDI_C_NOME", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string IdiomaNome { get; set; }


        [DataField("LIT_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        [DataField("IDIOMAS_DISPONIVEIS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        [SubSelectField("IDIOMAS_DISPONIVEIS", @"
                                                    SELECT CAST(IDI.IDI_N_CODIGO AS VARCHAR) + ':' 
                                                    + IDI.IDI_C_SIGLA + ':' 
                                                    + ISNULL((SELECT CAST(LIST.LIT_N_CODIGO  AS VARCHAR)+':'+LIST.LIT_C_URL FROM MOD_LIS_LISTA_CONTEUDO LIST 
			                                            WHERE LIST.LIT_IDI_N_CODIGO = IDI.IDI_N_CODIGO AND 
			                                            (LIST.LIT_N_CODIGO = tabela.LIT_N_CODIGO 
				                                            OR LIST.LIT_N_CODIGO_BASE = tabela.LIT_N_CODIGO
				                                            OR LIST.LIT_N_CODIGO = tabela.LIT_N_CODIGO_BASE
				                                            OR LIST.LIT_N_CODIGO_BASE = tabela.LIT_N_CODIGO_BASE)
			                                            ), ':') + ','
                                                
	                                            FROM FWK_IDI_IDIOMA IDI
	                                            WHERE IDI.IDI_B_ATIVO = 1
                                                AND IDI.IDI_N_CODIGO <> tabela.LIT_IDI_N_CODIGO
	                                            FOR XML PATH('')")]
        public string IdiomasCadastrados { get; set; }

        public List<MLListaConteudoRelacionados> Relacionados { get; set; }

        public List<MLListaConteudoRelacionadosIdiomas> listaRegistroRelacionadosIdiomas
        {
            get
            {
                var list = new List<MLListaConteudoRelacionadosIdiomas>();
                if (!string.IsNullOrEmpty(IdiomasCadastrados))
                {
                    var idiomas = IdiomasCadastrados.Split(',');
                    if (idiomas != null)
                    {
                        foreach (var item in idiomas)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {



                                var subitem = item.Split(':');
                                var model = new MLListaConteudoRelacionadosIdiomas();
                                if (subitem[0] != null && subitem[0] != "0")
                                {
                                    model.CodigoIdioma = Convert.ToDecimal(subitem[0]);
                                }
                                if (subitem.Length > 1)
                                {
                                    if (subitem[1] != null && subitem[1].Length > 0)
                                    {
                                        model.Sigla = subitem[1];
                                    }
                                    if (subitem.Length > 2)
                                    {
                                        if (subitem[2] != null && subitem[2].Length > 0)
                                        {
                                            model.Codigo = Convert.ToDecimal(subitem[2]);
                                        }
                                    }
                                    if (subitem.Length > 3)
                                    {
                                        if (subitem[3] != null && subitem[3].Length > 0)
                                        {
                                            model.Url = subitem[3];
                                        }
                                    }
                                }
                                list.Add(model);
                            }




                        }
                    }

                }
                return list;
            }

        }

        public string DataFormatada(string culture, string prefix)
        {
            if (this.Data.HasValue)
            {

                var dia = this.Data.Value.Day.ToString();
                var mes = this.Data.Value.ToString("MMMM", new CultureInfo(culture));
                var ano = this.Data.Value.Year.ToString();

                return string.Format("{0} {1} {2} {1} {3} ", dia, prefix, mes.UpperPrimeiroChar(), ano);
            }

            return String.Empty;

        }
    }

    public class MLListaConteudoRelacionados
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal Codigo { get; set; }

        [DataField("LIT_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LIT_C_CHAMADA", SqlDbType.VarChar, 300)]
        public string Chamada { get; set; }

        [DataField("LIT_C_URL", SqlDbType.VarChar, 250)]
        public string Url { get; set; }

        [DataField("LIT_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("LIT_C_IMAGEM", SqlDbType.VarChar, 250, IgnoreEmpty = true)]
        public string Imagem { get; set; }

        [DataField("LIT_C_EXTRA1", SqlDbType.VarChar, 250)]
        public string Extra1 { get; set; }

        [DataField("URL_DETALHE", SqlDbType.VarChar, 350, IgnoreEmpty = true)]
        public string UrlDetalheCompleta { get; set; }

        [DataField("ROWNUMBER", SqlDbType.BigInt, IgnoreEmpty = true)]
        public Int64? RowNumber { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int, IgnoreEmpty = true)]
        public Int32? TotalRows { get; set; }

        public string DataFormatada(string culture, string prefix)
        {
            if (this.Data.HasValue)
            {

                var dia = this.Data.Value.Day.ToString();
                var mes = this.Data.Value.ToString("MMMM", new CultureInfo(culture));
                var ano = this.Data.Value.Year.ToString();

                return string.Format("{0} {1} {2} {1} {3} ", dia, prefix, mes.UpperPrimeiroChar(), ano);
            }

            return String.Empty;

        }


    }

    public class MLListaConteudoRelacionadosIdiomas
    {
        public decimal? Codigo { get; set; }
        public decimal? CodigoIdioma { get; set; }
        public string Sigla { get; set; }

        public string Url { get; set; }
    }

    public class MLListaConteudoCompleto : MLListaConteudoPublicado
    {
        public MLListaConteudoCompleto()
        {
            Imagens = new List<MLListaConteudoImagem>();
            Videos = new List<MLListaConteudoVideo>();
            Audios = new List<MLListaConteudoAudio>();
            Seo = new MLListaConteudoSEOPublicado();
        }

        public List<MLListaConteudoImagem> Imagens { get; set; }
        public List<MLListaConteudoVideo> Videos { get; set; }
        public List<MLListaConteudoAudio> Audios { get; set; }
        public MLListaConteudoSEOPublicado Seo { get; set; }

        [DataField("QTDE_AVALIACOES", SqlDbType.Int)]
        public int? QtdeAvaliacoes { get; set; }

        [DataField("NOTA", SqlDbType.Int)]
        public int? TotalNota { get; set; }

        [DataField("MEDIA", SqlDbType.Int)]
        public int? Media { get; set; }

        [DataField("LCA_N_CODIGO", SqlDbType.Int)]
        public int? CodigoAvaliacao { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO")]
    public class MLListaConteudo : MLListaConteudoPrototype
    {
        public MLListaConteudo()
        {
            Seo = new MLListaConteudoSEO();
        }

        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("LIT_B_EDITADO", SqlDbType.Bit)]
        public bool? Editado { get; set; }

        [DataField("LIT_B_PUBLICADO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? Publicado { get; set; }

        public MLListaConteudoSEO Seo { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_PUBLICADO")]
    public class MLListaConteudoPublicado : MLListaConteudoPrototype, ISearchable
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        public decimal? CodigoPortal { get; set; }

        public string GetUrlModulo()
        {
            return "lista";
        }

        public decimal GetCodigoRegistro()
        {
            return this.Codigo.Value;
        }

        public decimal GetCodigoPortal()
        {
            if (this.CodigoPortal.HasValue)
                return CodigoPortal.Value;
            return 0;
        }

        public string GetTitulo()
        {
            return this.Titulo;
        }

        public string GetChamada()
        {
            return this.Chamada;
        }

        public string GetTermoBusca()
        {
            if (!String.IsNullOrEmpty(this.Conteudo))
                return this.Conteudo.HtmlUnescapeDecode().RemoveHtmlTags();
            return this.Titulo + " " + this.Chamada;
        }

        public string GetUrl()
        {
            return this.Url;
        }

        public string GetImagem()
        {
            return this.Imagem;
        }

        public DateTime? GetInicio()
        {
            return this.Data;
        }

        public DateTime? GetFim()
        {
            return this.DataPublicacao;
        }

        public decimal GetCodigoIdioma()
        {
            return 0;
        }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO")]
    public class MLListaConteudoGrid
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Codigo { get; set; }

        [DataField("LIT_LIS_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoLista { get; set; }

        [DataField("LIT_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("LIT_D_PUBLICACAO", SqlDbType.DateTime)]
        public DateTime? DataPublicacao { get; set; }

        [DataField("LIT_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LIT_C_URL", SqlDbType.VarChar, 250)]
        public string Url { get; set; }

        [DataField("LIT_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("LIT_B_EDITADO", SqlDbType.Bit)]
        public bool? Editado { get; set; }

        [DataField("LIT_B_PUBLICADO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? Publicado { get; set; }

        [DataField("LIT_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("LIT_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("LIT_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("LIT_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        [DataField("IDIOMAS_DISPONIVEIS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string IdiomasCadastrados { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }
    }
    #endregion

    #region Categorias
    [Table("MOD_LIS_LISTA_CONTEUDO_CATEGORIAS")]
    public class MLListaConteudoCategoria
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoCategoria { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_CATEGORIAS_PUBLICADO")]
    public class MLListaConteudoCategoriaPublicado
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LIC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoCategoria { get; set; }
    }
    #endregion

    #region Imagens
    public abstract class MLListaConteudoImagemPrototype
    {
        [DataField("LII_LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LII_C_IMAGEM", SqlDbType.VarChar, 250, IgnoreEmpty = true)]
        public string Imagem { get; set; }

        [DataField("LII_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LII_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("LII_C_FONTE", SqlDbType.VarChar, 250)]
        public string Fonte { get; set; }

        [DataField("LII_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

    }

    [Table("MOD_LIS_LISTA_CONTEUDO_IMAGENS")]
    public class MLListaConteudoImagem : MLListaConteudoImagemPrototype
    {
        [DataField("LII_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        public Guid? GuidConteudo { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_IMAGENS_PUBLICADO")]
    public class MLListaConteudoImagemPublicado : MLListaConteudoImagemPrototype
    {
        [DataField("LII_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }
    }
    #endregion
    
    #region Video
    public abstract class MLListaConteudoVideoPrototype
    {
        [DataField("LIV_LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LIV_C_VIDEO", SqlDbType.VarChar, 500, IgnoreEmpty = true)]
        public string Video { get; set; }

        [DataField("LIV_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LIV_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("LIV_C_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

        [DataField("LIV_C_FONTE", SqlDbType.VarChar, 250)]
        public string Fonte { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_VIDEOS")]
    public class MLListaConteudoVideo : MLListaConteudoVideoPrototype
    {
        [DataField("LIV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("LIV_C_GUID", SqlDbType.UniqueIdentifier, IgnoreEmpty = true)]
        public Guid? Guid { get; set; }

        public Guid? GuidConteudo { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_VIDEOS_PUBLICADO")]
    public class MLListaConteudoVideoPublicado : MLListaConteudoVideoPrototype
    {
        [DataField("LIV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("LIv_C_GUID", SqlDbType.UniqueIdentifier, IgnoreEmpty = true)]
        public Guid? Guid { get; set; }

        public Guid? GuidConteudo { get; set; }
    }
    #endregion

    #region Audio
    public abstract class MLListaConteudoAudioPrototype
    {
        [DataField("LIA_LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LIA_C_AUDIO", SqlDbType.VarChar, 500, IgnoreEmpty = true)]
        public string Audio { get; set; }

        [DataField("LIA_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("LIA_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("LIA_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

        [DataField("LIA_C_FONTE", SqlDbType.VarChar, 250)]
        public string Fonte { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_AUDIOS")]
    public class MLListaConteudoAudio : MLListaConteudoAudioPrototype
    {
        [DataField("LIA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("LIA_C_GUID", SqlDbType.UniqueIdentifier, IgnoreEmpty = true)]
        public Guid? Guid { get; set; }

        public Guid? GuidConteudo { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_AUDIOS_PUBLICADO")]
    public class MLListaConteudoAudioPublicado : MLListaConteudoAudioPrototype
    {
        [DataField("LIA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("LIA_C_GUID", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public Guid? Guid { get; set; }

        public Guid? GuidConteudo { get; set; }
    }
    #endregion

    public class MLFiltroAno
    {
        [DataField("ANO", SqlDbType.Int)]
        public int Ano { get; set; }
    }

    public class MLFiltroMes
    {
        [DataField("MES", SqlDbType.Int)]
        public int Mes { get; set; }

        public string NomeMes 
        {
            get
            {
                DateTime data = new DateTime(2000, this.Mes, 1);
                return data.ToString("MMMM", new CultureInfo(BLIdioma.Atual.Sigla)).UpperPrimeiroChar();
            }
        }
    }

    public class MLEventosLTP : MLListaConteudoPublicado
    {
        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }

        public string NomeMes
        {
            get
            {
                DateTime data = new DateTime(2000, this.Data.Value.Month, 1);
                return data.ToString("MMMM", new CultureInfo(BLIdioma.Atual.Sigla)).UpperPrimeiroChar();
            }
        }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_AVALIACAO")]
    public class MLListaConteudoAvaliacao
    {
        [DataField("LCA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("LCA_LIT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("LCA_CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCliente { get; set; }

        [DataField("LCA_N_NOTA", SqlDbType.Int)]
        public decimal? Nota { get; set; }

        [DataField("LCA_C_IP", SqlDbType.VarChar, 50)]
        public string IP { get; set; }

        [DataField("LCA_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }
    }

    public class MLAvaliacaoRetorno
    {
        [DataField("MEDIA", SqlDbType.Int)]
        public int? Media { get; set; }

        [DataField("QTDE_AVALIACOES", SqlDbType.Int)]
        public int? QtdeAvaliacoes { get; set; }
    }
}
