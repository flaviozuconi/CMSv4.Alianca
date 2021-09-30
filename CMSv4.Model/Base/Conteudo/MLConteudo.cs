using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model.Conteudo
{
    /// <summary>
    /// Conteudo
    /// </summary>
    [Serializable]
    public class MLConteudo : ISearchable
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [Required]
        [DataField("CON_C_CHAVE", SqlDbType.VarChar, 50)]
        public string Chave { get; set; }

        [DataField("CON_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [DataField("CON_D_PUBLICACAO", SqlDbType.DateTime)]
        public DateTime? DataPublicacao { get; set; }

        [DataField("CON_D_SOLICITACAO", SqlDbType.DateTime)]
        public DateTime? DataSolicitacao { get; set; }

        [DataField("CON_USU_N_CODIGO_SOLICITADO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioSolicitado { get; set; }

        [DataField("CON_D_APROVACAO", SqlDbType.DateTime)]
        public DateTime? DataAprovacao { get; set; }

        [DataField("CON_USU_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioAprovacao { get; set; }

        [DataField("CON_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("CON_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        public decimal? CodigoGrupoAprovador { get; set; }

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

        public string GetTitulo() { return Chave; }

        public string GetChamada()
        {
            var txt = this.Conteudo.HtmlUnescapeDecode().RemoveHtmlTags();

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
        public string GetTermoBusca() { return this.Conteudo.HtmlUnescapeDecode().RemoveHtmlTags() + " " + this.Chave; }

        //Url amigável de acesso ao evento
        public string GetUrl() { return Chave; }

        public string GetImagem() { return string.Empty; }

        public DateTime? GetInicio() { return DataPublicacao; }

        /// <summary>
        /// Determina a data de exibição do registro de acordo com o Status
        /// Se o evento estiver inativo, utilizada data limite menos que a data atual para não exibir o registro
        /// </summary>
        /// <returns></returns>
        public DateTime? GetFim() { return null; }

        #endregion
    }

    [Serializable]
    [Table("MOD_CON_CONTEUDO_EDICAO")]
    [Auditing("/cms/conteudoadmin", "CodigoPortal")]
    public class MLConteudoEdicao : MLConteudo
    {
        public MLConteudoEdicao()
        {
            Aprovadores = new List<MLUsuario>();
        }

        [DataField("CON_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("CON_B_EDITADO", SqlDbType.Bit)]
        public bool? Editado { get; set; }

        [DataField("CON_B_PUBLICADO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? Publicado { get; set; }

        [DataField("IDIOMAS_DISPONIVEIS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        [SubSelectField("IDIOMAS_DISPONIVEIS", @"SELECT CAST(IDI.IDI_N_CODIGO AS VARCHAR) + ':' + IDI.IDI_C_SIGLA + ':' + ISNULL(
		                                                (SELECT CAST(CON.CON_N_CODIGO AS VARCHAR)
		                                                FROM MOD_CON_CONTEUDO_EDICAO CON
                                                        WHERE CON.CON_IDI_N_CODIGO = IDI.IDI_N_CODIGO AND 
                                                        (CON.CON_N_CODIGO = tabela.CON_N_CODIGO 
                                                            OR CON.CON_N_CODIGO_BASE = tabela.CON_N_CODIGO
                                                            OR CON.CON_N_CODIGO = tabela.CON_N_CODIGO_BASE
                                                            OR CON.CON_N_CODIGO_BASE = tabela.CON_N_CODIGO_BASE)
                                                        ), '') + ','
                                                FROM FWK_IDI_IDIOMA IDI
                                                WHERE IDI.IDI_B_ATIVO = 1
                                                AND IDI.IDI_N_CODIGO <> tabela.CON_IDI_N_CODIGO
                                                FOR XML PATH('')")]
        public string IdiomasCadastrados { get; set; }

        public List<MLUsuario> Aprovadores { get; set; }
    }

    [Serializable]
    [Table("MOD_CON_CONTEUDO_PUBLICADO")]
    public class MLConteudoPublicado : MLConteudo
    {
        [DataField("CON_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }
    }

    [Table("MOD_CON_CONTEUDO_HISTORICO")]
    public class MLConteudoHistorico 
    {
        [DataField("COH_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("COH_CON_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoConteudo { get; set; }

        [DataField("COH_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [JoinField("COH_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_LOGIN")]
        [DataField("USU_C_LOGIN", SqlDbType.VarChar, 50)]
        public string Login { get; set; }

        [DataField("COH_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("COH_D_PUBLICACAO", SqlDbType.DateTime)]
        public DateTime? DataPublicacao { get; set; }

        [DataField("COH_C_CHAVE", SqlDbType.VarChar, 50)]
        public string Chave { get; set; }

        [DataField("COH_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }
    }
}
