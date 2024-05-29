namespace JumpStartCS.Orleans.Grains.Abstractions
{
    public interface ICustomerGrain : IGrainWithGuidKey
    {
        public Task AddCheckingAccount(Guid checkingAccountId);

        public Task<decimal> GetNetWorth();
    }
}
