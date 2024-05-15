using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Client.Contracts
{
    [DataContract]
    public record AtmWithdrawl
    {
        [DataMember]
        public decimal WithdrawlAmount { get; init; }

        [DataMember]
        public Guid CheckingAccountId { get; init; }
    }
}
