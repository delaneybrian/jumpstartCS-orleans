namespace JumpStartCS.Orleans.Infrastructure
{
    public interface IComplianceService
    {
        Task UpdateComplianceScore(int newScore);

        Task<int> ReadComplianceScore();
    }
}
