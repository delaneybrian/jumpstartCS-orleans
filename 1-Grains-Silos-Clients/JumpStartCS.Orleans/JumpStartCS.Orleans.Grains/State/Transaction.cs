using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record Transaction
    {
        [DataMember]
        public Guid Id { get; init; }

        [DataMember]
        public DateTime TransactionDateTimeUtc { get ; init; }

        [DataMember]
        public decimal TransactionAmount { get; init; }
    }
}
