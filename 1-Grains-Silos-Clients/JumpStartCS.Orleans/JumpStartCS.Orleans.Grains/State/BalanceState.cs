namespace JumpStartCS.Orleans.Grains.State
{
    [GenerateSerializer]
    public record class BalanceState
    {
        [Id(0)]
        public decimal CurrentBalance { get; set; }

        [Id(1)]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
