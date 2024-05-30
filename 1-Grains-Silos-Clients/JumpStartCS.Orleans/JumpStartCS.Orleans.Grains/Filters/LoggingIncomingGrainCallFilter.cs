using Microsoft.Extensions.Logging;

namespace JumpStartCS.Orleans.Grains.Filters
{
    public class LoggingIncomingGrainCallFilter : IIncomingGrainCallFilter
    {
        private readonly ILogger _logger;

        public LoggingIncomingGrainCallFilter(ILogger<LoggingIncomingGrainCallFilter> logger)
        { 
            _logger = logger; 
        }

        public Task Invoke(IIncomingGrainCallContext context)
        {
            _logger.LogInformation($"Incoming Silo Grain Filter: Recived grain call on '{context.Grain}' to '{context.MethodName}' method");

            return context.Invoke();
        }
    }
}
