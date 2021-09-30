using System;
using System.Collections.Generic;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_MUL_MULTIMIDIA_CATEGORIAS")]
    [Auditing("/cms/multimidiaadmin", "CodigoPortal")]
    public class MLMultimidiaCategoria : BaseModel
    {
        public MLMultimidiaCategoria()
        {
            CodigosGrupoLeitura = new List<decimal>();
        }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MCA_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MCA_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("MCA_C_TITULO", SqlDbType.VarChar, 200)]
        public string Titulo { get; set; }

        [DataField("MCA_GRP_N_CODIGO_EDICAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoEditor { get; set; }

        [DataField("MCA_GRP_N_CODIGO_APROVACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupoAprovador { get; set; }

        public List<decimal> CodigosGrupoLeitura { get; set; }

        // JOIN

        public string NomeGrupoEdicao { get; set; }

        public string NomeGrupoAprovacao { get; set; }

        /// <summary>
        /// Retorna o caminho da pasta física
        /// </summary>
        public string PastaFisica(string diretorioPortal)
        {
            return string.Concat(System.Web.HttpContext.Current.Server.MapPath(BLConfiguracao.Pastas.ModuloMultimidia(diretorioPortal)), "/", this.Nome.Replace("/", ""));
        }

        public string PastaRelativa(string diretorioPortal)
        {
            return string.Concat(BLConfiguracao.Pastas.ModuloMultimidia(diretorioPortal), "/", this.Nome.Replace("/", ""));
        }
    }
}
