using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Client.Contracts
{
    [DataContract]
    public record CreateAtm
    {
        [DataMember]
        public decimal OpeningBalance { get; init; }
    }
}
