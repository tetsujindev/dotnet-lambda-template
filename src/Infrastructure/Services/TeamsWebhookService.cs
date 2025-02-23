using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class TeamsWebhookService
{
    private readonly HttpClient httpClient;
    private readonly Uri uri;

    public TeamsWebhookService(string teamsWebhookUri)
    {
        httpClient = new();
        uri = new(teamsWebhookUri);
    }

    public async Task SendMessageAsync(string message)
    {
        TeasmWebhookBody teasmWebhookBody = new()
        {
            Type = "message",
            Attachments = [
                new() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = new() {
                        Schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                        Type = "AdaptiveCard",
                        Version = "1.4",
                        Body = [
                            new() {
                                Type = "TextBlock",
                                Text = message,
                                Wrap = true
                            }
                        ]
                    }
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

    public class Attachment
    {
        [JsonPropertyName("contentType")]
        public required string ContentType { get; set; }
        [JsonPropertyName("contentUrl")]
        public string? ContentUrl { get; set; }
        [JsonPropertyName("content")]
        public required ContentClass Content { get; set; }

        public class ContentClass
        {
            [JsonPropertyName("$schema")]
            public required string Schema { get; set; }
            [JsonPropertyName("type")]
            public required string Type { get; set; }
            [JsonPropertyName("version")]
            public required string Version { get; set; }
            [JsonPropertyName("body")]
            public required List<BodyItem> Body { get; set; }

            public class BodyItem
            {
                [JsonPropertyName("type")]
                public required string Type { get; set; }
                [JsonPropertyName("text")]
                public required string Text { get; set; }
                [JsonPropertyName("wrap")]
                public bool Wrap { get; set; }
            }
        }
    }
}