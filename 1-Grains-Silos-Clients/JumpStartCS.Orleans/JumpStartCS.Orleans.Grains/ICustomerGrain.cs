namespace JumpStartCS.Orleans.Grains
{
    public interface ICustomerGrain : IGrainWithStringKey
    {
        Task AddCustomerDetails(string name);

        Task DebitAccount(Guid accountId, int debitAmount);

        Task<int> GetCustomerCheckingAccountBalance(Guid checkingAccountId);

        Task<string> GetCustomerDetails();

        Task StartTimer(string timerName);

        Task StartReminder(string reminderName);
    }
}
