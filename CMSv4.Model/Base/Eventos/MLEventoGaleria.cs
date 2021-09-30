using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// Galeria de fotos do evento
    /// </summary>
    [Serializable]
    [Table("MOD_EVE_EVENTOS_GALERIA")]
    public class MLEventoGaleria : BaseModel
    {
        [DataField("EVI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("EVI_EVE_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEvento { get; set; }

        [DataField("EVI_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("EVI_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("EVI_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

        [DataField("EVI_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("EVI_C_FONTE", SqlDbType.VarChar, 250)]
        public string Fonte { get; set; }

        [DataField("EVI_C_ALTERNATE", SqlDbType.VarChar, 250)]
        public string Alternate { get; set; }

    }
}
