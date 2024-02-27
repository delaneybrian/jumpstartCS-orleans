namespace JumpStartCS.Orleans.Grains
{
    public class CheckingAccountGrain : Grain, ICheckingAccountGrain
    {
        public async Task<int> GetBalance()
        {
            Console.WriteLine("80 is the balance");

            return 80;
        }
    }
}
