using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_MUL_CATEGORIAS_MULTIMIDIA_X_GRUPO_CLIENTE")]
    public class MLMultimidiaCategoriaGrupoCliente
    {
        [DataField("MCG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MCG_MCA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoArquivoCategoria { get; set; }

        [DataField("MCG_GCL_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoLeitura { get; set; }        
    }
}
