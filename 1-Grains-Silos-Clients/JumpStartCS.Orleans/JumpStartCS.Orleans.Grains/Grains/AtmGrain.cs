using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.State;
using Orleans.Runtime;

namespace JumpStartCS.Orleans.Grains
{
    public class AtmGrain : Grain, IAtmGrain
    {
        private readonly IPersistentState<AtmState> _atmState;

        public AtmGrain(
            [PersistentState("atm", "globallyDistributedStorage")] IPersistentState<AtmState> atmState)
        {
            _atmState = atmState;
        }

        public async Task Initialise(decimal openingBalance)
        {
            _atmState.State = new AtmState
            {
                Id = this.GetPrimaryKey(),
                Balance = openingBalance,
            };

            await _atmState.WriteStateAsync();
        }

        public async Task Withdraw(Guid checkingAccountId, decimal amount)
        {
            var currentAtmBalance = _atmState.State.Balance;

            if (currentAtmBalance < amount)
                throw new ArgumentException("withdrawl amount greater than ATM balance");

            _atmState.State = _atmState.State with { Balance = currentAtmBalance - amount };

            var checkingAccount = GrainFactory.GetGrain<ICheckingAccountGrain>(checkingAccountId);

            await checkingAccount.Debit(amount);

            await _atmState.WriteStateAsync();
        }

        public async Task<decimal> Balance()
        {
            return _atmState.State.Balance;
        }
    }
}
