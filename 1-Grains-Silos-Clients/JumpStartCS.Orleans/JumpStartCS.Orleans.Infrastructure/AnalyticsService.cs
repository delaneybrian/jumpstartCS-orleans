namespace JumpStartCS.Orleans.Infrastructure
{
    public class AnalyticsService : IAnalyticsService
    {
        public async Task<int> ReadAnalytics()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            return 4;
        }

        public async Task UploadAnalytics()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }
    }
}
