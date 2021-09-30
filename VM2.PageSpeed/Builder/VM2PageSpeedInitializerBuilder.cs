using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM2.PageSpeed
{
    public class VM2PageSpeedInitializerBuilder
    {
        private string ApiKeyValue;
        private bool GZipEnabledValue;
        private string BasePageSpeedUrlValue;

        public VM2PageSpeedInitializerBuilder ApiKey(string apiKey)
        {
            ApiKeyValue = apiKey;
            return this;
        }

        public VM2PageSpeedInitializerBuilder GZipEnabled(bool gZipEnabledValue)
        {
            GZipEnabledValue = gZipEnabledValue;
            return this;
        }

        public VM2PageSpeedInitializerBuilder BasePageSpeedUrl(string basePageSpeedUrlValue)
        {
            BasePageSpeedUrlValue = basePageSpeedUrlValue;
            return this;
        }

        public IPageSpeedInitializer GetInstance()
        {
            return new VM2PageSpeedInitializer(ApiKeyValue, GZipEnabledValue, BasePageSpeedUrlValue);
        }
    }
}
