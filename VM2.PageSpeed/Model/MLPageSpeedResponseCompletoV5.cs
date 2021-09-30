namespace VM2.PageSpeed
{
    public class MLPageSpeedResponseCompletoV5
    {
        public MLPageSpeedResponseCompletoV5()
        {
            AnalysisResult = new PageSpeedApiResponseV5();
            Error = new PageSpeedError();
        }

        public PageSpeedApiResponseV5 AnalysisResult { get;set; }

        public PageSpeedError Error { get; set; }
    }
}
