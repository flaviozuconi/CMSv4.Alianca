using System.Collections.Generic;

namespace VM2.PageSpeed
{
    public class VM2PageSpeedRequestBuilder
    {
        private IPageSpeedInitializer InitializerValue;
        private string UrlToAnalyzeValue;
        private string LocaleValue;
        private EnumStrategy StrategyValue;
        private List<EnumCategory> CategoriesValue;

        public VM2PageSpeedRequestBuilder()
        {
            CategoriesValue = new List<EnumCategory>();
        }

        public VM2PageSpeedRequestBuilder Initializer(IPageSpeedInitializer initializerValue)
        {
            InitializerValue = initializerValue;
            return this;
        }

        public VM2PageSpeedRequestBuilder UrlToAnalyze(string urlToAnalyzeValue)
        {
            UrlToAnalyzeValue = urlToAnalyzeValue;
            return this;
        }

        public VM2PageSpeedRequestBuilder Locale(string localeValue)
        {
            LocaleValue = localeValue;
            return this;
        }

        public VM2PageSpeedRequestBuilder Strategy(EnumStrategy strategyValue)
        {
            StrategyValue = strategyValue;
            return this;
        }

        public VM2PageSpeedRequestBuilder addCategory(EnumCategory category)
        {
            CategoriesValue.Add(category);
            return this;
        }

        public MLPageSpeedResponseCompletoV5 Execute()
        {
            return new VM2PageSpeedRequest(UrlToAnalyzeValue, LocaleValue, StrategyValue, CategoriesValue).Execute();
        }
    }
}
