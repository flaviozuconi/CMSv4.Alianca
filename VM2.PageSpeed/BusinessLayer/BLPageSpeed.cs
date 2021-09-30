using System.Collections.Generic;

namespace VM2.PageSpeed
{
    public class BLPageSpeed
    {
        private IPageSpeedInitializer Initializer { get; set; }
        private IPageSpeedRequest<MLPageSpeedResponseCompletoV5> PageSpeedRequest { get; set; }
        private MLPageSpeedResponseCompletoV5 Response;

        private string ApiKey;
        private string UrlToAnalyze;
        private string Locale;
        private bool GZipEnabled;
        private EnumStrategy Strategy;
        private List<EnumCategory> CategoriesEnum;

        public BLPageSpeed(string apiKey, string urlToAnalyze, string locale, bool gZipEnabled, EnumStrategy strategy, List<EnumCategory> categoriesEnum)
        {
            ApiKey = apiKey;
            UrlToAnalyze = urlToAnalyze;
            Locale = locale;
            GZipEnabled = gZipEnabled;
            Strategy = strategy;
            CategoriesEnum = categoriesEnum;
        }

        public MLPageSpeedResponseCompletoV5 RunAnalysis()
        {
            ConfigureInitializer();

            ConfigureRequest();

            ExecuteAnalysisAndGetResponse();

            return Response;
        }

        private void ConfigureInitializer()
        {
            Initializer = new VM2PageSpeedInitializer(ApiKey)
            {
                GZipEnabled = GZipEnabled,
                BasePageSpeedUrl = "https://www.googleapis.com/pagespeedonline/v5/runPagespeed"
            };
        }

        private void ConfigureRequest()
        {
            PageSpeedRequest = new VM2PageSpeedRequest(UrlToAnalyze)
            {
                Initializer = Initializer,
                Locale = Locale,
                Strategy = Strategy,
                Categories = CategoriesEnum
            };
        }

        private void ExecuteAnalysisAndGetResponse()
        {
            Response = PageSpeedRequest.Execute();
        }
    }
}
