using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdaptiveCards;
using Infrastructure.Models;

namespace Infrastructure.Services;

public class TeamsWebhookService(string teamsWebhookUri)
{
    private readonly HttpClient httpClient = new();
    private readonly Uri uri = new(teamsWebhookUri);

    public async Task SendLogAsync(CloudWatchLogModel log)
    {
        TeasmWebhookBody teasmWebhookBody = TeasmWebhookBody.CreateFromLog(log);

        var json = JsonSerializer.Serialize(teasmWebhookBody);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}


public class TeasmWebhookBody
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("attachments")]
    public required List<Attachment> Attachments { get; set; }

    private static AdaptiveCard CreateAdaptiveCardFromLog(CloudWatchLogModel log)
    {
        return new AdaptiveCard("1.4")
        {
            Type = "AdaptiveCard",
            Version = "1.4",
            Body =
            [
                new AdaptiveTextBlock
                {
                    Type = "TextBlock",
                    Text = $"{log.Level} Log Detected",
                    Size = AdaptiveTextSize.Large,
                    Color = AdaptiveTextColor.Attention
                },
                new AdaptiveFactSet
                {
                    Type = "FactSet",
                    Facts =
                    {
                        new AdaptiveFact("Log Group", log.LogGroup),
                        new AdaptiveFact("Log Group", log.Level),
                        new AdaptiveFact("Log Stream", log.LogStream),
                        new AdaptiveFact("Request ID", log.RequestId),
                        new AdaptiveFact("Trace ID", log.TraceId)
                    }
                },
                new AdaptiveTextBlock
                {
                    Type = "TextBlock",
                    Text = log.Message,
                    Wrap = true
                }
            ]
        };
    }

    private static AdaptiveCard CreateAdaptiveCardFromText(string text)
    {
        return new AdaptiveCard("1.4")
        {
            Type = "AdaptiveCard",
            Version = "1.4",
            Body =
            [
                new AdaptiveTextBlock
                {
                    Type = "TextBlock",
                    Text = text,
                    Wrap = true
                }
            ]
        };
    }

    public static TeasmWebhookBody CreateFromLog(CloudWatchLogModel log)
    {
        return new TeasmWebhookBody()
        {
            Type = "message",
            Attachments = [
                new() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = JsonDocument.Parse(CreateAdaptiveCardFromLog(log).ToJson())
                }
            ]
        };
    }

    public static TeasmWebhookBody CreateFromText(string text)
    {
        return new TeasmWebhookBody()
        {
            Type = "message",
            Attachments = [
                new() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = JsonDocument.Parse(CreateAdaptiveCardFromText(text).ToJson())
                }
            ]
        };
    }

    public class Attachment
    {
        [JsonPropertyName("contentType")]
        public required string ContentType { get; set; }
        [JsonPropertyName("contentUrl")]
        public string? ContentUrl { get; set; }
        [JsonPropertyName("content")]
        public required JsonDocument Content { get; set; }
    }
}
