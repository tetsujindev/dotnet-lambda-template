using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using static HelloTeamsWebhookLambda.TeasmWebhookBody;
using static HelloTeamsWebhookLambda.TeasmWebhookBody.Attachment;
using static HelloTeamsWebhookLambda.TeasmWebhookBody.Attachment.ContentClass;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloTeamsWebhookLambda;

public class Function
{
    private readonly HttpClient httpClient;
    public Function()
    {
        httpClient = new();
    }

    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        var teasmWebhookBody = new TeasmWebhookBody()
        {
            Type = "message",
            Attachments = new List<Attachment>() {
                new Attachment() {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    ContentUrl = null,
                    Content = new ContentClass() {
                        Schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                        Type = "AdaptiveCard",
                        Version = "1.4",
                        Body = new List<BodyItem>() {
                            new BodyItem() {
                                Type = "TextBlock",
                                Text = "Hello from Lambda??"
                            }
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize<TeasmWebhookBody>(teasmWebhookBody);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://prod-02.japaneast.logic.azure.com:443/workflows/a7052248f26f45b7b6e079ce7a32c352/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=EOuGFLDdCqgFcvEWocglARAbSac_34BWbLBjiJy0afA"),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return input.ToUpper();
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
            }
        }
    }
}
