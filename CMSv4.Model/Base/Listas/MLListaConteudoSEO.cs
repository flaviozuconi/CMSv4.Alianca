using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    public class MLListaConteudoSEOPrototype : MLConteudoSeo
    {
        [DataField("LIT_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }
    }

    [Table("MOD_LIS_LISTA_CONTEUDO_SEO")]
    public class MLListaConteudoSEO : MLListaConteudoSEOPrototype { }

    [Table("MOD_LIS_LISTA_CONTEUDO_SEO_PUBLICADO")]
    public class MLListaConteudoSEOPublicado : MLListaConteudoSEOPrototype { }

}
