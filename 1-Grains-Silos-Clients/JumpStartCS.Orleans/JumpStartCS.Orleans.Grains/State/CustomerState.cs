namespace JumpStartCS.Orleans.Grains.State
{
    [GenerateSerializer]
    public record CustomerState
    {
        [Id(0)]
        public Dictionary<Guid, decimal> BalanceByCheckingAccountId { get; set; } = new();
    }
}
