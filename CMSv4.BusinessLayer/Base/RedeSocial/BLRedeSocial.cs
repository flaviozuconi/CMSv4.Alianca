using CMSv4.Model;
using Framework.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace CMSv4.BusinessLayer
{
    public class BLRedeSocial
    {
        #region Variavel privada

        /// <summary>Script do twitter para exibir a linha do tempo com as ultimas postagens </summary>
        private const string SCRIPT_TWITTER = "<script src=\"http://api.twitter.com/1.1/statuses/user_timeline/{0}.json?include_rts=true&callback=twitterCallback2&count=5\" type=\"text/javascript\"></script>";

        /// <summary>URL para obter o token do aplicativo</summary>
        private const string URL_TOKEN = "https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id={0}&client_secret={1}";

        /// <summary>URL para obter toda linha do tempo do cliente com as ultimas postagens</summary>
        private const string URL_LINHA_TEMPO = "https://graph.facebook.com/{0}/feed?access_token={1}&limit={2}&fields=id,message,link,created_time,from";

        /// <summary>URL para obter postagens do perfil no Linkedin</summary>
        private const string URL_LINKEDIN_FEED = "https://api.linkedin.com/v1/companies/{0}/updates?count={1}&format=json&&oauth2_access_token={2}";

        #endregion

        #region ListarFeeds

        private static void DefinirValoresPadrao(MLModuloRedeSocial model)
        {
            #region Default Facebook

            if (String.IsNullOrWhiteSpace(model.IdPagina))
                model.IdPagina = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.IdPagina"];

            if (String.IsNullOrWhiteSpace(model.FaceAppID))
                model.FaceAppID = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.AppId"];

            if (String.IsNullOrWhiteSpace(model.FaceAppIDSecret))
                model.FaceAppIDSecret = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Facebook.AppIdSecret"];

            #endregion

            #region Twitter

            if (String.IsNullOrWhiteSpace(model.TwitterPagina))
                model.TwitterPagina = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Twitter.User"];

            if (String.IsNullOrWhiteSpace(model.TwitterConsumer))
                model.TwitterConsumer = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Twitter.Consumer"];

            if (String.IsNullOrWhiteSpace(model.TwitterConsumerSecret))
                model.TwitterConsumerSecret = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Twitter.ConsumerSecret"];

            if (String.IsNullOrWhiteSpace(model.TwitterToken))
                model.TwitterToken = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Twitter.ConsumerToken"];

            if (String.IsNullOrWhiteSpace(model.TwitterTokenSecret))
                model.TwitterTokenSecret = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.Twitter.TokenSecret"];

            #endregion

            #region LinkedIn

            if (String.IsNullOrWhiteSpace(model.LinkedinCompany))
                model.LinkedinCompany = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.LinkedIn.Company"];

            if (String.IsNullOrWhiteSpace(model.LinkedinToken))
                model.LinkedinToken = ConfigurationManager.AppSettings["CMS.Modulos.RedeSocial.LinkedIn.Token"];

            #endregion
        }

        public static List<MLFeeds> ListarFeeds(MLModuloRedeSocial model)
        {
            //Para Testes
            //PaginaId = "452296324804718";
            //pAppId = "1750650168495695";
            //appsecretid = "4a3b97f46194c66a270490488495f24d";

            //valores default do webconfig
            DefinirValoresPadrao(model);

            List<MLFeeds> retorno = null;

            //Definir quantidade Default
            if (!model.Quantidade.HasValue)
                model.Quantidade = 30;

            //Definir quantidade de itens para cada rede social de acordo com a quantidade total do móduço
            decimal decDivisor = 0;

            if (model.IsFacebook.GetValueOrDefault()) decDivisor++;
            if (model.IsTwitter.GetValueOrDefault()) decDivisor++;
            if (model.IsLinkedin.GetValueOrDefault()) decDivisor++;

            if (decDivisor > 0)
            {
                //Quantidade de posts de cada uma das redes sociais.
                decimal decQuantidade = Convert.ToDecimal(Math.Ceiling((decimal)model.Quantidade / decDivisor));

                var portal = BLPortal.Atual;
                
                var lstMLFacePost = new List<MLFacePost>();
                var lstMLTwitterPost = new List<MLTwitter>();
                var objLinkedIn = new MLLinkedIn();

                #region Facebook

                if (model.IsFacebook.GetValueOrDefault()
                    && !string.IsNullOrWhiteSpace(model.FaceAppID)
                    && !string.IsNullOrWhiteSpace(model.FaceAppIDSecret))
                {
                    //Gerar token de acesso
                    var token = GerarTokenFacebook(model.FaceAppID, model.FaceAppIDSecret);

                    //Obter Json com os posts do facebook
                    string strJsonFb = CarregarJsonFacebook(model.IdPagina, token, decQuantidade.ToString());

                    //Serializar Json para model
                    lstMLFacePost = CarregarFacebook(strJsonFb, model.IdPagina);
                }

                #endregion

                #region Twitter

                if (model.IsTwitter.GetValueOrDefault()
                    && !string.IsNullOrWhiteSpace(model.TwitterPagina)
                    && !string.IsNullOrWhiteSpace(model.TwitterConsumer)
                    && !string.IsNullOrWhiteSpace(model.TwitterToken)
                    && !string.IsNullOrWhiteSpace(model.TwitterConsumerSecret)
                    && !string.IsNullOrWhiteSpace(model.TwitterTokenSecret)
                    )
                {
                    //Obter Json com os posts do Twitter
                    var strJsonTwitter = CarregarJsonTwitter(model.TwitterPagina, model.TwitterConsumer, model.TwitterToken, model.TwitterConsumerSecret, model.TwitterTokenSecret, (int)decQuantidade);

                    //Serializar Json para Model.
                    lstMLTwitterPost = CarregarTwitter(strJsonTwitter);
                }

                #endregion

                #region Linkedin

                if (model.IsLinkedin.GetValueOrDefault()
                    && !string.IsNullOrWhiteSpace(model.LinkedinCompany)
                    && !string.IsNullOrWhiteSpace(model.LinkedinToken))
                {
                    //Obter Json com os posts do LinkedIn
                    var strJsonLinkedin = CarregarJsonLinkedIn(model.LinkedinCompany, model.LinkedinToken, decQuantidade.ToString());

                    //Serializar json para model
                    objLinkedIn = CarregarLinekedin(strJsonLinkedin);
                }

                #endregion

                //Converter o feed das redes sociais em uma única lista
                retorno = ConverterFeeds(lstMLFacePost, lstMLTwitterPost, objLinkedIn);

                //Em números impares, ao calcular o divisor com arredondamento para cima, pode ocorrer
                //de ter mais registros do que a quantidade configurada na propriedade do módulo.
                //Remover os registros excedentes
                if (retorno.Count > model.Quantidade)
                    retorno.RemoveRange((int)model.Quantidade, (int)(retorno.Count - model.Quantidade));
            }

            return retorno ?? new List<MLFeeds>();
        }

        /// <summary>
        /// Buscar os feeds das redes socias configuradas, e renderizar o conteúdo na view e controller especificados
        /// </summary>
        /// <param name="model">MLModuloRedeSocial com as conifigurações do módulo</param>
        /// <param name="controller">Controller que esta chamando este método</param>
        /// <param name="view">Nome da View que será renderizada com os itens do feed</param>
        /// <returns>Html com o conteúdo renderizado</returns>
        public static string ListarFeedsHtml(MLModuloRedeSocial model, System.Web.Mvc.Controller controller, string view)
        {

            //Verificar se o html com as informações estão no cache, caso não esteja, renderizar os itens.
            string keyCacheHtml = String.Format("redes_sociais_html_lista_item_{0}_{1}_{2}", model.Quantidade, model.CodigoPagina, model.Repositorio);
            string htmlOutput = BLCachePortal.Get<string>(keyCacheHtml);

            if (String.IsNullOrWhiteSpace(htmlOutput))
            {
                var feeds = ListarFeeds(model);

                if (feeds != null && model.LimiteChar.HasValue)
                {
                    foreach (var item in feeds)
                        if (!string.IsNullOrWhiteSpace(item.Mensagem) && item.Mensagem.Length - model.LimiteChar > 0)
                            item.Mensagem = item.Mensagem.Substring(0, model.LimiteChar.Value) + "...";
                }

                htmlOutput = BLConteudoHelper.RenderPartialViewToString(controller, "Modulo", "RedeSocial", view, feeds);

                BLCachePortal.Add(keyCacheHtml, htmlOutput, 5);
            }

            return htmlOutput;
        }

        #endregion

        #region Métodos Privados

        #region Facebook

        #region Gerar token facebook
        /// <summary>
        /// Metodo utilizado para gerar o token de 
        /// acesso do facebook.
        /// </summary>
        /// <param name="pAppId">Id do aplicativo</param>
        /// <param name="pstrIdAppSecret">Id do aplicativo secret</param>
        /// <returns>Token de acesso</returns>
        /// <user>vmoura</user>
        private static string GerarTokenFacebook(string pAppId, string appsecretid)
        {
            string strRetornoToken = String.Empty;
            HttpWebResponse hwrResponse = null;
            try
            {
                HttpWebRequest wqtRequest = (HttpWebRequest)WebRequest.Create(String.Format(URL_TOKEN, pAppId, appsecretid));
                wqtRequest.Accept = "application/json";
                wqtRequest.Method = "GET";

                hwrResponse = (HttpWebResponse)wqtRequest.GetResponse();

                using (var sr = new StreamReader(hwrResponse.GetResponseStream()))
                {
                    strRetornoToken += sr.ReadToEnd();
                }

                dynamic dynamicJsonObject = JObject.Parse(strRetornoToken);

                if (dynamicJsonObject != null && dynamicJsonObject.access_token != null && !string.IsNullOrWhiteSpace(dynamicJsonObject.access_token.Value))
                    strRetornoToken = dynamicJsonObject.access_token;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            finally
            {
                hwrResponse.Close();
            }

            return strRetornoToken;
        }

        #endregion

        #region Carregar json facebook
        /// <summary>
        /// Metodo utilizado para carregar as novas
        /// publicações na pagina do facebook, atraves do token, 
        /// id da pagina que servirá de font de dados.
        /// </summary>       
        /// <param name="pstrAppId">Id do aplicativo que será feito a leitura da linha do tempo</param>
        /// <param name="pstrToken">Token de acesso</param>
        /// <returns>Json com a linha do tempo</returns>
        /// <user>vmoura</user>
        private static string CarregarJsonFacebook(string pstrIdPagina, string pstrToken, string pstrQuantidade)
        {
            string strRetonoJson = String.Empty;
            HttpWebResponse hwrResponse = null;
            try
            {
                HttpWebRequest wqtRequest = (HttpWebRequest)WebRequest.Create(String.Format(URL_LINHA_TEMPO, pstrIdPagina, pstrToken, pstrQuantidade));

                wqtRequest.Accept = "application/json";
                wqtRequest.Method = "GET";
                hwrResponse = (System.Net.HttpWebResponse)wqtRequest.GetResponse();

                using (var sr = new StreamReader(hwrResponse.GetResponseStream()))
                {
                    strRetonoJson += sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            finally
            {
                hwrResponse.Close();
            }

            return strRetonoJson;
        }

        #endregion

        #region Carregar facebook
        /// <summary>
        /// Metodo utilizado para carregar
        /// a linha do tempo do facebook
        /// lendo um json e passando para um 
        /// objeto com os dados que precisamos
        /// </summary>
        /// <param name="pstrJson">Json com a linha do tempo</param>
        /// <param name="pstrIdPagina">Id da pagina da linha do tempo</param>
        /// <user>vmoura</user>
        private static List<MLFacePost> CarregarFacebook(string pstrJson, string pstrIdPagina)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<MLFacePost> lstMLFacePost = new List<MLFacePost>();

            try
            {
                MLFace objMLFace = serializer.Deserialize<MLFace>(pstrJson);

                if (objMLFace != null)
                    for (int i = 0; i < objMLFace.data.Length; i++)
                        if (objMLFace.data[i].from.id.Equals(pstrIdPagina))
                            lstMLFacePost.Add(objMLFace.data[i]);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return lstMLFacePost;
        }

        #endregion

        #endregion

        #region Twitter

        #region Carregar twitter
        /// <summary>
        /// Metodo utilizado para carregar
        /// a linha do tempo do twitter
        /// lendo um json e passando para um 
        /// objeto com os dados que precisamos
        /// </summary>
        /// <param name="pstrJson">Json com a linha do tempo</param>
        /// <param name="pstrIdPagina">Id da pagina da linha do tempo</param>
        /// <user>vmoura</user>
        private static List<MLTwitter> CarregarTwitter(string pstrJson)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<MLTwitter> lstMLTwitter = new List<MLTwitter>();

            try
            {
                lstMLTwitter = serializer.Deserialize<List<MLTwitter>>(pstrJson);
            }
            catch
            {

            }

            return lstMLTwitter;
        }

        #endregion

        #region CarregarJsonTwitter
        /// <summary>
        /// Metodo utilizado para criar o script
        /// aonde carrega a linha do tempo do twitter
        /// do usuario cadastro na propriedade do modulo.
        /// </summary>
        /// <param name="pstrNomeUsuario">Nome do usuario</param>
        /// <returns>json com a consulta</returns>
        /// <user>vmoura</user>
        private static string CarregarJsonTwitter(string nome, string consumerKey, string token, string consumerScret, string tokenSecret, int count)
        {
            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";

            // unique request details
            var oauth_nonce = Convert.ToBase64String(
                new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow
                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details            
            var resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json";

            // create oauth signature
            var baseFormat = "count={7}&oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            var baseString = string.Format(baseFormat, consumerKey, oauth_nonce, oauth_signature_method, oauth_timestamp, token, oauth_version, Uri.EscapeDataString(nome), Uri.EscapeDataString(count.ToString()));

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(consumerScret),
                                    "&", Uri.EscapeDataString(tokenSecret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(consumerKey),
                                    Uri.EscapeDataString(token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );


            // make the request

            ServicePointManager.Expect100Continue = false;

            var postBody = "screen_name=" + Uri.EscapeDataString(nome);//
            var postbody2 = "count=" + Uri.EscapeDataString(count.ToString());
            resource_url += "?" + postBody + "&" + postbody2;

            WebResponse response = null;
            string strRertono = String.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
                request.Headers.Add("Authorization", authHeader);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                response = request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    strRertono += sr.ReadToEnd();
                }
            }
            catch
            {

            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return strRertono;
        }

        #endregion

        #endregion

        #region Linekedin

        #region Carregar Linekedin
        /// <summary>
        /// Metodo utilizado para carregar
        /// os updates do linkedin
        /// lendo um json e passando para um 
        /// objeto com os dados que precisamos
        /// </summary>
        /// <param name="pstrJson">Json com a linha do tempo</param>
        /// <user>rvissontai</user>
        private static MLLinkedIn CarregarLinekedin(string pstrJson)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            MLLinkedIn linkedIn = new MLLinkedIn();

            if (string.IsNullOrWhiteSpace(pstrJson))
            {
                return linkedIn;
            }

            try
            {
                linkedIn = serializer.Deserialize<MLLinkedIn>(pstrJson);

                foreach (var item in linkedIn.values)
                {
                    double timestamp = Convert.ToDouble(item.timestamp);
                    System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(timestamp / 1000).ToLocalTime();

                    item.timestamp = dateTime;
                }
            }
            catch { }

            return linkedIn;
        }

        #endregion

        #region CarregarJsonLinkedIn
        /// <summary>
        /// Metodo utilizado para carregar as novas
        /// publicações na pagina do facebook, atraves do token, 
        /// id da pagina que servirá de font de dados.
        /// </summary>       
        /// <param name="pstrIdPagina">Id da pagina que será feito a leitura da linha do tempo</param>
        /// <param name="pstrToken">Token de acesso</param>
        /// <returns>Json com a linha do tempo</returns>
        /// <user>vmoura</user>
        private static string CarregarJsonLinkedIn(string company, string token, string quantidade)
        {
            try
            {
                HttpWebRequest wqtRequest = (HttpWebRequest)WebRequest.Create(String.Format(URL_LINKEDIN_FEED, company, quantidade, token));

                wqtRequest.Accept = "application/json";
                wqtRequest.Method = "GET";

                using (var hwrResponse = wqtRequest.GetResponse())
                using (var sr = new StreamReader(hwrResponse.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return string.Empty;
        }

        #endregion

        #endregion

        #region ConverterFeeds

        public static List<MLFeeds> ConverterFeeds(List<MLFacePost> lstFace, List<MLTwitter> lstTwitter, MLLinkedIn objLinkedIn)
        {
            var lstRetono = new List<MLFeeds>();

            #region Facebook
            //item.text.ParseURL().ParseUsername().ParseHashtag(), 

            try
            {
                if (lstFace == null)
                {
                    lstFace = new List<MLFacePost>();
                }

                foreach (var item in lstFace)
                    lstRetono.Add(
                        new MLFeeds()
                        {
                            Data = DateTime.Parse(item.created_time).ToLocalTime(),
                            Mensagem = item.message,
                            Link = item.link,
                            NomeRedeSocial = "Facebook",
                            Class = "icon-facebook"
                        }
                    );
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            #endregion

            #region Twitter
            try
            {
                if (lstTwitter == null)
                {
                    lstTwitter = new List<MLTwitter>();
                }

                foreach (var item in lstTwitter)
                {
                    if (item.user == null)
                    {
                        item.user = new MLTwitterPost();
                    }

                    lstRetono.Add(
                        new MLFeeds()
                        {
                            Data = DateTime.ParseExact(item.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture).ToLocalTime(),
                            Mensagem = item.text,
                            Link = String.Format("https://twitter.com/{0}/status/{1}", item.user.screen_name, item.id_str), //item.source, 
                            NomeRedeSocial = "Twitter",
                            Class = "icon-twitter"
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            #endregion

            #region LinkedIn
            try
            {
                if (objLinkedIn == null)
                {
                    objLinkedIn = new MLLinkedIn();
                }

                foreach (var item in objLinkedIn.values)
                {
                    string strMensagem = item.updateContent.companyStatusUpdate.share.comment;

                    if (String.IsNullOrWhiteSpace(strMensagem))
                        strMensagem = item.updateContent.companyStatusUpdate.share.content.description;

                    if (String.IsNullOrWhiteSpace(strMensagem))
                        strMensagem = item.updateContent.companyStatusUpdate.share.content.title;

                    lstRetono.Add(
                        new MLFeeds()
                        {
                            NomeRedeSocial = "LinkeIn",
                            Class = "icon-linkedin",
                            Data = (DateTime)item.timestamp,
                            Mensagem = strMensagem,
                            Link = item.updateUrl
                        }
                   );
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            #endregion

            return lstRetono.OrderByDescending(o => o.Data).ToList();
        }

        #endregion

        #endregion
    }
}
