﻿using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Models;

public class SubscriptionFilterPayloadModel
{
    [JsonPropertyName("awslogs")]
    public required AwsLogsObject AwsLogs { get; set; }

    public class AwsLogsObject
    {
        [JsonPropertyName("data")]
        public required string Base64EncodedGzipCompressedData { get; set; }
    }

    private DataObject GetData()
    {
        using var compressedStream = new MemoryStream(Convert.FromBase64String(this.AwsLogs.Base64EncodedGzipCompressedData));
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        gzipStream.CopyTo(decompressedStream);

        var data = JsonSerializer.Deserialize<DataObject>(Encoding.UTF8.GetString(decompressedStream.ToArray()))!;
        return data;
    }

    public List<CloudWatchLogModel> GetLogs()
    {
        var data = GetData();
        var logs = new List<CloudWatchLogModel>();

        foreach (var logEvent in data.LogEvents)
        {
            var message = JsonSerializer.Deserialize<MessageObject>(logEvent.Message)!;

            logs.Add(CloudWatchLogModel.Create(
                logGroup: data.LogGroup,
                logStream: data.LogStream,
                level: message.Level,
                requestId: message.RequestId,
                traceId: message.TraceId,
                message: message.Message,
                errorMessage: message.ErrorMessage));
        }

        return logs;
    }

    public class DataObject
    {
        [JsonPropertyName("logGroup")]
        public required string LogGroup { get; set; }
        [JsonPropertyName("logStream")]
        public required string LogStream { get; set; }
        [JsonPropertyName("logEvents")]
        public required List<LogEventObject> LogEvents { get; set; }
    }

    public class LogEventObject
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }

    public class MessageObject
    {
        [JsonPropertyName("requestId")]
        public required string RequestId { get; set; }
        [JsonPropertyName("traceId")]
        public required string TraceId { get; set; }
        [JsonPropertyName("level")]
        public required string Level { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
