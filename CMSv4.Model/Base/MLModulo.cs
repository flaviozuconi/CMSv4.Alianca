using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("CMS_MOD_MODULO")]
    public class MLModulo
    {
        [DataField("MOD_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("MOD_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MOD_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("MOD_C_URL", SqlDbType.VarChar, 50)]
        public string Url { get; set; }

        [DataField("MOD_C_NOME_ASSEMBLY", SqlDbType.VarChar, 150)]
        public string NomeAssembly { get; set; }

        [DataField("MOD_C_NOME_BUSINESSLAYER", SqlDbType.VarChar, 150)]
        public string NomeBusinesLayer { get; set; }

        [DataField("MOD_B_EDITAVEL", SqlDbType.Bit)]
        public bool? Editavel { get; set; }

        [DataField("MOD_GRP_N_CODIGO_APROVADOR", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoAprovador { get; set; }

        [DataField("MOD_LIS_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoLista { get; set; }

        [DataField("MOD_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }
    }
}
