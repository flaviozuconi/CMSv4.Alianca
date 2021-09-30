using System.Collections.Generic;

namespace VM2.PageSpeed
{
    public interface IPageSpeedRequest<TResponse>
    {
        IPageSpeedInitializer Initializer { get; }

        string UrlToAnalyze { get; }

        string Locale { get; }

        EnumStrategy Strategy { get; }

        List<EnumCategory> Categories { get; }

        void Deserialize();

        TResponse Execute();
    }
}
