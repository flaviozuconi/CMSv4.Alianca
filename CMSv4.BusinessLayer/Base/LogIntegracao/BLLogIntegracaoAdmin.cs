using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CMSv4.BusinessLayer
{
    public class BLLogIntegracaoAdmin
    {
        #region Listar Log Integracao

        /// <summary>
        /// LISTAR PUBLICO POR ANO
        /// </summary>
        public static List<MLAgendamentoIntermodalLog> Listar()
        {
            using (var command = Database.NewCommand("USP_MOD_AIL_L_AGENDAMENTO_INTERMODAL_LOG", string.Empty))
            {
                // Parametros
                command.NewCriteriaParameter("@DIAS", SqlDbType.Int, (Convert.ToInt32(CRUD.Obter(new MLConfiguracao { Chave = "ROTINA.AGENDAMENTO.INTERMODAL" })?.Valor)));

                // Execucao
                return Database.ExecuteReader<MLAgendamentoIntermodalLog>(command);
            }
        }

        #endregion

        #region Integrar
        /// <summary>
        /// Integrar 
        /// </summary>
        public static void Integrar(string schema, string authoriry)
        {
            try
            {
                foreach (var model in BLLogIntegracaoAdmin.Listar())
                {
                    var retorno = IntegrarAPI(model.Json, model.Imagem, schema, authoriry);

                    if (retorno.Contains("id")) CRUD.SalvarParcial(new MLAgendamentoIntermodalLog { Codigo = model.Codigo, isIntegrado = true, RetornoAPI = retorno });
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }
        #endregion

        #region Integrar API
        /// <summary>
        /// IntegrarAPI
        /// </summary>
        /// <param name="objModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string IntegrarAPI(string json, string caminho, string schema, string authoriry)
        {
            string retorno = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                var url = CRUD.Obter(new MLConfiguracao { Chave = "URL.Integracao.Movidesk.Ticket" })?.Valor ?? "https://api.movidesk.com/public/v1/tickets";

                #region Request para inserção de ticket
                var webRequest = (HttpWebRequest)WebRequest.Create(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&returnAllProperties=false");
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Method = "POST";

                var dados = Encoding.UTF8.GetBytes(json);

                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(dados, 0, dados.Length);
                    stream.Close();
                }

                using (var resposta = webRequest.GetResponse())
                {
                    var streamDados = resposta.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    string response = reader.ReadToEnd();

                    retorno = Newtonsoft.Json.Linq.JToken.Parse(response).ToString();
                }

                if (!string.IsNullOrEmpty(caminho) && !string.IsNullOrEmpty(retorno)) SendFile(retorno, caminho, schema, authoriry);

                #endregion
            }
            catch (Exception ex)
            {
                #region Envio de email
                var email = CRUD.Obter(new MLConfiguracao { Chave = "Email-Integracao-Movidesk" })?.Valor ?? "william.silva@vm2.com.br";

                // enviar email
                BLEmail.Enviar("Erro na integracação do movidesk", email, BLEmail.ObterModelo(BLEmail.ModelosPadrao.AlterarSenhaEnUs));
                #endregion

                ApplicationLog.ErrorLog(ex);
            }

            return retorno;
        }
        #endregion

        #region Send File
        /// <summary>
        /// Envio dos arquivos
        /// </summary>
        /// <param name="retorno"></param>
        /// <param name="lstCarga"></param>
        private static void SendFile(string retorno, string imagem, string schema, string authoriry)
        {
            var obj = JsonConvert.DeserializeObject<RetornoMovidesk>(retorno);

            if (obj?.id > 0)
            {
                foreach (var item in imagem.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string fileName = string.Format("{0}{1}", System.Web.Hosting.HostingEnvironment.MapPath("~"), item.Replace(schema + "://" + authoriry + "/", string.Empty));

                        using (HttpClient client = new HttpClient())
                        using (MultipartFormDataContent content = new MultipartFormDataContent())
                        using (FileStream fileStream = System.IO.File.OpenRead(fileName))
                        using (StreamContent fileContent = new StreamContent(fileStream))
                        {
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                FileName = item.Split('/')[item.Split('/').Length - 1]
                            };

                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                            fileContent.Headers.Add("name", item);
                            content.Add(fileContent);

                            var url = CRUD.Obter(new MLConfiguracao { Chave = "URL.Integracao.Movidesk.Arquivo" })?.Valor ?? "https://api.movidesk.com/public/v1/ticketFileUpload?";

                            var result = client.PostAsync(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&id=" + obj.id + "&actionId=1", content).Result;
                            result.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
