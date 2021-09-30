using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    public class MLModuloArquivos
    {
        public MLModuloArquivos()
        {
            CodigosCategoriaArquivo = new List<decimal>();
        }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("ARQ_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [StringLength(100)]
        [DataField("ARQ_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [DataField("ARQ_C_CATEGORIAS", SqlDbType.VarChar, 100)]
        public string Categorias { get; set; }

        [StringLength(100)]
        [DataField("ARQ_C_URL_LISTA", SqlDbType.VarChar, 100)]
        public string UrlLista { get; set; }

        [StringLength(100)]
        [DataField("ARQ_C_URL_DETALHE", SqlDbType.VarChar, 100)]
        public string UrlDetalhe { get; set; }

        [DataField("ARQ_C_ORDEM_DATA", SqlDbType.Bit)]
        public bool? OrdenarData { get; set; }

        [DataField("ARQ_C_ORDEM_DESC", SqlDbType.Bit)]
        public bool? OrdenarDesc { get; set; }

        [DataField("ARQ_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("ARQ_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        [DataField("ARQ_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [StringLength(100)]
        [DataField("ARQ_C_PAGINACAO", SqlDbType.VarChar, 100)]
        public string TipoPaginacao { get; set; }

        public string Pasta { get; set; }

        public int Pagina { get; set; }
        
        public List<decimal> CodigosCategoriaArquivo { get; set; }
    }

    [Table("MOD_ARQ_ARQUIVO_EDICAO")]
    public class MLModuloArquivosEdicao : MLModuloArquivos { }

    [Table("MOD_ARQ_ARQUIVO_PUBLICADO")]
    public class MLModuloArquivosPublicado : MLModuloArquivos { }

    [Table("MOD_ARQ_ARQUIVO_HISTORICO")]
    public class MLModuloArquivosHistorico : MLModuloArquivos
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
