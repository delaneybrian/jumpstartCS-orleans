namespace JumpStartCS.Orleans.Grains
{
    public interface ICheckingAccountGrain : IGrainWithGuidKey
    {
        Task Initialise(decimal openingBalance, Guid customerId);

        Task<decimal> GetBalance();

        Task ScheduleRecurringPayment(Guid paymentId, decimal paymentAmount, int reccursEveryMinutes);

        Task Credit(decimal creditAmount);

        Task Debit(decimal debitAmount);
    }
}
