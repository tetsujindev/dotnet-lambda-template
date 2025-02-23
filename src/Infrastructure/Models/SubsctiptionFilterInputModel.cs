using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

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
}
