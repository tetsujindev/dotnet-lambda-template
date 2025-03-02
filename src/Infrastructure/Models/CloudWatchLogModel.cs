namespace Infrastructure.Models;

public class CloudWatchLogModel
{
    public required string LogGroup { get; set; }
    public required string LogStream { get; set; }
    public required string Level { get; set; }
    public required string RequestId { get; set; }
    public required string TraceId { get; set; }
    public required string Message { get; set; }

    public static CloudWatchLogModel Create(string logGroup, string logStream, string level, string requestId, string traceId, string? message, string? errorMessage)
    {
        return new CloudWatchLogModel
        {
            LogGroup = logGroup,
            LogStream = logStream,
            Level = level,
            RequestId = requestId,
            TraceId = traceId,
            Message = message ?? errorMessage ?? string.Empty
        };
    }
}
