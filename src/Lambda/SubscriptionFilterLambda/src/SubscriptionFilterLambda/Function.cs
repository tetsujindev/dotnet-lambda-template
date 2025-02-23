using Amazon.Lambda.Core;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SubscriptionFilterLambda;

public class Function
{
    private readonly TeamsWebhookService teamsWebhookService;
    private readonly SimpleNotificationServiceService simpleNotificationServiceService;
    private readonly string topicArn;

    public Function()
    {
        // teamsWebhookService = new("https://prod-02.japaneast.logic.azure.com:443/workflows/a7052248f26f45b7b6e079ce7a32c352/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=EOuGFLDdCqgFcvEWocglARAbSac_34BWbLBjiJy0afA");
        teamsWebhookService = new TeamsWebhookService(Environment.GetEnvironmentVariable("teams_webhook_uri") ?? throw new ArgumentNullException("teams_webhook_uri"));
        //simpleNotificationServiceService = new SimpleNotificationServiceService();
        //topicArn = Environment.GetEnvironmentVariable("topic_arn") ?? throw new ArgumentNullException("topic_arn");
    }

    public string FunctionHandler(string input, ILambdaContext context)
    {
        return input.ToUpper();
    }

    public async Task<string> PublishFromSubscriptionFilterToChatbotAsync(SubsctiptionFilterInputModel input, ILambdaContext context)
    {
        context.Logger.LogLine(input.GetLogMessage());
        return await simpleNotificationServiceService.PublishToTopicForChatbotAsync(topicArn, input.GetLogMessage());
    }

    public async Task PublishFromSubscriptionFilterToTeamsWebhookAsync(SubsctiptionFilterInputModel input, ILambdaContext context)
    {
        var logEvents = JsonDocument.Parse(input.GetLogMessage()).RootElement.GetProperty("logEvents");
        foreach (var logEvent in logEvents.EnumerateArray())
        {
            var message = logEvent.GetProperty("message").GetString();
            
            if (message != null)
            {
                await teamsWebhookService.SendMessageAsync(message);
            }
        }
    }
}
