using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.Events;
using JumpStartCS.Orleans.Grains.State;
using Orleans.Runtime;
using Orleans.Streams;

namespace JumpStartCS.Orleans.Grains.Grains
{
    public class CustomerGrain : Grain, ICustomerGrain, IAsyncObserver<BalanceChangeEvent>
    {
        private readonly IPersistentState<CustomerState> _customerState;

        public CustomerGrain(
            [PersistentState("customerState")] IPersistentState<CustomerState> customerState)
        {
            _customerState = customerState;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider("StreamProvider");

            foreach (var id in _customerState.State.BalanceByCheckingAccountId.Keys)
            {
                var streamId = StreamId.Create("BalanceChange", id);

                var stream = streamProvider.GetStream<BalanceChangeEvent>(streamId);

                var handles = await stream.GetAllSubscriptionHandles();

                foreach(var handle in handles)
                {
                    await handle.ResumeAsync(this);
                }
            }

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task AddCheckingAccount(Guid checkingAccountId)
        {
            _customerState.State.BalanceByCheckingAccountId.Add(checkingAccountId, 0);

            var streamProvider = this.GetStreamProvider("StreamProvider");

            var streamId = StreamId.Create("BalanceChange", checkingAccountId);

            var stream = streamProvider.GetStream<BalanceChangeEvent>(streamId);

            await stream.SubscribeAsync(this);

            await _customerState.WriteStateAsync();
        }

        public async Task<decimal> GetNetWorth()
        {
            return _customerState.State.BalanceByCheckingAccountId.Values.Sum();
        }

        public Task OnCompletedAsync()
        {
            //todo add some logging

            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            //todo add some logging

            return Task.CompletedTask;
        }

        public async Task OnNextAsync(BalanceChangeEvent balanceChangeEvent, StreamSequenceToken? token = null)
        {
            var balanceByCheckingAccountId = _customerState.State.BalanceByCheckingAccountId;

            if (balanceByCheckingAccountId.ContainsKey(balanceChangeEvent.AccountId))
            {
                balanceByCheckingAccountId[balanceChangeEvent.AccountId] = balanceChangeEvent.Balance;
            }
            else
            {
                balanceByCheckingAccountId.Add(balanceChangeEvent.AccountId, balanceChangeEvent.Balance);
            }

            await _customerState.WriteStateAsync();
        }
    }
}
