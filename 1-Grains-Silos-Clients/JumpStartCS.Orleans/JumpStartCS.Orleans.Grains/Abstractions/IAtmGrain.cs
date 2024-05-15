namespace JumpStartCS.Orleans.Grains.Abstractions
{
    public interface IAtmGrain : IGrainWithGuidKey
    {
        Task Initialise(decimal openingBalance);

        Task Withdraw(Guid checkingAccountId, decimal amount);
    }
}
