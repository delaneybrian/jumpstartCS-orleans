using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record CustomerDetailsState
    {
        [DataMember]
        public string Name { get; set; }
    }
}
