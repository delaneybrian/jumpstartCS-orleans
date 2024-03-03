using JumpStartCS.Orleans.Grains.State;
using JumpStartCS.Orleans.Infrastructure;
using Orleans.Runtime;

namespace JumpStartCS.Orleans.Grains
{
    public class CustomerGrain : Grain, ICustomerGrain
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IPersistentState<CustomerDetailsState> _customerDetailsState;
        private readonly IPersistentState<CustomerAccountsState> _customerAccountsState;
        private readonly IAnalyticsService _analyticsService;

        public CustomerGrain(
            [PersistentState("customerDetails", "customerStore")] IPersistentState<CustomerDetailsState> customerDetailsState,
            [PersistentState("customerAccounts", "customerStore")] IPersistentState<CustomerAccountsState> customerAccountsState,
            IGrainFactory grainFactory,
            IAnalyticsService analyticsService)
        {
            _customerDetailsState = customerDetailsState;
            _customerAccountsState = customerAccountsState;

            _grainFactory = grainFactory;
            _analyticsService = analyticsService;
        }

        public async Task AddCustomerDetails(string name)
        {
            _customerDetailsState.State.Name = name;

            await _customerAccountsState.WriteStateAsync();

            await _analyticsService.UploadAnalytics();
        }

        public async Task DebitAccount(Guid accountId, int debitAmount)
        {
            if (!_customerAccountsState.State.CheckingAccountIds.Any(x => x == accountId))
            {
                _customerAccountsState.State.CheckingAccountIds.Add(accountId);

                await _customerAccountsState.WriteStateAsync();
            }

            var accountGrain = _grainFactory.GetGrain<ICheckingAccountGrain>(accountId);

            await accountGrain.DebitBalance(debitAmount);
        }

        public async Task<int> GetCustomerCheckingAccountBalance(Guid checkingAccountId)
        {
            var checkingAccountGrain = _grainFactory.GetGrain<ICheckingAccountGrain>(checkingAccountId);

            return await checkingAccountGrain.GetBalance();
        }

        public async Task<string> GetCustomerDetails()
        {
            return _customerDetailsState.State.Name;
        }
    }
}
