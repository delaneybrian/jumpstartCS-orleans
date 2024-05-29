using JumpStartCS.Orleans.Grains.State;
using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains
{
    [GenerateSerializer]
    [DataContract]
    public record CheckingAccountState
    {
        [DataMember]
        public Guid AccountId { get; set; }

        [DataMember]
        public Guid CustomerId { get; set; }

        [DataMember]
        public string AccountType { get; set;  }

        [DataMember]
        public DateTime OpenedAtUtc { get; set; }

        [DataMember]
        public List<RecurringPayment> RecurringPayments { get; set; } = new List<RecurringPayment>();
    }
}
