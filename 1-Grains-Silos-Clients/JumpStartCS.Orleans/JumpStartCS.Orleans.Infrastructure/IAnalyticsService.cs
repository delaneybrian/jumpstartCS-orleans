namespace JumpStartCS.Orleans.Infrastructure
{
    public interface IAnalyticsService
    {
        Task UploadAnalytics();

        Task<int> ReadAnalytics();
    }
}
