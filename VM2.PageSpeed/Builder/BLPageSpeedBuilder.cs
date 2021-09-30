using System.Collections.Generic;

namespace VM2.PageSpeed
{
    public class BLPageSpeedBuilder
    {
        private string ApiKeyValue;
        private string UrlToAnalyzeValue;
        private bool GZipEnabledValue;
        private string LocaleValue;
        private EnumStrategy StrategyValue;
        private List<EnumCategory> CategoryValue;

        public BLPageSpeedBuilder(string apiKey)
        {
            ApiKeyValue = apiKey;
            CategoryValue = new List<EnumCategory>();
        }

        public BLPageSpeedBuilder UrlToAnalyze(string urlToAnalyzeValue)
        {
            UrlToAnalyzeValue = urlToAnalyzeValue;
            return this;
        }

        public BLPageSpeedBuilder GZipEnabled(bool gZipEnabledValue)
        {
            GZipEnabledValue = gZipEnabledValue;
            return this;
        }

        public BLPageSpeedBuilder Locale(string localeValue)
        {
            LocaleValue = localeValue;
            return this;
        }

        public BLPageSpeedBuilder Strategy(EnumStrategy strategyValue)
        {
            StrategyValue = strategyValue;
            return this;
        }

        public BLPageSpeedBuilder AddCategory(EnumCategory categoryValue)
        {
            CategoryValue.Add(categoryValue);
            return this;
        }

        public BLPageSpeedBuilder AddRangeCategory(List<EnumCategory> categoriesValue)
        {
            CategoryValue.AddRange(categoriesValue);
            return this;
        }

        public BLPageSpeedBuilder AddAllCategories()
        {
            CategoryValue.Clear();
            CategoryValue.Add(EnumCategory.Accessibility);
            CategoryValue.Add(EnumCategory.BestPractices);
            CategoryValue.Add(EnumCategory.Performance);
            CategoryValue.Add(EnumCategory.Pwa);
            CategoryValue.Add(EnumCategory.Seo);

            return this;
        }

        public MLPageSpeedResponseCompletoV5 RunAnalysis()
        {
            return new BLPageSpeed(ApiKeyValue, UrlToAnalyzeValue, LocaleValue, GZipEnabledValue, StrategyValue, CategoryValue).RunAnalysis();
        }
    }
}
