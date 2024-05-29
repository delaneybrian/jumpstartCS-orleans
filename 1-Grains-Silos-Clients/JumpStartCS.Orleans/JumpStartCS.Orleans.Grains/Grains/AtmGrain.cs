using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.State;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

namespace JumpStartCS.Orleans.Grains
{
    [Reentrant]
    public class AtmGrain : Grain, IAtmGrain
    {
        private readonly ITransactionalState<AtmState> _atmState;

        public AtmGrain(
            [TransactionalState("atm")] ITransactionalState<AtmState> atmState)
        {
            _atmState = atmState;
        }

        public async Task Initialise(decimal openingBalance)
        {
            await _atmState.PerformUpdate(state =>
            {
                state.Id = this.GetPrimaryKey();
                state.Balance = openingBalance;
            });
        }

        [Transaction(TransactionOption.Create)]
        public async Task Withdraw(decimal amount)
        {
            await _atmState.PerformUpdate(state =>
            {
                var currentAtmBalance = state.Balance;

                if (currentAtmBalance < amount)
                    throw new ArgumentException("withdrawl amount greater than ATM balance");

                state.Balance = currentAtmBalance - amount;
            });
        }

        public async Task<decimal> Balance()
        {
            return await _atmState.PerformRead(atmState => atmState.Balance);
        }
    }
}
