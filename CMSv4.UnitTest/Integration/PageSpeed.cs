using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using VM2.PageSpeed;

namespace CMSv4.UnitTest.Integration
{
    [TestClass]
    public class PageSpeed
    {
        [TestMethod]
        public void AccessibilitySucesso()
        {
            var response = ObterResult("pt", EnumCategory.Accessibility);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Accessibility);
        }

        [TestMethod]
        public void BestPracticesSucesso()
        {
            var response = ObterResult("pt", EnumCategory.BestPractices);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.BestPractices);
        }

        [TestMethod]
        public void PerformanceSucesso()
        {
            var response = ObterResult("pt", EnumCategory.Performance);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Performance);
        }

        [TestMethod]
        public void PwaSucesso()
        {
            var response = ObterResult("pt", EnumCategory.Pwa);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Pwa);
        }

        [TestMethod]
        public void SeoSucesso()
        {
            var response = ObterResult("pt", EnumCategory.Seo);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Seo);
        }

        [TestMethod]
        public void TodasCategoriasSucesso()
        {
            var response = ObterResult("pt", new List<EnumCategory>() {
                EnumCategory.Accessibility,
                EnumCategory.BestPractices,
                EnumCategory.Performance,
                EnumCategory.Pwa,
                EnumCategory.Seo
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Accessibility);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.BestPractices);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Performance);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Pwa);
            Assert.IsNotNull(response.AnalysisResult.LighthouseResult.Categories.Seo);
        }

        [TestMethod]
        //[ExpectedException(typeof(Web),
        public void ExceptionErro500()
        {
            var pageSpeedBuilder = new BLPageSpeedBuilder("AIzaSyAsWGgdxKUXkD_z-SGrEJbQE5aiEUGUjWo");

            pageSpeedBuilder
                .UrlToAnalyze("http://localhost:5000/UrlInvalida")
                .Locale("pt")
                .Strategy(EnumStrategy.Desktop)
                .AddCategory(EnumCategory.Accessibility);

            var result = pageSpeedBuilder.RunAnalysis();
        }

        private MLPageSpeedResponseCompletoV5 ObterResult(string locale, EnumCategory category)
        {
            return ObterResult(locale, new List<EnumCategory>() { category });
        }

        private MLPageSpeedResponseCompletoV5 ObterResult(string locale, List<EnumCategory> category)
        {
            var pageSpeedBuilder = new BLPageSpeedBuilder("AIzaSyAsWGgdxKUXkD_z-SGrEJbQE5aiEUGUjWo");

            pageSpeedBuilder
                .UrlToAnalyze("http://democms.homolog.co")
                .Locale(locale)
                .Strategy(EnumStrategy.Desktop)
                .AddRangeCategory(category);

            return pageSpeedBuilder.RunAnalysis();
        }
    }
}
