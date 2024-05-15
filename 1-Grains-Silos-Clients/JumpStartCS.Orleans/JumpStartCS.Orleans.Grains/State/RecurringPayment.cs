using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.State
{
    [DataContract]
    public record RecurringPayment
    {
        [DataMember]
        public Guid Id { get; init; }

        [DataMember]
        public decimal PaymentAmount { get; init; }
    }
}
