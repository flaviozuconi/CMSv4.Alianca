using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// Aniversariante
    /// </summary>
    [Serializable]
    public class MLModuloAniversariante
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("ANI_N_QUANTIDADE_DESTAQUE", SqlDbType.Int)]
        public int? QuantidadeDestaque { get; set; }

        [Required]
        [DataField("ANI_C_TITULO", SqlDbType.VarChar, 50)]
        public string Titulo { get; set; }

        [DataField("ANI_C_GRUPOS", SqlDbType.VarChar, -1)]
        public string Grupos { get; set; }

        [DataField("ANI_C_URL_LISTAGEM", SqlDbType.VarChar, 100)]
        public string UrlListagem { get; set; }

        [Required]
        [DataField("ANI_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("ANI_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("ANI_B_LISTAR_MES", SqlDbType.Bit)]
        public bool? ListarMes { get; set; }

        [DataField("ANI_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }

        [DataField("ANI_B_COMPACTO", SqlDbType.Bit)]
        public bool? ModoCompacto { get; set; }

        [DataField("ANI_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("ANI_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        public string DataString
        {
            get
            {
                if (DataInicio.HasValue && DataTermino.HasValue)
                    return String.Concat(BLTraducao.T("De"), " ", DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy")), " ", BLTraducao.T("até"), " ", DataTermino.Value.ToString(BLTraducao.T("dd/MM/yyyy")));
                else if (DataInicio.HasValue && !DataTermino.HasValue)
                    return String.Concat(BLTraducao.T("A partir de"), " ", DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy")));
                else if (!DataInicio.HasValue && DataTermino.HasValue)
                    return String.Concat(BLTraducao.T("Até"), " ", DataTermino.Value.ToString(BLTraducao.T("dd/MM/yyyy")));
                else
                    return String.Empty;
            }
        }
    }

    [Table("MOD_ANI_ANIVERSARIANTES_EDICAO")]
    public class MLModuloAniversarianteEdicao : MLModuloAniversariante { }

    [Table("MOD_ANI_ANIVERSARIANTES_PUBLICADO")]
    public class MLModuloAniversariantePublicado : MLModuloAniversariante { }

    [Table("MOD_ANI_ANIVERSARIANTES_HISTORICO")]
    public class MLModuloAniversarianteHistorico : MLModuloAniversariante
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
