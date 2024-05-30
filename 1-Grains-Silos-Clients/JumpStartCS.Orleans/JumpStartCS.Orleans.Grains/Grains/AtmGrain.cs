using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.State;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

namespace JumpStartCS.Orleans.Grains
{
    [Reentrant]
    public class AtmGrain : Grain, IAtmGrain, IIncomingGrainCallFilter
    {
        private readonly ILogger<AtmGrain> _logger;
        private readonly ITransactionalState<AtmState> _atmState;

        public AtmGrain(
            ILogger<AtmGrain> logger,
            [TransactionalState("atm")] ITransactionalState<AtmState> atmState)
        {
            _logger = logger;
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

        public Task Invoke(IIncomingGrainCallContext context)
        {
            _logger.LogInformation($"Incoming Atm Grain Filer: Recived grain call on '{context.Grain}' to '{context.MethodName}' method");

            return context.Invoke();
        }
    }
}
