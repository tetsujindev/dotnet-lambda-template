using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Infrastructure.Data;
using Infrastructure.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookLambda;

public class Function
{
    private readonly SecretsManagerService secretsManagerService;
    private readonly BookRepository bookRepository;

    public Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        secretsManagerService = new SecretsManagerService();
        //bookRepository = new BookRepository(secretsManagerService.GetSecretAsync<string>("connectionstring").Result);
        bookRepository = new BookRepository(Environment.GetEnvironmentVariable("connectionstring")!);
    }

    public string FunctionHandler(string input, ILambdaContext context)
    {
        return input.ToUpper();
    }

    public async Task<APIGatewayProxyResponse> GetBooks(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var books = await bookRepository.GetBooksAsync();
        return new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(books),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
