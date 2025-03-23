using System.ComponentModel.DataAnnotations;

namespace MyAiropsLogging.Shared
{
    public class LogMessageDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public LevelDto Level { get; set; }

        //Log Message
        [MaxLength(255)]
        public required string MessageTemplate { get; set; }
    }
}