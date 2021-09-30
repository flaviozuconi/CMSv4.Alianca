using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// BANNER
    /// </summary>
    [Serializable]
    [Table("MOD_RES_RESULTADO")]
    [Auditing("/cms/Resultadoadmin","CodigoPortal")]
    public class MLResultado:BaseModel
    {
        [DataField("RES_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("RES_C_CHAMADA", SqlDbType.VarChar, 250)]
        public string Chamada { get; set; }

        [DataField("RES_N_ANO", SqlDbType.Decimal, 18)]
        public decimal? Ano { get; set; }

        [DataField("RES_N_TRIMESTRE", SqlDbType.Decimal, 18)]
        public decimal? Trimestre { get; set; }

        [DataField("RES_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }
        //       iframe 
        [DataField("RES_C_IFRAME_TELECONFERENCIA", SqlDbType.VarChar, 500)]
        public string IframeTeleconferencia { get; set; }
        //        arquivos
        [DataField("RES_C_VALUE_BOOK", SqlDbType.VarChar, 250)]
        public string ValueBook { get; set; }

        [DataField("RES_C_RELEASE_RESULTADOS", SqlDbType.VarChar, 250)]
        public string ReleaseResultados { get; set; }

        [DataField("RES_C_DFP", SqlDbType.VarChar, 250)]
        public string Dfp { get; set; }

        [DataField("RES_C_AUDIO", SqlDbType.VarChar, 250)]
        public string Audio { get; set; }

        [DataField("RES_C_TRANSCRICAO", SqlDbType.VarChar, 250)]
        public string Transcricao { get; set; }

        [DataField("RES_C_APRESENTACAO", SqlDbType.VarChar, 250)]
        public string Apresentacao { get; set; }

        [DataField("RES_IDI_N_CODIGO", SqlDbType.Int)]
        public decimal? Idioma { get; set; }
        

        [JoinField("RES_IDI_N_CODIGO", "FWK_IDI_IDIOMA", "IDI_N_CODIGO", "IDI_C_NOME")]
        [DataField("IDI_C_NOME", SqlDbType.VarChar, 200, IgnoreEmpty = true)]
        public string IdiomaNome { get; set; }

        public string TrimestreLegenda
        {

            get {
                if (Trimestre.HasValue && Ano.HasValue)
                    return Trimestre + "T" + Ano;
                else
                    return string.Empty;
            }
        }
    }

    public class MLResultadoAno
    {

        [DataField("RES_N_ANO", SqlDbType.Decimal, 18)]
        public decimal? Ano { get; set; }
    }

    public class MLResultadoCidade
    {

        [DataField("nome", SqlDbType.VarChar, 100)]
        public string nome { get; set; }
    }
}
