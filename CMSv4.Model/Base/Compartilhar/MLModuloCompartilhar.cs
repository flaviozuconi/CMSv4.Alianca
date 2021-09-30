using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    public class MLModuloCompartilhar
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("CMP_C_VIEW", SqlDbType.VarChar, 200)]
        public string View { get; set; }

        [DataField("CMP_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("CMP_C_CSS", SqlDbType.VarChar, 100)]
        public string Css { get; set; }

        [DataField("CMP_POR_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoPortal { get; set; }

        [DataField("CMP_USU_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("CMP_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }
    }

    [Table("MOD_CMP_COMPARTILHAR_EDICAO")]
    public class MLModuloCompartilharEdicao : MLModuloCompartilhar { }

    [Table("MOD_CMP_COMPARTILHAR_PUBLICADO")]
    public class MLModuloCompartilharPublicado : MLModuloCompartilhar { }

    [Table("MOD_CMP_COMPARTILHAR_HISTORICO")]
    public class MLModuloCompartilharHistorico : MLModuloCompartilhar 
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
