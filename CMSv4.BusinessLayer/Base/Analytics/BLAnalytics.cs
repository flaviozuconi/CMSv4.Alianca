using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using VM2.Google.Analytics.Model;
using VM2.Google.BusinessLayer;

namespace CMSv4.BusinessLayer
{
    public class BLAnalytics
    {
        public static bool HasRequiredInfoToGetData()
        {
            var portal = PortalAtual.Obter;
            var jsonFile = HostingEnvironment.MapPath($"/Portal/{portal.Diretorio}/analytics/demo.json");

            if (!File.Exists(jsonFile))
                return false;

            if (string.IsNullOrWhiteSpace(portal.AnalyticsProfileId))
                return false;

            return true;
        }

        public static List<MLAnalytics> GetInfo(DateTime? dataInicial, DateTime? dataFinal)
        {
            var portal = PortalAtual.Obter;

            var analyticsProcessor = new Analytics($"/Portal/{portal.Diretorio}/analytics/demo.json", portal.AnalyticsProfileId);
            var cacheKey = analyticsProcessor.CacheKeyAnalytics + "_" + dataInicial.Value.ToShortDateString() + "_" + dataFinal.Value.ToShortDateString();
            var cachedAnalytics = BLCache.Get<List<MLAnalytics>>(cacheKey);

            if (cachedAnalytics == null)
            {
                var result = analyticsProcessor.Execute(dataInicial, dataFinal);

                BLCache.Add(cacheKey, result, 1);

                cachedAnalytics = result;
            }

            //analyticsProcessor.ExecuteRealTime();

            return cachedAnalytics;
        }

        public static List<MLAnalytics> GetAmountOfUserOnlineInRealTime()
        {
            var analyticsProcessor = new Analytics("/Portal/Principal/analytics/demo.json", "ga:181127913");

            //analyticsProcessor.ExecuteRealTime();

            return BLCache.Get<List<MLAnalytics>>(analyticsProcessor.CacheKeyAnalytics);
        }

        private static List<MLAnalytics> GenerateFakeData()
        {
            var fakeModel = new List<MLAnalytics>();

            fakeModel.Add(new MLAnalytics());

            fakeModel[0].lstUser = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        value = 145574,
                        valueb = 6549871
                    },
                    new MLAnalyticsGrafico()
                    {
                        value = 321654,
                        valueb = 9876541
                    }
                };

            fakeModel[0].realTime = 20;

            fakeModel[0].lstCity = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        label = "Brasilia",
                        value = 154789
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "São Paulo",
                        value = 139547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Campinas",
                        value = 124784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Rio de Janeiro",
                        value = 105578
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Curitiba",
                        value = 98754
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Sorocaba",
                        value = 58784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Itu",
                        value = 47458
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Vinhedo",
                        value = 39547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Votorantim",
                        value = 25478
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Pilar do Sul",
                        value = 15478
                    }
                };

            fakeModel[0].lstBrowser = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        label = "Chrome",
                        value = 154789
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Samsung Internet",
                        value = 139547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Safari",
                        value = 124784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Android Webview",
                        value = 105578
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Curitiba",
                        value = 98754
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Firefox",
                        value = 58784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Edge",
                        value = 47458
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Internet Explorer",
                        value = 39547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Opera",
                        value = 25478
                    }
                };

            fakeModel[0].lstSistOperacional = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        label = "Android",
                        value = 154789
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Windows",
                        value = 139547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "iOS",
                        value = 124784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Linux",
                        value = 105578
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Others",
                        value = 98754
                    }
                };

            fakeModel[0].lstPageVisit = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        label = "/home",
                        value = 154789
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "/noticia/integra",
                        value = 139547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "/esqueci-minha-senha",
                        value = 124784
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "/cadastro",
                        value = 105578
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "/contato",
                        value = 98754
                    }
                };

            fakeModel[0].lstDispositivo = new List<MLAnalyticsGrafico>()
                {
                    new MLAnalyticsGrafico()
                    {
                        label = "Desktop",
                        value = 154789
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Mobile",
                        value = 139547
                    },
                    new MLAnalyticsGrafico()
                    {
                        label = "Tablet",
                        value = 124784
                    }
            };

            return fakeModel;
        }
    }
}
