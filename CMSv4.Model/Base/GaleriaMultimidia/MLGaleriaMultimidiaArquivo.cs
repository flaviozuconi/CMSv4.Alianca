using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// GALERIA DE IMAGEM - ARQUIVO
    /// </summary>
    [Table("MOD_GAL_GALERIAS_ARQUIVOS")]
    public class MLGaleriaMultimidiaArquivo : BaseModel
    {
        [DataField("GAA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("GAA_GAL_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGaleria { get; set; }

        [DataField("GAA_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("GAA_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("GAA_N_ORDEM", SqlDbType.Int)]
        public int? Ordem { get; set; }

        [DataField("GAA_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("GAA_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("GAA_N_TIPO", SqlDbType.Int)]
        public int? Tipo { get; set; }

        //arquivo
        [DataField("GAA_C_ARQUIVO", SqlDbType.VarChar, 500)]
        public string Arquivo { get; set; }

        [DataField("GAA_C_IMAGEM", SqlDbType.VarChar, 500)]
        public string Imagem { get; set; }

        [DataField("GAA_C_ALTERNATE", SqlDbType.VarChar, 250)]
        public string AlternateText { get; set; }

        [DataField("GAA_C_YOUTUBE", SqlDbType.VarChar, 250)]
        public string YouTube { get; set; }

        [DataField("GAA_C_URL", SqlDbType.VarChar, 250)]
        public string Url { get; set; }

        public string TipoLabel
        {
            get
            {
                if (Tipo == 1)
                {
                    return "Imagem";
                }
                else if (Tipo == 2)
                {
                    return "Vídeo";
                }
                else if (Tipo == 3)
                {
                    return "Infográfico";
                }
                
                return "";
            }
        }

        public bool? ImagemVertical { get; set; }
    }
}
