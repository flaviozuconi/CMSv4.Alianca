using System;
using System.Collections.Generic;
using Framework.Model;
using System.Data;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// PAGINA
    /// </summary>
    [Serializable]
    [Table("CMS_PAG_PAGINA")]
    public class MLPaginaPublico
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PAG_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("PAG_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("PAG_SEC_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoSecao { get; set; }

        [DataField("PAG_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("PAG_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        [DataField("PAG_B_HTTPS", SqlDbType.Bit)]
        public bool? Https { get; set; }

        // Publicado

        [DataField("PAG_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("PAG_C_DESCRICAO", SqlDbType.VarChar, 250)]
        public string Descricao { get; set; }

        [DataField("PAG_C_TAGS", SqlDbType.VarChar, 250)]
        public string Tags { get; set; }

        [DataField("PAG_C_NOME_LAYOUT", SqlDbType.VarChar, 100)]
        public string NomeLayout { get; set; }

        [DataField("PAG_C_TEMPLATE", SqlDbType.VarChar, -1)]
        public string TemplateCustomizado { get; set; }

        [DataField("PAG_C_NOME_TEMPLATE", SqlDbType.VarChar, 100)]
        public string NomeTemplate { get; set; }

        [DataField("IDI_C_SIGLA", SqlDbType.VarChar, -1)]
        public string Scripts { get; set; }

        [DataField("PAG_C_URL_LOGIN", SqlDbType.VarChar, 500)]
        public string UrlLogin { get; set; }

        #region Informações de idioma
        [DataField("PAG_IDI_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? IdiomaCodigo { get; set; }

        [DataField("IDI_C_SIGLA", SqlDbType.VarChar, 5, IgnoreEmpty = true)]
        public string IdiomaSigla { get; set; }

        [DataField("IDI_C_NOME", SqlDbType.VarChar, 50, IgnoreEmpty = true)]
        public string IdiomaNome { get; set; }

        [DataField("IDI_B_ATIVO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? IdiomaAtivo { get; set; }


        //Idiomas relacionados a este registros 
        //ex. string contem codigo,idioma# 

        [DataField("IDIOMAS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string Idiomas { get; set; }


        public List<MLIdiomaPagina> listaIdiomas
        {
            get
            {
                var list = new List<MLIdiomaPagina>();
                if (!string.IsNullOrEmpty(Idiomas))
                {
                    var idiomas = Idiomas.Split('#');
                    if (idiomas != null)
                    {
                        foreach (var item in idiomas)
                        {
                            if (string.IsNullOrEmpty(item))
                            {
                                continue;
                            }

                            var subitem = item.Split(',');
                            var model = new MLIdiomaPagina();
                            if (subitem[0] != null && subitem[0] != "0")
                            {
                                model.CodigoPagina = Convert.ToDecimal(subitem[0]);
                            }
                            if (subitem[1] != null && subitem[1] != "0")
                            {
                                model.Codigo = Convert.ToDecimal(subitem[1]);
                            }

                            if (subitem[2] != null && subitem[2] != "0")
                            {
                                model.Sigla = subitem[2];
                            }
                            if (subitem[3] != null &&  !string.IsNullOrEmpty(subitem[3]))
                            {
                                model.urlPagina = subitem[3];
                            }

                            list.Add(model);
                        }
                    }

                }
                return list;
            }

        }

        public List<MLListaConteudoRelacionadosIdiomas> ListaDetalhesRelacionados
        {
            get;
            set;

        }
        #endregion

    }
}
