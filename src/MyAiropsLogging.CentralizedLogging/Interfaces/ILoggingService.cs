using MyAiropsLogging.Shared;

namespace MyAiropsLogging.CentralizedLogging.Interfaces
{
    public interface ILoggingService
    {
        public void LogMessage(LogMessageDto logmessage);
        public string TruncateLog(string logmessage);

    }
}
