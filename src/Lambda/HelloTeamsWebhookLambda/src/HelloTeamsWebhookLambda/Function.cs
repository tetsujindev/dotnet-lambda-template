using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Infrastructure.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloTeamsWebhookLambda;

public class Function
{
    private readonly TeamsWebhookService teamsWebhookService;
    public Function()
    {
        teamsWebhookService = new("https://prod-02.japaneast.logic.azure.com:443/workflows/a7052248f26f45b7b6e079ce7a32c352/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=EOuGFLDdCqgFcvEWocglARAbSac_34BWbLBjiJy0afA");
    }

    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        await teamsWebhookService.SendMessageAsync(input);
        return input.ToUpper();
    }
}
