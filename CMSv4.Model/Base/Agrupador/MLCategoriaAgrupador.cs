using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_CAA_CATEGORIA_AGRUPADOR")]
    public class MLCategoriaAgrupador
    {
        [DataField("CAA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [Required]
        [DataField("CAA_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("CAA_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("CAA_C_CLASSE", SqlDbType.VarChar, 100)]
        public string Classe { get; set; }

        [DataField("CAA_B_EXIBIR_CADASTRO", SqlDbType.Bit)]
        public bool? ExibirNoCadastroDeCliente { get; set; }

        [DataField("CAA_B_FILTRO_SECUNDARIO", SqlDbType.Bit)]
        public bool? FiltroSecundario { get; set; }

        [DataField("CAA_N_ORDEM", SqlDbType.Int)]
        public int? Ordem { get; set; }

        [DataField("CAA_C_URL_AMIGAVEL", SqlDbType.VarChar, 200)]
        public string UrlAmigavel { get; set; }
    }

    [Serializable]
    [Table("MOD_AGR_AGRUPADOR")]
    public class MLAgrupador
    {
        [DataField("AGR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [Required]
        [DataField("AGR_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("AGR_C_CONFIG", SqlDbType.VarChar, -1)]
        public string Config { get; set; }

        [DataField("AGR_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }

    public class MLAgrupadorAnos
    {
        [DataField("ANO", SqlDbType.Int)]
        public int Ano { get; set; }
    }
}
