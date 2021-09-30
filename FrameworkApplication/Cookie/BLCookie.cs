using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Framework.Model;

namespace Framework.Utilities
{
    public class BLCookie
    {
        #region CarregarFiltroCookie

        /// <summary>
        /// Verifica se existe o cookie com as informações do filtro 
        /// </summary>
        /// <param name="pstrChaceCookie">Chave do Cookie</param>
        /// <user>rvissontai</user>
        public static MLCookie Carregar(string pstrChaceCookie)
        {
            MLCookie retorno = new MLCookie();
            HttpCookie cookie = HttpContext.Current.Request.Cookies[pstrChaceCookie];

            if (cookie != null && cookie.HasKeys)
            {
                retorno.Nome = cookie.Name;

                foreach (var i in cookie.Values.AllKeys)
                {
                    retorno.Valores.Add(new MLCookieValores { Chave = i.ToString(), Valor = cookie.Values[i] });
                }
                retorno.Expires = cookie.Expires;
            }

            return retorno;
        }

        public static TipoModel Carregar<TipoModel>(string pstrChaceCookie)
        {
            //Cria uma nova instancia da model genérica.
            var retorno = Activator.CreateInstance<TipoModel>();

            //Obter o coockie da chave especificada
            HttpCookie cookie = HttpContext.Current.Request.Cookies[pstrChaceCookie];

            if (cookie != null && cookie.HasKeys)
            {
                //ler as propriedades da model genérica
                var propriedades = typeof(TipoModel).GetProperties();

                //Procurar nas propriedads os fieldnames do cookie e setar a model
                foreach (var prop in propriedades)
                {
                    var dbField = prop.GetCustomAttributes(typeof(DataField), false);

                    //Verifica se existe o DateField
                    if (dbField == null || dbField.Length <= 0) continue;
                    var dataField = ((DataField)dbField[0]);

                    if (dataField.IsCookie)
                    {
                        if (!String.IsNullOrEmpty(cookie.Values[prop.Name]))
                            prop.SetValue(retorno, Convert.ChangeType(cookie.Values[prop.Name], prop.PropertyType), null);
                    }
                }
            }

            return retorno;
        }

        #endregion

        #region Criar

        /// <summary>
        /// Metodo para criar cookie
        /// </summary>
        /// <param name="pobjMLCookie"></param>
        /// <returns>HttpCookie</returns>
        /// <user>rvissontai</user>
        public static bool Criar(MLCookie pobjMLCookie)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(pobjMLCookie.Nome);

                foreach (var item in pobjMLCookie.Valores)
                    cookie.Values.Add(item.Chave, item.Valor);

                cookie.Expires = pobjMLCookie.Expires;

                //salvar cookie no disco
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }

            return true;
        }

        public static bool Criar<TipoModel>(TipoModel pobjML, string Chave, DateTime dtExpiration)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(Chave);

                var propriedades = typeof(TipoModel).GetProperties();

                //Procurar nas propriedads os fieldnames do cookie e setar a model
                foreach (var prop in propriedades)
                {
                    var dbField = prop.GetCustomAttributes(typeof(DataField), false);

                    //Verifica se existe o DateField
                    if (dbField == null || dbField.Length <= 0) continue;
                    var dataField = ((DataField)dbField[0]);

                    if (dataField.IsCookie)
                    {
                        var valor = prop.GetValue(pobjML, null);
                        
                        if (valor != null)
                            cookie.Values.Add(prop.Name, valor.ToString());
                    }
                }   

                cookie.Expires = dtExpiration;

                //salvar cookie no disco
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Deletar

        /// <summary>
        /// Deletar um cookie modificando a data de expiração
        /// </summary>
        /// <param name="pstrChaveCookie">Chave única do cookie</param>
        /// <user>rvissontai</user>
        public static void Deletar(string pstrChaveCookie)
        {
            if (HttpContext.Current.Request.Cookies[pstrChaveCookie] != null)
            {
                HttpCookie myCookie = new HttpCookie(pstrChaveCookie);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }

        #endregion
    }
}
