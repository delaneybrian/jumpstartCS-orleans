using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains
{
    [DataContract]
    public record CheckingAccountState
    {
        [DataMember]
        public Guid AccountId { get; init; }

        [DataMember]
        public Guid CustomerId { get; init; }

        [DataMember]
        public string AccountType { get; init;  }

        [DataMember]
        public DateTime OpenedAtUtc { get; init; }
    }
}
