using Framework.Model;
using System;
using System.Data;


namespace CMSv4.Model.Base.GestaoInformacoesImportacao
{
    [Table("MOD_GII_GESTAO_INFORMACOES_IMPORTACAO_HISTORICO")]
    public class MLGestaoInformacoesImportacaoHistorico
    {
        [DataField("IIH_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("IIH_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("IIH_C_USUARIO", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("IIH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }

        [DataField("IIH_C_ARQUIVO", SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }

        [DataField("IIH_B_FINALIZADO", SqlDbType.Bit)]
        public bool? Finalizado { get; set; }
    }
}
