using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ARQ_CATEGORIAS_ARQUIVO_X_GRUPO_CLIENTE")]
    public class MLArquivoCategoriaGrupoCliente
    {
        [DataField("ACG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ACG_ACA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoArquivoCategoria { get; set; }        
        
        [DataField("ACG_GCL_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoLeitura { get; set; }        
    }
}
