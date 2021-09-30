using Framework.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.Model
{
    public class MLAgrupadorPublico
    {
        public MLAgrupadorPublico()
        {
            Categorias = new List<MLCategoriaAgrupador>();
            Conteudos = new List<MLAgrupadorConteudoPublico>();
        }

        public List<MLCategoriaAgrupador> Categorias { get; set; }

        public List<MLAgrupadorConteudoPublico> Conteudos { get; set; }
    }


    public class MLAgrupadorConteudoPublico
    {
        [DataField("CODIGO", SqlDbType.Decimal)]
        public decimal? Codigo { get; set; }

        [DataField("TIPO", SqlDbType.VarChar)]
        public string TipoBanco { get; set; }

        [DataField("LISTA", SqlDbType.Decimal)]
        public decimal? Lista { get; set; }

        [DataField("TITULO", SqlDbType.VarChar)]
        public string Titulo { get; set; }

        [DataField("CHAMADA", SqlDbType.VarChar)]
        public string Chamada { get; set; }

        [DataField("EXTRA1", SqlDbType.VarChar)]
        public string Extra1 { get; set; }

        [DataField("IMAGEM", SqlDbType.VarChar)]
        public string Imagem { get; set; }

        [DataField("TAGS", SqlDbType.VarChar)]
        public string Tags { get; set; }

        [DataField("DATA", SqlDbType.DateTime)]
        public DateTime? Data { get; set; }

        [DataField("URL", SqlDbType.VarChar)]
        public string Url { get; set; }

        [DataField("URL_DETALHE", SqlDbType.VarChar)]
        public string UrlDetalhe { get; set; }        
   
        [DataField("CAA_C_NOME", SqlDbType.VarChar)]
        public string NomeCategoriaAgrupador { get; set; }

        [DataField("CAA_N_CODIGO", SqlDbType.Decimal)]
        public decimal? CategoriaAgrupador { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public int? TotalRows { get; set; }

        public string DataString
        {
            get
            {
                if (Data.HasValue) return Data.Value.ToString(BLTraducao.T("dd/MM/yyyy"));
                return "";
            }
        }

        public string UrlDetalheDisplay(List<decimal> links)
        {
            var portal = BLPortal.Atual;

            if (TipoBanco == "A")
            {
                return string.Format("{0}/download/{1}/{2}", Portal.Url(), portal.Diretorio, Codigo);
            }
            else
            {
                if (links != null && links.Contains(Lista.Value))
                    return Url;

                return string.Format("{0}/{1}/{2}", Portal.UrlDiretorio(portal), (UrlDetalhe ?? string.Empty).TrimStart('/'), (Url ?? string.Empty).TrimStart('/'));
            }
        }
    }

}
