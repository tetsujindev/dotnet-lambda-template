namespace Infrastructure.Models;

public class CloudWatchLogModel
{
    public required string LogGroup { get; set; }
    public required string LogStream { get; set; }
    public required string Level { get; set; }
    public required string RequestId { get; set; }
    public required string TraceId { get; set; }
    public string? Message { get; set; }
}
