using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ARE_ARQUIVO_RESTRITO_CATEGORIA_X_GRUPO_CLIENTE")]    
    public class MLArquivoRestritoCategoriaGrupoCliente
    {
        [DataField("ACG_ARC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoCategoria { get; set; }

        [DataField("ACG_GLC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoGrupoCliente { get; set; }
    }
}
