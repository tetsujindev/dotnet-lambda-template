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

    public async Task SendMessageAsync(string message)
    {
        TeasmWebhookBody teasmWebhookBody = new()
        {
            Type = "message",
            Attachments = [
                new() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = JsonDocument.Parse(new AdaptiveCard("1.4") {
                        Type = "AdaptiveCard",
                        Version = "1.4",
                        Body = [
                            new AdaptiveTextBlock() {
                                Type = "TextBlock",
                                Text = message,
                                Wrap = true
                            }
                        ]
                    }.ToJson())
                }
            ]
        };

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

    public static TeasmWebhookBody CreateFromLog(CloudWatchLogModel log)
    {
        return new TeasmWebhookBody()
        {
            Type = "message",
            Attachments = [
                new() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = JsonDocument.Parse(new AdaptiveCard("1.4") {
                        Type = "AdaptiveCard",
                        Version = "1.4",
                        Body = [
                            new AdaptiveTextBlock() {
                                Type = "TextBlock",
                                Text = $"{log.Level} Log Detected",
                                Size = AdaptiveTextSize.Medium,
                                Color = AdaptiveTextColor.Attention
                            },
                            new AdaptiveTextBlock() {
                                Type = "TextBlock",
                                Text = log.Message,
                                Wrap = true
                            }
                        ]
                    }.ToJson())
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
