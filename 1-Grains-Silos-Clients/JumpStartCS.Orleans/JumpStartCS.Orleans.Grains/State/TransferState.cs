namespace JumpStartCS.Orleans.Grains.State
{
    [GenerateSerializer]
    public record TransferState
    {
        [Id(0)]
        public long TransferCount { get; set; }
    }
}
