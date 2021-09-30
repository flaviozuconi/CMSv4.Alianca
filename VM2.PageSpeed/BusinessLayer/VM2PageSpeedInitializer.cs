namespace VM2.PageSpeed
{
    public class VM2PageSpeedInitializer : IPageSpeedInitializer
    {
        public string ApiKey { get; set; }
        public bool GZipEnabled { get; set; }
        public string BasePageSpeedUrl { get; set; }

        public VM2PageSpeedInitializer(string apiKey)
        {
            ApiKey = apiKey;
            GZipEnabled = true;
        }

        public VM2PageSpeedInitializer(string apiKey, bool gZipEnabled, string basePageSpeedUrl)
        {
            ApiKey = apiKey;
            GZipEnabled = gZipEnabled;
            BasePageSpeedUrl = basePageSpeedUrl;
        }
    }
}
