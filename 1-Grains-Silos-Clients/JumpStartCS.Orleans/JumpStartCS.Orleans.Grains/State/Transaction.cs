namespace JumpStartCS.Orleans.Grains.State
{
    [GenerateSerializer]
    public record Transaction
    {
        [Id(0)]
        public Guid Id { get; set; }

        [Id(1)]
        public DateTime TransactionDateTimeUtc { get ; set; }

        [Id(2)]
        public decimal TransactionAmount { get; set; }
    }
}
