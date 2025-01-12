using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Infrastructure.Data;
using Infrastructure.Models;
using Infrastructure.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloLambda;

public class Function
{
    private readonly SecretsManagerService secretsManagerService;
    private readonly BookRepository bookRepository;

    public Function()
    {
        secretsManagerService = new SecretsManagerService();
        //bookRepository = new BookRepository(secretsManagerService.GetSecretAsync<string>("connectionstring").Result);
        bookRepository = new BookRepository(Environment.GetEnvironmentVariable("connectionstring")!);
    }

    public string FunctionHandler(string input, ILambdaContext context)
    {
        context.Logger.LogLine($"Processing input!!!: {input}");
        return "HELLOOOOO!!";
    }

    public async Task<string> GetSecretString(string secretName)
    {
        var secret = await secretsManagerService.GetSecretAsync<string>(secretName);
        return secret;
    }

    public async Task<SecretObject> GetSecretObject(string secretName)
    {
        var secret = await secretsManagerService.GetSecretAsync<SecretObject>(secretName);
        return secret;
    }

    public async Task<IEnumerable<BookDataModel>> GetBooks(string input, ILambdaContext context)
    {
        return await bookRepository.GetBooksAsync();
    }
}

public class SecretObject
{
    [JsonPropertyName("username")]
    public required string UserName { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}
