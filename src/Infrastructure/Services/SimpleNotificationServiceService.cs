using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace Infrastructure.Services;

public class SimpleNotificationServiceService
{
    private readonly IAmazonSimpleNotificationService simpleNotificationService;

    public SimpleNotificationServiceService()
    {
        simpleNotificationService = new AmazonSimpleNotificationServiceClient();
    }

    public async Task<string> PublishToTopicAsync(string topicArn, string messageText)
    {
        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = messageText,
        };

        var response = await simpleNotificationService.PublishAsync(request);
        return response.MessageId;
    }

    public async Task<string> PublishToTopicForChatbotAsync(string topicArn, string messageText)
    {
        var messageForChatbot = new SimpleNotificationServiceMessageForChatbotModel(messageText);

        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(messageForChatbot),
        };

        var response = await simpleNotificationService.PublishAsync(request);
        return response.MessageId;
    }
}

public class SimpleNotificationServiceMessageForChatbotModel(string messageText)
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";
    [JsonPropertyName("source")]
    public string Source { get; set; } = "custom";
    [JsonPropertyName("content")]
    public ContentObject Content { get; set; } = new ContentObject(messageText);

    public class ContentObject(string description)
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = description;
    }
}
