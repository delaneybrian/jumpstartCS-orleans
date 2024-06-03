using JumpStartCS.Orleans.Grains.Abstractions;
using JumpStartCS.Orleans.Grains.State;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace JumpStartCS.Orleans.Grains.Grains
{
    [StatelessWorker]
    public class StatelessTransferProcessingGrain : Grain, IStatlessTransferProcessingGrain
    {
        private readonly ITransactionClient _transactionClient;
        private readonly IPersistentState<TransferState> _transferState;
        private readonly ILogger<StatelessTransferProcessingGrain> _logger;

        public StatelessTransferProcessingGrain(
            ITransactionClient transactionClient,
            [PersistentState("transferState")] IPersistentState<TransferState> transferState,
            ILogger<StatelessTransferProcessingGrain> logger)
        {
            _transactionClient = transactionClient;
            _transferState = transferState;
            _logger = logger;
        }

        public async Task ProcessTransfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccountGrain = GrainFactory.GetGrain<ICheckingAccountGrain>(fromAccountId);

            var toAccountGrain = GrainFactory.GetGrain<ICheckingAccountGrain>(toAccountId);

            await _transactionClient.RunTransaction(TransactionOption.Create, async () =>
            {
                await fromAccountGrain.Debit(amount);

                await toAccountGrain.Credit(amount);
            });

            var updatedTransactionCount = _transferState.State.TransferCount += 1;

            _logger.LogInformation($"Transaction count on this processor {this.GetPrimaryKey()} now {updatedTransactionCount}");

            await _transferState.WriteStateAsync();
        }
    }
}
