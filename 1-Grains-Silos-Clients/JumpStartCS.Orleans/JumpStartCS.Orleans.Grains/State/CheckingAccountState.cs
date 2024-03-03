using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record CheckingAccountState
    {
        [DataMember]
        public int Balance { get; set; }
    }
}
