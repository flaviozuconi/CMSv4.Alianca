using System;
using System.Data;
using Framework.Model;
using System.Web.Mvc;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// ARQUIVO
    /// </summary>
    [Serializable]
    [Table("MOD_ARQ_ARQUIVOS")]
    public class MLArquivoLista
    {
        [DataField("ARQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ARQ_D_DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("ARQ_C_NOME", SqlDbType.VarChar, -1)]
        public string Nome { get; set; }

        [DataField("ARQ_C_PASTA_RELATIVA", SqlDbType.VarChar, -1)]
        public string Pasta { get; set; }

        [DataField("ARQ_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("ARQ_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [AllowHtml]
        [DataField("ARQ_C_DESCRICAO", SqlDbType.VarChar, -1)]
        public string Descricao { get; set; }

        [DataField("ARQ_B_DESTAQUE", SqlDbType.Bit)]
        public bool? Destaque { get; set; }

        [DataField("ARQ_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        public string DataString
        {
            get
            {
                if (Data.HasValue) return Data.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
                return "";
            }
        }

    }
}
