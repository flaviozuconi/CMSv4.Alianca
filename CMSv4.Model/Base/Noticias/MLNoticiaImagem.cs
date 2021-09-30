using System;
using System.Data;
using Framework.Utilities;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// NOTICIA
    /// </summary>
    [Serializable]
    [Table("MOD_NOT_NOTICIAS_GALERIA")]
    public class MLNoticiaImagem : BaseModel
    {
        [DataField("NOI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("NOI_NOT_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoNoticia { get; set; }

        [DataField("NOI_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("NOI_C_TEXTO", SqlDbType.VarChar, -1)]
        public string Texto { get; set; }

        [DataField("NOI_N_ORDEM", SqlDbType.Int,IgnoreEmpty=true)]
        public int? Ordem { get; set; }

        [DataField("NOI_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("NOI_C_FONTE", SqlDbType.VarChar, 250)]
        public string Fonte { get; set; }

        [DataField("NOI_C_ALTERNATE", SqlDbType.VarChar, 250)]
        public string Alternate { get; set; }

    }
}
