using MyAiropsLogging.CentralizedLogging.Interfaces;
using MyAiropsLogging.CentralizedLogging.Services;
using Serilog;
using Moq;
using MyAiropsLogging.Shared;
using Serilog.Events;

namespace MyAiropsLogging.CentralizedLogging.Tests;

public class LoggingServiceTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly ILoggingService _loggingService;

    public LoggingServiceTests()
    {
        _loggerMock = new Mock<ILogger>();
        _loggingService = new LoggingService(_loggerMock.Object);
    }
    [Fact]
    public void LogMessage_ShouldCallLogInfoMessage()
    {
        var logMessage = new LogMessageDto { MessageTemplate = "Test info", Level = LevelDto.Information };
        _loggingService.LogMessage(logMessage);

        _loggerMock.Verify(logger => logger.Information(It.Is<string>(s => s.Contains("Test info"))), Times.Once);
    }


    [Fact]
    public void LogMessage_ShouldThrowExceptionWhenMessageIsNull()
    {
        Action action = () => _loggingService.LogMessage(null);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void LogMessage_ShouldTruncateMessageWhenMessageLengthIsBiggerThan255()
    {
        var message = new string('l', 300);
        var returnedMessage = _loggingService.TruncateLog(message);

        Assert.Equal(255, returnedMessage.Length);
    }
}
