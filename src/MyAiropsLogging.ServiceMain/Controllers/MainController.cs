using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using MyAiropsLogging.Shared;

[ApiController]
[Route("api/[controller]")]
public class MainController : ControllerBase
{
    private readonly IRabbitMQPublisher<LogMessageDto> _rabbitMQPublisher;

    public MainController(IRabbitMQPublisher<LogMessageDto> rabbitMQPublisher)
    {
        Console.WriteLine("MainController initiated");
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
                "error" => LevelDto.Error,
                "fatal" => LevelDto.Fatal,
                "success" => LevelDto.Information,
                "information"
                 or
                "info" => LevelDto.Information,

                _ => LevelDto.Warning
            };

            var message = level == LevelDto.Information && (simulate == "success" || simulate == "info" || simulate == "information")
                ? "Microservice (Main) processed the request successfully"
                : level == LevelDto.Warning && simulate != "warning" ? $"Unknown request {simulate}" : $"Microservice (Main) raised a {simulate}";

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
