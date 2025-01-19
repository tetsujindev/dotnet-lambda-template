using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Models;
using Infrastructure.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BookLambda;

public class Function
{
    private readonly SecretsManagerService secretsManagerService;
    private readonly BookRepository bookRepository;
    private readonly SqsService sqsService;
    private readonly StepFunctionsService stepFunctionsService;

    public Function()
    {
        AWSSDKHandler.RegisterXRayForAllServices();
        secretsManagerService = new SecretsManagerService();
        //bookRepository = new BookRepository(secretsManagerService.GetSecretAsync<string>("connectionstring").Result);
        bookRepository = new BookRepository(Environment.GetEnvironmentVariable("connectionstring")!);
        sqsService = new SqsService();
        stepFunctionsService = new StepFunctionsService();
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

    public async Task<APIGatewayProxyResponse> AddBook(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogLine(request.Body);
        var book = JsonSerializer.Deserialize<Book>(request.Body);
        context.Logger.LogLine($"Adding book {book.Id} {book.Title} {book.Author}");
        //book = new Book("9", "Supernova Era", "Liu Cixin");
        context.Logger.LogLine($"Adding book {book.Id} {book.Title} {book.Author}");
        await sqsService.SendMessageAsync("https://sqs.ap-northeast-1.amazonaws.com/194722443726/BookQueue", book);
        return new APIGatewayProxyResponse
        {
            StatusCode = 204
        };
    }

    public async Task SendBookFromQueueToStepFunction(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            var book = JsonSerializer.Deserialize<Book>(record.Body);
            context.Logger.LogLine($"Processing book {book.Id} {book.Title} {book.Author}");
            await stepFunctionsService.StartExecutionAsync("arn:aws:states:ap-northeast-1:194722443726:stateMachine:BookStateMachine", book);
        }
    }

    public async Task AddBookToDatabase(Book book, ILambdaContext context)
    {
        await bookRepository.AddBookAsync(book);
    }
}
