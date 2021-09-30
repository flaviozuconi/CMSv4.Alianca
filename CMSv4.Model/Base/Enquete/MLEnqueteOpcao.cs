using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;


namespace CMSv4.Model
{
    /// <summary>
    /// ENQUETE OPCAO
    /// </summary>
    [Serializable]
    [Table("MOD_ENQ_ENQUETE_OPCAO")]
    public class MLEnqueteOpcao
    {
        [DataField("EQO_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("EQO_ENQ_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEnquete { get; set; }

        [Required]
        [DataField("EQO_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("EQO_N_ORDEM", SqlDbType.Int)]
        public int? Ordem { get; set; }
    }

    /// <summary>
    /// ENQUETE OPCAO
    /// </summary>
    [Serializable]
    [Table("MOD_ENQ_ENQUETE_OPCAO")]
    public class MLEnqueteOpcaoResultado
    {
        [DataField("EQO_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("EQO_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("VOTOS", SqlDbType.Int)]
        public int? QuantidadeVotos { get; set; }

        [DataField("TOTAL", SqlDbType.Int)]
        public int? TotalEnquete { get; set; }

        public decimal? Porcentagem
        {
            get
            {
                if (TotalEnquete > 0)
                    return (QuantidadeVotos * 100) / TotalEnquete;
                return 0;
            }
        }

        public string label
        {
            get { return Titulo; }
        }
        public int data
        {
            get { return QuantidadeVotos.Value; }
        }

    }
}