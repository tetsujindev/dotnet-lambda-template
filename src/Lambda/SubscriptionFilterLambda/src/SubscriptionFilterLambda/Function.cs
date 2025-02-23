using Amazon.Lambda.Core;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SubscriptionFilterLambda;

public class Function
{
    private readonly TeamsWebhookService teamsWebhookService;

    public Function()
    {
        teamsWebhookService = new TeamsWebhookService(Environment.GetEnvironmentVariable("teams_webhook_uri") ?? throw new ArgumentNullException("teams_webhook_uri"));
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
