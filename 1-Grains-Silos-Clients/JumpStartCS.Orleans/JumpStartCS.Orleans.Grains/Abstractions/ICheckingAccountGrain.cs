using Orleans.Concurrency;

namespace JumpStartCS.Orleans.Grains
{
    public interface ICheckingAccountGrain : IGrainWithGuidKey
    {
        [Transaction(TransactionOption.Create)]
        Task Initialise(decimal openingBalance, Guid customerId);

        [Transaction(TransactionOption.Create)]
        Task<decimal> GetBalance();

        [Transaction(TransactionOption.Supported)]
        Task ScheduleRecurringPayment(Guid paymentId, decimal paymentAmount, int reccursEveryMinutes);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Credit(decimal creditAmount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Debit(decimal debitAmount);

        Task CancellableWork(GrainCancellationToken grainCancellationToken, long workDurationSeconds);

        [OneWay]
        Task FireAndForgetWork();
    }
}
