using JumpStartCS.Orleans.Grains.State;
using Orleans.Runtime;

namespace JumpStartCS.Orleans.Grains
{
    public class CheckingAccountGrain : Grain, ICheckingAccountGrain, IRemindable
    {
        private const string ReccuringPaymentReminderPrepender = "ReccuringPayment";

        private readonly IPersistentState<CheckingAccountState> _checkingAccountState;
        private readonly IPersistentState<BalanceState> _balanceState;

        public CheckingAccountGrain(
            [PersistentState("checkingAccount", "locallyDistributedStorage")] IPersistentState<CheckingAccountState> checkingAccountState,
            [PersistentState("balance", "globallyDistributedStorage")] IPersistentState<BalanceState> balanceState)
        {
            _checkingAccountState = checkingAccountState;
            _balanceState = balanceState;
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

            _balanceState.State = new BalanceState
            {
                CurrentBalance = openingBalance,
            };

            await _checkingAccountState.WriteStateAsync();
            await _balanceState.WriteStateAsync();
        }

        public async Task ScheduleRecurringPayment(Guid paymentId, decimal paymentAmount, int reccursEveryMinutes)
        {
            if (paymentAmount < 0)
                throw new ArgumentException("recurring payments must be greater than 0");

            var recurringPayments = _balanceState.State.RecurringPayments;

            recurringPayments.Add(new RecurringPayment
            {
                Id = paymentId,
                PaymentAmount = paymentAmount,
            });

            await this.RegisterOrUpdateReminder(
                $"{ReccuringPaymentReminderPrepender}:::{paymentId}", 
                TimeSpan.FromMinutes(reccursEveryMinutes), 
                TimeSpan.FromMinutes(reccursEveryMinutes));

            await _balanceState.WriteStateAsync();
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName.StartsWith(ReccuringPaymentReminderPrepender))
            {
                var recurringPaymentId = Guid.Parse(reminderName.Split(":::").Last());

                var recurringPayment = _balanceState.State.RecurringPayments
                    .Single(x => x.Id == recurringPaymentId);

                await Debit(recurringPayment.PaymentAmount);
            }   
        }

        public async Task<decimal> GetBalance()
        {
            return _balanceState.State.CurrentBalance;
        }

        public async Task Credit(decimal creditAmount)
        {
            var currentBalance = _balanceState.State.CurrentBalance;

            if (creditAmount < 0)
                throw new ArgumentException("credits must be greater than 0");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionAmount = creditAmount,
                TransactionDateTimeUtc = DateTime.UtcNow,
            };

            _balanceState.State.Transactions.Add(transaction);

            var newBalance = currentBalance + creditAmount;

            _balanceState.State = _balanceState.State with { CurrentBalance = newBalance };

            await _balanceState.WriteStateAsync();
        }

        public async Task Debit(decimal debitAmount)
        {
            var currentBalance = _balanceState.State.CurrentBalance;

            if (debitAmount < 0)
                throw new ArgumentException("debits must be greater than 0");

            if (_balanceState.State.CurrentBalance < debitAmount)
                throw new ArgumentException("debit amount exceeds balance");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionAmount = debitAmount,
                TransactionDateTimeUtc = DateTime.UtcNow,
            };

            _balanceState.State.Transactions.Add(transaction);

            var newBalance = currentBalance - debitAmount;

            _balanceState.State = _balanceState.State with { CurrentBalance = newBalance };

            await _balanceState.WriteStateAsync();
        }
    }
}
