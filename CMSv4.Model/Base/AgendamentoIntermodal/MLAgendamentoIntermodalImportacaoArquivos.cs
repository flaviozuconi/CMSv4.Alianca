using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model
{
    #region MLAgendamentoIntermodalArquivoDeclaracaoImportacao
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal Importacao
    /// </summary> 
    [Serializable]
    [Table("MOD_AIDI_ARQUIVO_IMPORTACAO_DECLARACAO_IMPORTACAO")]
    public class MLAgendamentoIntermodalArquivoDeclaracaoImportacao
    {
        [DataField("AIDI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIDI_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [StringLength(500)]
        [DataField("AIDI_C_ARQUIVO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("AIDI_B_INTEGRADO", SqlDbType.Bit)]
        public bool? isIntegrado { get; set; }
    }
    #endregion

    #region MLAgendamentoIntermodalArquivoGare
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal Importacao
    /// </summary> 
    [Serializable]
    [Table("MOD_AIGE_ARQUIVO_IMPORTACAO_GARE")]
    public class MLAgendamentoIntermodalArquivoGare
    {
        [DataField("AIGE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIGE_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [StringLength(500)]
        [DataField("AIGE_C_ARQUIVO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("AIGE_B_INTEGRADO", SqlDbType.Bit)]
        public bool? isIntegrado { get; set; }
    }
    #endregion

    #region MLAgendamentoIntermodalArquivoBl
    /// <summary> 
    /// Model da Entidade Agendamento Intermodal Importacao
    /// </summary> 
    [Serializable]
    [Table("MOD_AIBL_ARQUIVO_IMPORTACAO_BL")]
    public class MLAgendamentoIntermodalArquivoBl
    {
        [DataField("AIBL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("AIBL_AII_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoImportacao { get; set; }

        [StringLength(500)]
        [DataField("AIBL_C_ARQUIVO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("AIBL_B_INTEGRADO", SqlDbType.Bit)]
        public bool? isIntegrado { get; set; }
    }
    #endregion

}
