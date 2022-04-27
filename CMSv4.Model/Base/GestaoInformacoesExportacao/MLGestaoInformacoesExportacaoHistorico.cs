using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model.Base.GestaoInformacoesExportacao
{
    [Table("MOD_GIE_GESTAO_INFORMACOES_EXPORTACAO_HISTORICO")]
    public class MLGestaoInformacoesExportacaoHistorico
    {
        [DataField("IEH_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("IEH_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("IEH_C_USUARIO", SqlDbType.VarChar, 100)]
        public string Usuario { get; set; }

        [DataField("IEH_B_SUCESSO", SqlDbType.Bit)]
        public bool? Sucesso { get; set; }

        [DataField("IEH_C_ARQUIVO", SqlDbType.VarChar, 200)]
        public string Arquivo { get; set; }

        [DataField("IEH_B_FINALIZADO", SqlDbType.Bit)]
        public bool? Finalizado { get; set; }

        
    }
}
