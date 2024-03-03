using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record CustomerAccountsState
    {
        [DataMember]
        public ICollection<Guid> CheckingAccountIds { get; set; } = new HashSet<Guid>();
    }
}
