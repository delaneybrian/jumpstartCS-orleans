using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record BalanceState
    {
        [DataMember]
        public decimal CurrentBalance { get; init; }

        [DataMember]
        public ICollection<Transaction> Transactions { get; init; } = new List<Transaction>();

        [DataMember]
        public ICollection<RecurringPayment> RecurringPayments { get; init; } = new List<RecurringPayment>();
    }
}
