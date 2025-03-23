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
            if (!string.IsNullOrEmpty(message) && message.Length > 255)
            {
                message = message.Substring(0, 252) + "...";
            }
            return message;
        }

        public void LogMessage(LogMessageDto log)
        {
            if (log == null)
                throw new ArgumentException("Log Message is required");

            log.MessageTemplate = TruncateLog(log.MessageTemplate);

            var logFormatted = $" ID : [{log.Id}] | {log.MessageTemplate}";

            switch (log.Level)
            {
                case LevelDto.Error:
                    _logger.Error(logFormatted);
                    break;
                case LevelDto.Warning:
                    _logger.Warning(logFormatted);
                    break;
                case LevelDto.Fatal:
                    _logger.Fatal(logFormatted);
                    break;
                default:
                    _logger.Information(logFormatted);
                    break;
            }
        }
    }
}
