using System.Runtime.Serialization;

namespace JumpStartCS.Orleans.Grains.Events
{
    [GenerateSerializer]
    public record BalanceChangeEvent
    {
        [Id(0)]
        public Guid AccountId { get; init; }

        [Id(1)]
        public decimal Balance { get; init; }
    }
}
