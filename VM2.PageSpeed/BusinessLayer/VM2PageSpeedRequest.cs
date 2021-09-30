using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace VM2.PageSpeed
{
    public class VM2PageSpeedRequest : IPageSpeedRequest<MLPageSpeedResponseCompletoV5>
    {
        public IPageSpeedInitializer Initializer { get; set; }

        public string UrlToAnalyze { get; set; }
        public string Locale { get; set; }
        public EnumStrategy Strategy { get; set; }
        public List<EnumCategory> Categories { get; set; }

        private UriBuilder UriPageSpeedBuilder { get; set; }
        private string JsonResponse { get; set; }
        private MLPageSpeedResponseCompletoV5 ModelResponse { get; set; }

        public VM2PageSpeedRequest(string urlToAnalyze)
        {
            UrlToAnalyze = urlToAnalyze;
            Locale = "pt";
            Strategy = EnumStrategy.Desktop;
            ModelResponse = new MLPageSpeedResponseCompletoV5();
        }

        public VM2PageSpeedRequest(string urlToAnalyze, string locale, EnumStrategy strategy, List<EnumCategory> categories)
        {
            UrlToAnalyze = urlToAnalyze;
            Locale = locale;
            Strategy = strategy;
            Categories = categories;
            ModelResponse = new MLPageSpeedResponseCompletoV5();
        }

        public MLPageSpeedResponseCompletoV5 Execute()
        {
            BindUrlPageSpeed();

            GetJsonResponse();

            Deserialize();

            return ModelResponse;
        }

        private void BindUrlPageSpeed()
        {
            UriPageSpeedBuilder = new UriBuilder(Initializer.BasePageSpeedUrl);
            var query = HttpUtility.ParseQueryString(UriPageSpeedBuilder.Query);

            query["key"] = Initializer.ApiKey;
            query["url"] = UrlToAnalyze;
            query["strategy"] = Strategy.ToString();

            UriPageSpeedBuilder.Query = query.ToString();

            foreach (var category in Categories)
                UriPageSpeedBuilder.Query += $"&category={ category.ToQueryStringParameter() }";
        }

        private void GetJsonResponse()
        {
            try
            {
                using (var webClient = new WebClient())
                    JsonResponse = webClient.DownloadString(UriPageSpeedBuilder.ToString());
            }
            catch (WebException exception)
            {
                string jsonResponseError;

                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                    jsonResponseError = reader.ReadToEnd();

                ModelResponse.Error = JsonConvert.DeserializeObject<PageSpeedError>(jsonResponseError);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Deserialize()
        {
            if(!string.IsNullOrWhiteSpace(JsonResponse))
                ModelResponse.AnalysisResult = JsonConvert.DeserializeObject<PageSpeedApiResponseV5>(JsonResponse);
        }
    }
}
