namespace JumpStartCS.Orleans.Infrastructure
{
    public class ComplianceService : IComplianceService
    {
        public async Task<int> ReadComplianceScore()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            return 4;
        }

        public async Task UpdateComplianceScore(int newScore)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }
    }
}
