using JumpStartCS.Orleans.Grains.State;
using Orleans.Runtime;

namespace JumpStartCS.Orleans.Grains
{
    public class CheckingAccountGrain : Grain, ICheckingAccountGrain
    {
        private readonly IPersistentState<CheckingAccountState> _checkingAccountState;

        public CheckingAccountGrain([PersistentState("checkingAccount", "accountStore")] IPersistentState<CheckingAccountState> checkingAccountState)
        {
            _checkingAccountState = checkingAccountState;
        }

        public async Task<int> GetBalance()
        {
            return _checkingAccountState.State.Balance;
        }

        public async Task DebitBalance(int debitAmount)
        {
            _checkingAccountState.State.Balance = _checkingAccountState.State.Balance + debitAmount;

            await _checkingAccountState.WriteStateAsync();
        }
    }
}
