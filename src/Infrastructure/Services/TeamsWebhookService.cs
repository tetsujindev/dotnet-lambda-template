using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
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

    private static AdaptiveCard CreateAdaptiveCardFromLog(CloudWatchLogModel log, string? region = "ap-northeast-1")
    {
        var encodedLogGroup = HttpUtility.UrlEncode(HttpUtility.UrlEncode(log.LogGroup)).Replace("%", "$");
        var encodedRequestId = HttpUtility.UrlEncode(HttpUtility.UrlEncode($"\"{log.RequestId}\"")).Replace("%", "$");
        var cloudWatchLogsUrl = $"https://{region}.console.aws.amazon.com/cloudwatch/home?region={region}#logsV2:log-groups/log-group/{encodedLogGroup}/log-events$3FfilterPattern$3D{encodedRequestId}";

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
                        new AdaptiveFact("Level", log.Level),
                        new AdaptiveFact("Log Group", log.LogGroup),
                        new AdaptiveFact("Request ID", log.RequestId)
                    }
                },
                new AdaptiveTextBlock
                {
                    Type = "TextBlock",
                    Text = log.Message,
                    Wrap = true
                },
                new AdaptiveActionSet
                {
                    Type = "ActionSet",
                    Actions =
                    {
                        new AdaptiveOpenUrlAction
                        {
                            Type = "Action.OpenUrl",
                            Title = "Open in CloudWatch Logs",
                            Url = new Uri(cloudWatchLogsUrl),
                            Style = "positive"
                        }
                    }
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
