namespace JumpStartCS.Orleans.Grains
{
    public interface ICustomerGrain : IGrainWithStringKey
    {
        Task<int> GetCustomerCheckingAccountBalance();
    }
}
