namespace JumpStartCS.Orleans.Grains
{
    public interface ICheckingAccountGrain : IGrainWithGuidKey
    {
        Task<int> GetBalance();
    }
}
