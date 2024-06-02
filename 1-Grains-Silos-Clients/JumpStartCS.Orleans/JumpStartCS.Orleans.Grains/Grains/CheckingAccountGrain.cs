using JumpStartCS.Orleans.Grains.Events;
using JumpStartCS.Orleans.Grains.State;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;

namespace JumpStartCS.Orleans.Grains
{
    [Reentrant]
    public class CheckingAccountGrain : Grain, ICheckingAccountGrain, IRemindable
    {
        private const string ReccuringPaymentReminderPrepender = "ReccuringPayment";

        private readonly IPersistentState<CheckingAccountState> _checkingAccountState;
        private readonly ITransactionalState<BalanceState> _transactionalBalanceState;
        private readonly ITransactionClient _transactionClient;

        public CheckingAccountGrain(
            [PersistentState("checkingAccount")] IPersistentState<CheckingAccountState> checkingAccountState,
            [TransactionalState("balance")] ITransactionalState<BalanceState> transactionalBalanceState,
            ITransactionClient transactionClient)
        {
            _checkingAccountState = checkingAccountState;
            _transactionalBalanceState = transactionalBalanceState;
            _transactionClient = transactionClient;
        }

        public async Task Initialise(decimal openingBalance, Guid customerId)
        {
            _checkingAccountState.State = new CheckingAccountState
            {
                AccountId = this.GetPrimaryKey(),
                AccountType = "STANDARD",
                CustomerId = customerId,
                OpenedAtUtc = DateTime.UtcNow
            };

            await _transactionalBalanceState.PerformUpdate(state =>
            {
                state.CurrentBalance = openingBalance;
            });

            await _checkingAccountState.WriteStateAsync();
        }

        public async Task ScheduleRecurringPayment(Guid paymentId, decimal paymentAmount, int reccursEveryMinutes)
        {
            if (paymentAmount < 0)
                throw new ArgumentException("recurring payments must be greater than 0");

            _checkingAccountState.State.RecurringPayments.Add(new RecurringPayment
                {
                    Id = paymentId,
                    PaymentAmount = paymentAmount,
                });

            await _checkingAccountState.WriteStateAsync();

            await this.RegisterOrUpdateReminder(
                $"{ReccuringPaymentReminderPrepender}:::{paymentId}", 
                TimeSpan.FromMinutes(reccursEveryMinutes), 
                TimeSpan.FromMinutes(reccursEveryMinutes));
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName.StartsWith(ReccuringPaymentReminderPrepender))
            {
                var recurringPaymentId = Guid.Parse(reminderName.Split(":::").Last());

                var recurringPayment =  _checkingAccountState.State.RecurringPayments
                        .Single(x => x.Id == recurringPaymentId);

                await _transactionClient.RunTransaction(TransactionOption.Create, async () =>
                {
                     await Debit(recurringPayment.PaymentAmount);
                });
            }
        }

        public async Task<decimal> GetBalance()
        {
            return await _transactionalBalanceState.PerformRead(state => state.CurrentBalance);
        }

        public async Task Credit(decimal creditAmount)
        {
            await _transactionalBalanceState.PerformUpdate(state =>
            {
                var currentBalance = state.CurrentBalance;

                if (creditAmount < 0)
                    throw new ArgumentException("credits must be greater than 0");

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionAmount = creditAmount,
                    TransactionDateTimeUtc = DateTime.UtcNow,
                };

               state.Transactions.Add(transaction);

               state.CurrentBalance = currentBalance + creditAmount;
            });

            var balance = await GetBalance();

            var streamProvider = this.GetStreamProvider("StreamProvider");

            var streamId = StreamId.Create("BalanceChange", this.GetGrainId().GetGuidKey());

            var stream = streamProvider.GetStream<BalanceChangeEvent>(streamId);

            await stream.OnNextAsync(new BalanceChangeEvent
            {
                AccountId = this.GetGrainId().GetGuidKey(),
                Balance = balance
            });
        }

        public async Task Debit(decimal debitAmount)
        {
            await _transactionalBalanceState.PerformUpdate(state =>
            {
                var currentBalance = state.CurrentBalance;

                if (debitAmount < 0)
                    throw new ArgumentException("debits must be greater than 0");

                if (state.CurrentBalance < debitAmount)
                    throw new ArgumentException("debit amount exceeds balance");

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionAmount = debitAmount,
                    TransactionDateTimeUtc = DateTime.UtcNow,
                };

                state.Transactions.Add(transaction);

                state.CurrentBalance = currentBalance - debitAmount;
            });
        }

        public async Task CancellableWork(GrainCancellationToken grainCancellationToken, long workDurationSeconds)
        {
            try
            {
                while (!grainCancellationToken.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(workDurationSeconds), grainCancellationToken.CancellationToken);
                }
            }
            catch (TaskCanceledException _)
            {
                return;
            }       
        }

        public async Task FireAndForgetWork()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            throw new NotSupportedException("This is actually not supported");
        }
    }
}
