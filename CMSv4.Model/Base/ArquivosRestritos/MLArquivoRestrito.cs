using System;
using System.Data;
using System.Web.Mvc;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ARE_ARQUIVO_RESTRITO")]
    [Auditing("/cms/ArquivoRestritoAdmin/arquivo", "Codigo")]    
    public class MLArquivoRestrito : BaseModel
    {        
        [DataField("ARE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ARE_ARC_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoria { get; set; }

		[DataField("ARE_ART_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoTipo { get; set; }

		[DataField("ARE_C_NOME", SqlDbType.VarChar, -1)]
        public string Nome { get; set; }

        [DataField("ARE_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("ARE_C_IMAGEM", SqlDbType.VarChar, 400, IgnoreEmpty = true)]
        public string Imagem { get; set; }

        [AllowHtml]
        [DataField("ARE_C_DESCRICAO", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        public string Descricao { get; set; }
        
        /// <summary>
        /// Campo deve conter o nome do arquivo ou a URL do video
        /// </summary>
		[DataField("ARE_C_ARQUIVO_URL", SqlDbType.VarChar, 400)]
        public string ArquivoUrl { get; set; }

        [DataField("ARE_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("ARE_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

		[JoinField("ARE_ART_N_CODIGO", "MOD_ARE_ARQUIVO_RESTRITO_TIPO", "ART_N_CODIGO", "ART_C_NOME")]
		[DataField("ART_C_NOME", SqlDbType.VarChar, 50)]
		public string Tipo { get; set; }
    }
}
