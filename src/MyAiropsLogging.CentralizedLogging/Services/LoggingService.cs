using MyAiropsLogging.CentralizedLogging.Interfaces;
using MyAiropsLogging.Shared;

using Serilog;

namespace MyAiropsLogging.CentralizedLogging.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly Serilog.ILogger _logger;

        public LoggingService(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public string TruncateLog(string message)
        {
            return string.Empty;
        }

        public void LogMessage(LogMessageDto log)
        {
            
        }
    }
}
