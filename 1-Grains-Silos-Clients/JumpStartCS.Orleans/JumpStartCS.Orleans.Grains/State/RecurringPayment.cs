namespace JumpStartCS.Orleans.Grains.State
{
    [GenerateSerializer]
    public record RecurringPayment
    {
        [Id(0)]
        public Guid Id { get; set; }

        [Id(1)]
        public decimal PaymentAmount { get; set; }
    }
}
