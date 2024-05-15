using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Client.Contracts
{
    [DataContract]
    public record CreateRecurringPayment
    {
        [DataMember]
        public Guid Id { get; init; }

        [DataMember]
        public decimal Amount { get; init; }

        [DataMember]
        public int ReccursEveryMinute { get; init; }
    }
}
