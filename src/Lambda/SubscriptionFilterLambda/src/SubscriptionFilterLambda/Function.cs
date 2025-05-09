using Amazon.Lambda.Core;
using Infrastructure.Services;
using Infrastructure.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SubscriptionFilterLambda;

public class Function
{
    private readonly TeamsWebhookService teamsWebhookService;

    public Function()
    {
        teamsWebhookService = new TeamsWebhookService(Environment.GetEnvironmentVariable("teams_webhook_uri") ?? throw new ArgumentNullException("teams_webhook_uri"));
    }

    public async Task PublishFromSubscriptionFilterToTeamsWebhookAsync(SubscriptionFilterPayloadModel payload, ILambdaContext context)
    {
        context.Logger.LogInformation("start function {context.FunctionName} with request {@payload}", context.FunctionName, payload);

        var logs = payload.GetLogs();

        foreach (var log in logs)
        {
            await teamsWebhookService.SendLogAsync(log);
            context.Logger.LogInformation("send to teams webhook: {@log}", log);
        }

        context.Logger.LogInformation("end function {context.FunctionName} with no response", context.FunctionName);
    }
}
