using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    public class MLModuloArquivoRestrito
    {
        public MLModuloArquivoRestrito()
        {
            CodigosCategoriaArquivo = new List<decimal>();
        }

        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [StringLength(100)]
        [DataField("ARE_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("ARE_C_CATEGORIA", SqlDbType.VarChar, 100)]
        public string Categorias { get; set; }

        [DataField("ARE_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        public string Pasta { get; set; }

        public int Pagina { get; set; }
        
        public List<decimal> CodigosCategoriaArquivo { get; set; }
    }

    [Table("MOD_ARE_ARQUIVO_RESTRITO_EDICAO")]
    public class MLModuloArquivoRestritoEdicao : MLModuloArquivoRestrito { }

    [Table("MOD_ARE_ARQUIVO_RESTRITO_PUBLICADO")]
    public class MLModuloArquivoRestritoPublicado : MLModuloArquivoRestrito { }

    [Table("MOD_ARE_ARQUIVO_RESTRITO_HISTORICO")]
    public class MLModuloArquivoRestritoHistorico : MLModuloArquivoRestrito
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
