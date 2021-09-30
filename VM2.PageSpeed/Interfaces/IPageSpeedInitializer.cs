namespace VM2.PageSpeed
{
    public interface IPageSpeedInitializer
    {
        string ApiKey { get; }

        bool GZipEnabled { get; }

        string BasePageSpeedUrl { get; }
    }
}
