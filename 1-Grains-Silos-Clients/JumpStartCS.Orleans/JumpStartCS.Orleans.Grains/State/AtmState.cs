using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record AtmState
    {
        [DataMember]
        public Guid Id { get; init; }

        [DataMember]
        public decimal Balance { get; init; }
    }
}
