using Orleans;

namespace JumpStartCS.Orleans.Grains
{
    public interface ICheckingAccountGrain : IGrainWithGuidKey
    {
        Task LogBalance();
    }
}
