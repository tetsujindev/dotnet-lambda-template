using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Infrastructure.Services;

public class SqsService
{
    private readonly IAmazonSQS sqsClient;

    public SqsService()
    {
        sqsClient = new AmazonSQSClient();
    }

    public async Task SendMessageAsync(string queueUrl, object messageObject)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(messageObject),
        };

        await sqsClient.SendMessageAsync(request);
    }
}
