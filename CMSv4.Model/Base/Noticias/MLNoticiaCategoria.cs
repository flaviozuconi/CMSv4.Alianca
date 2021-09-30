using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// NOTICIA
    /// </summary>
    [Serializable]
    [Table("MOD_NOT_CATEGORIAS_NOTICIA")]
    public class MLNoticiaCategoria : BaseModel
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("NCA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }
        
        [DataField("NCA_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("NCA_GRP_N_CODIGO_EDICAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoEditor { get; set; }

        [DataField("NCA_GRP_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoAprovador { get; set; }


        // JOIN

        public string NomeGrupoEdicao { get; set; }

        public string NomeGrupoAprovacao { get; set; }

    }
}
