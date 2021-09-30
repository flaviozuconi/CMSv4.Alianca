using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// ENQUETE VOTO
    /// </summary>
    [Serializable]
    [Table("MOD_ENQ_ENQUETE_VOTO")]
    public class MLEnqueteVoto
    {
        [DataField("EQV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("EQV_EQO_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoOpcao { get; set; }
                
        [DataField("EQV_D_RESPOSTA", SqlDbType.DateTime)]
        public DateTime? DataResposta { get; set; }

        [DataField("EQV_C_IP", SqlDbType.VarChar, 20)]
        public string IP { get; set; }

        [DataField("EQP_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }
     
    }

    public class MLEnqueteVotoRelatorio
    {
        [DataField("EQO_C_TITULO", SqlDbType.VarChar, 250)]        
        public string TituloOpcao { get; set; }

        [DataField("EQV_D_RESPOSTA", SqlDbType.DateTime)]
        public DateTime? DataResposta { get; set; }

        [DataField("EQV_C_IP", SqlDbType.VarChar, 20)]
        public string IP { get; set; }
    }

}