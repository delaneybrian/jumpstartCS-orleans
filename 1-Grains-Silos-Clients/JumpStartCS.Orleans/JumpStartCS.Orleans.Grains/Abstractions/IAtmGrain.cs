namespace JumpStartCS.Orleans.Grains.Abstractions
{
    public interface IAtmGrain : IGrainWithGuidKey
    {
        [Transaction(TransactionOption.Create)]
        Task Initialise(decimal openingBalance);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Withdraw(decimal amount);

        [Transaction(TransactionOption.Create)]
        Task<decimal> Balance();
    }
}
