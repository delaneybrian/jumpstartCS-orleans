using Microsoft.Extensions.Logging;

namespace JumpStartCS.Orleans.Grains.Filters
{
    public class LoggingOutgoingGrainCallFilter : IOutgoingGrainCallFilter
    {
        private readonly ILogger _logger;

        public LoggingOutgoingGrainCallFilter(ILogger<LoggingOutgoingGrainCallFilter> logger)
        {
            _logger = logger;
        }

        public Task Invoke(IOutgoingGrainCallContext context)
        {
            _logger.LogInformation($"Outcoming Atm Grain Filer: Recived grain call on '{context.Grain}' to '{context.MethodName}' method");

            return context.Invoke();
        }
    }
}
