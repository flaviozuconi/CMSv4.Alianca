using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Framework.Utilities
{
    public static class BLExtentionMethod
    {
        #region HtmlDecode

        /// <summary>
        /// Decode Html
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>string</returns>
        public static string HtmlDecode(this string texto)
        {
            return HttpUtility.HtmlDecode(texto);
        }

        #endregion

        #region HtmlUnescapeDecode

        /// <summary>
        /// Decode Html
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>string</returns>
        public static string HtmlUnescapeDecode(this string texto)
        {
            return HttpUtility.HtmlDecode(Microsoft.JScript.GlobalObject.unescape(texto));
        }

        #endregion

        public static string Escape(this string texto)
        {
            return Microsoft.JScript.GlobalObject.escape(texto);
        }

        public static string Unescape(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";
                
            return Microsoft.JScript.GlobalObject.unescape(texto);
        }

        #region RemoverAcentos
        /// <summary>
        /// Remover acentos
        /// </summary>
        public static string RemoverAcentos(this string palavra)
        {
            if (string.IsNullOrWhiteSpace(palavra))
                return string.Empty;

            string palavraSemAcento = null;
            string caracterComAcento = "áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ*[](){}|¹²³";
            string caracterSemAcento = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC-----------";

            palavra = palavra.Replace('*', '-');
            palavra = palavra.Replace('[', '-');
            palavra = palavra.Replace(']', '-');
            palavra = palavra.Replace('(', '-');
            palavra = palavra.Replace(')', '-');
            palavra = palavra.Replace('{', '-');
            palavra = palavra.Replace('}', '-');
            palavra = palavra.Replace('|', '-');
            palavra = palavra.Replace('¹', '-');
            palavra = palavra.Replace('²', '-');
            palavra = palavra.Replace('³', '-');

            for (int i = 0; i < palavra.Length; i++)
            {
                if (caracterComAcento.IndexOf(Convert.ToChar(palavra.Substring(i, 1))) >= 0)
                {
                    int car = caracterComAcento.IndexOf(Convert.ToChar(palavra.Substring(i, 1)));
                    palavraSemAcento += caracterSemAcento.Substring(car, 1);
                }
                else
                {
                    palavraSemAcento += palavra.Substring(i, 1);
                }
            }

            return palavraSemAcento;
        }

        #endregion

        #region RemoverHtmlTags

        /// <summary>
        /// Remove as tags HTMl do texto
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>string</returns>
        public static string RemoveHtmlTags(this string texto)
        {
            string retorno = Regex.Replace(texto, @"<[^>]*>", string.Empty);
            retorno = retorno.Replace("\n", " ").Replace("\t", "").Replace("\r", "");
            retorno = Regex.Replace(retorno, @"\s+", " ");

            return retorno;
        }

        #endregion

        #region SomenteNumeros

        /// <summary>
        /// Remove todos os caracteres de uma string que não sejam números.
        /// </summary>
        /// <returns>string</returns>
        /// <user>rvissontai</user>
        public static string SomenteNumeros(this string valor)
        {
            return new Regex("[^0-9 -]").Replace(valor, "");
        }

        #endregion

        #region toUrlAmigavel

        public static string toUrlAmigavel(this string value)
        {
            value = value.RemoverAcentos();
            value = Regex.Replace(value, "[^0-9a-zA-Z ]+", "");
            
            return value.Replace(" ", "-"); ;
        }

        #endregion

        #region UpperPrimeiroChar

        /// <summary>
        /// Somente o primeiro caracter maiusculo.
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>string</returns>
        /// <user>rvissontai</user>
        public static string UpperPrimeiroChar(this string texto)
        {
            if(!String.IsNullOrWhiteSpace(texto) && texto.Length > 1)
                return char.ToUpper(texto[0]) + texto.Substring(1).ToLower();
            return texto;
        }
 
	    #endregion

        #region NomeMesExtenso

        /// <summary>
        /// Obter o nome do mês por extenso, ex: Janeiro.
        /// </summary>
        /// <returns>String</returns>
        public static string NomeMesExtenso(this DateTime data)
        {
            return data.ToString("MMMM", new CultureInfo("pt-BR"));
        }

        #endregion

        #region LimiteCaracter
		
        /// <summary>
        /// Função para limitar tamanho de caracter na exibição
        /// </summary>
        /// <param name="Tamanho">Tamanho máximo de caracteres permitidos</param>
        /// <param name="IncluirReticencia"></param>
        /// <returns>string</returns>
   
        public static MvcHtmlString LimiteCaracter(this string texto, int Tamanho, bool IncluirReticencia = true)
        {
            if (!String.IsNullOrEmpty(texto))
            {
                if (texto.Length > Tamanho)
                {
                    var indexOf = texto.Substring(0, Tamanho).LastIndexOf(' ');
                    var stringGerado = string.Empty;

                    if (indexOf > -1)
                        stringGerado = texto.Substring(0, indexOf - (IncluirReticencia ? 3 : 0));
                    else
                        stringGerado = texto.Substring(0, Tamanho - (IncluirReticencia ? 3 : 0));

                    if (IncluirReticencia)
                        return MvcHtmlString.Create(string.Concat(stringGerado.Trim(), "..."));

                    return MvcHtmlString.Create(stringGerado.Trim());
                }
                
                return MvcHtmlString.Create(texto);
            }

            return MvcHtmlString.Empty;
        }
 
	    #endregion

        #region Twitter

        public static string Link(this string s, string url)
        {
            if (!String.IsNullOrWhiteSpace(url) && !url.StartsWith("http"))
                url = "http://" + url;

            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, s);
        }
        public static string ParseURL(this string s)
        {
            if(!String.IsNullOrEmpty(s))
                return Regex.Replace(s, @"(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", new MatchEvaluator(BLExtentionMethod.URL));
            return String.Empty;
        }
        public static string ParseUsername(this string s)
        {
            return Regex.Replace(s, "(@)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(BLExtentionMethod.Username));
        }
        public static string ParseHashtag(this string s)
        {
            return Regex.Replace(s, "(#)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(BLExtentionMethod.Hashtag));
        }
        private static string Hashtag(Match m)
        {
            string x = m.ToString();
            string tag = x.Replace("#", "%23");
            return x.Link("http://search.twitter.com/search?q=" + tag);
        }
        private static string Username(Match m)
        {
            string x = m.ToString();
            string username = x.Replace("@", "");
            return x.Link("http://twitter.com/" + username);
        }
        private static string URL(Match m)
        {
            string x = m.ToString();
            return x.Link(x);
        }

        #endregion
    }
}
