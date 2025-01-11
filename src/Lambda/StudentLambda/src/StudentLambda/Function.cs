using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StudentLambda;

public class Function
{
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var student1 = new Student { Id = 1, Name = "John Doe" };
        var student2 = new Student { Id = 2, Name = "Alice Smith" };
        var students = new List<Student> { student1, student2 };
        return new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(students),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}

public class Student
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
