using System;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary> 
    /// <remarks>
    /// TIPO: três caracteres que identificam o tipo do registro PAG | NOT | EVE | ARQ | GAL
	/// URL: url do item para acessa-lo
    /// TITULO: texto que será apresentado na primeira linha do resultado de busca
    /// CHAMADA: texto que será apresentado abaixo do título no resultado de busca
    /// RANK: identificador de relevancia calculado pelo sql,
    /// CODIGO: chave primaria do registro
    /// DATA: data de cadastro do registro
    /// IMAGEM: imagem de referência do item
    /// </remarks>
    public class MLBuscaResultado
    {
        public MLBuscaResultado()
        {
            LstAssuntos = new List<MLAssuntosBusca>();
        }

        [DataField("TIPO", SqlDbType.VarChar, 3)]
        public string Tipo { get; set; }

        [DataField("URL", SqlDbType.VarChar, -1)]
        public string Url { get; set; }

        [DataField("TITULO", SqlDbType.VarChar, -1)]
        public string Titulo { get; set; }

        [DataField("CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("RANK", SqlDbType.Int)]
        public int? Rank { get; set; }

        [DataField("CODIGO", SqlDbType.Decimal)]
        public decimal? Codigo { get; set; }

        [DataField("DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("IMAGEM", SqlDbType.VarChar, -1)]
        public string Imagem { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }

        public List<MLAssuntosBusca> LstAssuntos { get; set; }
    }

    public class MLAssuntosBusca
    {
        [DataField("AXP_PAG_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal CodigoPagina { get; set; }

        [DataField("ASU_C_ASSUNTO", SqlDbType.VarChar, 50)]
        public string Assunto { get; set; }

        [DataField("ASU_C_CSS", SqlDbType.VarChar, 30)]
        public string Class { get; set; }

        [DataField("ASU_C_URL", SqlDbType.VarChar, 50)]
        public string UrlAssunto { get; set; }

    }
}
