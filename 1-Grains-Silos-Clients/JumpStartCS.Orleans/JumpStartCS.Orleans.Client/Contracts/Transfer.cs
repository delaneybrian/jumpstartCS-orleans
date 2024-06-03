using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Client.Contracts
{
    [DataContract]
    public record Transfer
    {
        [DataMember]
        public decimal Amount { get; init; }

        [DataMember]
        public Guid FromAccountId { get; init; }

        [DataMember]
        public Guid ToAccountId { get; init; }
    }
}
