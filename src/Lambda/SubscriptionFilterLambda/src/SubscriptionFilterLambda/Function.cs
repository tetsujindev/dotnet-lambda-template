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
        var logInfo = input.GetLogInfo();
        var message = input.GetMessage();
        await teamsWebhookService.SendMessageAsync($"{logInfo.LogGroup} {logInfo.LogStream} {message.Level} {message.RequestId} {message.TraceId} {message.Body}");
    }
}
