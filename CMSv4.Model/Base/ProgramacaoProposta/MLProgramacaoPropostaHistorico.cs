using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model.Base
{
    [Table("MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_PROPOSTA_HISTORICO")]
    public class MLProgramacaoPropostaHistorico
    {
        [DataField("PPH_N_CODIGO",SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PPH_USU_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("PPH_D_DATA_IMPORTACAO",SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("PPH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }

        [DataField("PPH_B_FINALIZADO",SqlDbType.Bit)]
        public bool? Finalizado { get; set; }

        [DataField("PPH_C_ARQUIVO",SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }

        [JoinField("PPH_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_NOME")]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeUsuario { get; set; }
    }
}
