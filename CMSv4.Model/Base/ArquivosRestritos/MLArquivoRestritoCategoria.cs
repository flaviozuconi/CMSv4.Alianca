using System;
using System.Data;
using Framework.Model;
using Framework.Utilities;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_ARE_ARQUIVO_RESTRITO_CATEGORIA")]
    [Auditing("/cms/ArquivoRestritoAdmin", "CodigoPortal")]
    public class MLArquivoRestritoCategoria : BaseModel
    {
		[DataField("ARC_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
		public decimal? Codigo { get; set; }

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("ARC_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("ARC_C_TITULO", SqlDbType.VarChar, 200)]
        public string Titulo { get; set; }

        [DataField("ARC_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        /// <summary>
        /// Retorna o caminho da pasta física
        /// </summary>
        public string PastaFisicaArquivo(string diretorioPortal)
        {
            return System.Web.HttpContext.Current.Server.MapPath(this.PastaRelativaArquivo(diretorioPortal));
        }

        public string PastaFisicaImagem(string diretorioPortal)
        {
            return System.Web.HttpContext.Current.Server.MapPath(this.PastaRelativaImagem(diretorioPortal));
        }

        public string PastaRelativaArquivo(string diretorioPortal)
        {
            return string.Concat(BLConfiguracao.Pastas.ModuloArquivoRestritoArquivo(diretorioPortal), "/", this.Nome.Replace("/", ""));
        }

        public string PastaRelativaImagem(string diretorioPortal)
        {
            return string.Concat(BLConfiguracao.Pastas.ModuloArquivoRestritoImagem(diretorioPortal), "/", this.Nome.Replace("/", ""));
        }
    }
}
