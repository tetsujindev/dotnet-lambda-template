using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace Infrastructure.Models;

public class SubsctiptionFilterInputModel
{
    [JsonPropertyName("awslogs")]
    public required AwsLogsObject AwsLogs { get; set; }

    public class AwsLogsObject
    {
        [JsonPropertyName("data")]
        public required string Data { get; set; }
    }

    public string GetLogMessage()
    {
        using var compressedStream = new MemoryStream(Convert.FromBase64String(this.AwsLogs.Data));
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);
        return Encoding.UTF8.GetString(decompressedStream.ToArray());
    }

    public LogInfo GetLogInfo()
    {
        using var compressedStream = new MemoryStream(Convert.FromBase64String(this.AwsLogs.Data));
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);

        var logInfo = JsonSerializer.Deserialize<LogInfo>(Encoding.UTF8.GetString(decompressedStream.ToArray()));
        return logInfo;
    }

    public Message GetMessage()
    {
        using var compressedStream = new MemoryStream(Convert.FromBase64String(this.AwsLogs.Data));
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);

        var logInfo = JsonSerializer.Deserialize<LogInfo>(Encoding.UTF8.GetString(decompressedStream.ToArray()));
        var message = JsonSerializer.Deserialize<Message>(logInfo.LogEvents[0].Message);
        return message;
    }
}

public class LogInfo
{
    [JsonPropertyName("messageType")]
    public required string MessageType { get; set; }
    [JsonPropertyName("owner")]
    public required string Owner { get; set; }
    [JsonPropertyName("logGroup")]
    public required string LogGroup { get; set; }
    [JsonPropertyName("logStream")]
    public required string LogStream { get; set; }
    [JsonPropertyName("logEvents")]
    public required List<LogEvent> LogEvents { get; set; }

    public class LogEvent
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }
}

public class Message
{
    [JsonPropertyName("level")]
    public required string Level { get; set; }
    [JsonPropertyName("requestId")]
    public required string RequestId { get; set; }
    [JsonPropertyName("traceId")]
    public required string TraceId { get; set; }
    [JsonPropertyName("message")]
    public required string Body { get; set; }
}
