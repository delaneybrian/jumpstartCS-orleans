using Orleans;

namespace JumpStartCS.Orleans.Grains
{
    internal class CheckingAccountGrain : Grain, ICheckingAccountGrain
    {
        public async Task LogBalance()
        {
            Console.WriteLine("80 is the balance");
        }
    }
}
