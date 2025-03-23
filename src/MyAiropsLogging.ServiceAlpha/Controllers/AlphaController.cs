using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using MyAiropsLogging.Shared;

[ApiController]
[Route("api/[controller]")]
public class AlphaController : ControllerBase
{
    private readonly IRabbitMQPublisher<LogMessageDto> _rabbitMQPublisher;

    public AlphaController(IRabbitMQPublisher<LogMessageDto> rabbitMQPublisher)
    {
        Console.WriteLine("AlphaController initiated");
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpGet("process")]
    public async Task<IActionResult> ProcessRequest(string simulate)
    {
        try
        {
        var level = simulate.ToLower() switch
        {
            "warning" => LevelDto.Warning,
            "error"   => LevelDto.Error,
            "fatal"   => LevelDto.Fatal,
            "success" => LevelDto.Information,
            _         => LevelDto.Warning
        };

            var message = level == LevelDto.Information && simulate == "success" 
                ? " Service [A] processed the request successfully" 
                : $"Service [A] raised a {simulate}";

            var logMessage = new LogMessageDto 
                {
                    MessageTemplate = message,
                    Level = level,
                    Id = Guid.NewGuid()
                };

            await _rabbitMQPublisher.PublishMessageAsync(logMessage, RabbitQueues.LogsQ);

            return Ok(new { Message = message, ID = logMessage.Id });
        }
        catch (Exception e)
        {
            return BadRequest("Error occured" + e);
        }
    }
}
