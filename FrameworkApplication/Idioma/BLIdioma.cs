using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Framework.Utilities
{
    public class BLIdioma
    {
        public static decimal? CodigoAtual
        {
            get
            {
                return Atual.Codigo;
            }
        }

        public static MLIdioma Atual
        {
            get
            {
                var url = HttpContext.Current.Request.Url.ToString();


                var obj = HttpContext.Current.Session["idioma-atual-publico"];
                if (obj != null)
                {
                    return obj as MLIdioma;
                }


                return new MLIdioma
                {
                    Codigo = 1,
                    Ativo = true,
                    Nome = "Português - Brasil",
                    Sigla = "pt-BR",
                    Iniciado = false
                };
            }
            set
            {
                HttpContext.Current.Session["idioma-atual-publico"] = value;
            }
        }


        public static MLIdioma AtualAdm
        {
            get
            {             
                var obj = HttpContext.Current.Session["idioma-atual-admin"];
                if (obj != null)
                {
                    return obj as MLIdioma;
                }


                return new MLIdioma
                {
                    Codigo = 1,
                    Ativo = true,
                    Nome = "Português - Brasil",
                    Sigla = "pt-BR",
                    Iniciado = false
                };
            }
            set
            {
                HttpContext.Current.Session["idioma-atual-admin"] = value;
            }
        }


        /// <summary>
        /// Listar todos os idiomas ativos
        /// </summary>
        public static List<MLIdioma> Listar(bool? cache = false)
        {

            string strCache = string.Format("ListarIdiomaPublico");
            var retorno = BLCachePortal.Get<List<MLIdioma>>(strCache);
            if (retorno != null && cache == true) return retorno;

            var rtn = new CRUD.Select<MLIdioma>()
                    .Equals(a => a.Ativo, true)
                    .ToList();
            if (cache == true)
            {
                BLCachePortal.Add(strCache, retorno);
            }
            return rtn;
        }

        public static string ObterSiglaParaMomentJs()
        {
            var splitedSigla = AtualAdm.Sigla.Split('-');

            if (splitedSigla.Length > 0)
                return splitedSigla[0];

            return "";
        }
    }
}
