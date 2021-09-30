using CMSv4.Model;
using Framework.Utilities;
using Newtonsoft.Json;
using System;
using VM2.PageSpeed;

namespace CMSv4.BusinessLayer
{
    public class BLCmsPageSpeed
    {
        private decimal CodigoPagina;
        private MLPaginaPageSpeed ModelPageSpeed;
        private MLPagina ModelPagina;
        private string UrlCompletaPagina;
        private MLPageSpeedResponseCompletoV5 Response;

        public BLCmsPageSpeed(decimal codigoPagina)
        {
            CodigoPagina = codigoPagina;
            ModelPageSpeed = new MLPaginaPageSpeed();
            ModelPagina = new MLPagina();
        }

        public MLPageSpeedResponseCompletoV5 RunAnalysis()
        {
            ObterModelPageSpeed();

            ObterModelPagina();

            MontarUrlParaAnalise();

            ExecutarAnalise();

            SalvarResultado();

            return Response;
        }

        private void ObterModelPageSpeed()
        {
            ModelPageSpeed = CRUD.Obter(new MLPaginaPageSpeed() { CodigoPagina = CodigoPagina }, PortalAtual.ConnectionString);

            if(ModelPageSpeed == null)
            {
                ModelPageSpeed = new MLPaginaPageSpeed()
                {
                    CodigoPagina = CodigoPagina
                };
            }
        }

        private void ObterModelPagina()
        {
            ModelPagina = CRUD.Obter<MLPagina>(CodigoPagina);
        }

        private void MontarUrlParaAnalise()
        {
            var schema = "http://";

            if (ModelPagina.Https.GetValueOrDefault(false))
                schema = "https://";

            UrlCompletaPagina = $"{schema}{HttpContextFactory.Current.Request.Url.Authority}/{PortalAtual.Obter.Diretorio}/{ModelPagina.Url}";
        }

        private void ExecutarAnalise()
        {
            var pageSpeedBuilder = new BLPageSpeedBuilder(PortalAtual.Obter.PageSpeedApiKey);

            Response = pageSpeedBuilder
                .UrlToAnalyze(UrlCompletaPagina)
                .Strategy(EnumStrategy.Desktop)
                .Locale("en")
                .AddAllCategories()
                .RunAnalysis();
        }

        private void SalvarResultado()
        {
            if(Response.Error == null || Response.Error.error == null)
            {
                ModelPageSpeed.DataUltimaAvaliacao = DateTime.Now;
                ModelPageSpeed.JsonResult = JsonConvert.SerializeObject(Response.AnalysisResult);

                CRUD.Salvar(ModelPageSpeed);
            }
        }
    }
}
