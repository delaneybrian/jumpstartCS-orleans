namespace JumpStartCS.Orleans.Grains
{
    public class CustomerGrain : Grain, ICustomerGrain
    {
        private readonly IGrainFactory _grainFactory;

        public CustomerGrain(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public async Task<int> GetCustomerCheckingAccountBalance()
        {
            var checkingAccountId = Guid.NewGuid();

            var checkingAccountGrain = _grainFactory.GetGrain<ICheckingAccountGrain>(checkingAccountId);

            return await checkingAccountGrain.GetBalance();
        }
    }
}
