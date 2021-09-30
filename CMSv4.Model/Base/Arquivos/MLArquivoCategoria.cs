using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// CATEGORIA
    /// </summary>
    [Serializable]
    [Table("MOD_ARQ_CATEGORIAS_ARQUIVO")]
    [Auditing("/cms/arquivoadmin", "CodigoPortal")]
    public class MLArquivoCategoria : BaseModel
    {
        public MLArquivoCategoria()
        {
            CodigosGrupoLeitura = new List<decimal>();
        }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("ACA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ACA_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("ACA_C_TITULO", SqlDbType.VarChar, 200)]
        public string Titulo { get; set; }

        [DataField("ACA_GRP_N_CODIGO_EDICAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoEditor { get; set; }

        [DataField("ACA_GRP_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoAprovador { get; set; }

        [DataField("ACA_CAA_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCategoriaAgrupador { get; set; }

        public List<decimal> CodigosGrupoLeitura { get; set; }

        public IEnumerable<SelectListItem> ItensPermissaoEscrita { get; set; }

        public IEnumerable<SelectListItem> ItensPermissaoLeitura { get; set; }

        // JOIN

        public string NomeGrupoEdicao { get; set; }

        public string NomeGrupoAprovacao { get; set; }

        /// <summary>
        /// Retorna o caminho da pasta física
        /// </summary>
        public string PastaFisica(string diretorioPortal)
        {
            return string.Concat(System.Web.HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloArquivos(diretorioPortal)), "/", this.Nome.Replace("/", ""));
        }

        public string PastaRelativa(string diretorioPortal)
        {
            return string.Concat(BLConfiguracao.Pastas.ModuloArquivos(diretorioPortal), "/", this.Nome.Replace("/", ""));
        }
    }
}
