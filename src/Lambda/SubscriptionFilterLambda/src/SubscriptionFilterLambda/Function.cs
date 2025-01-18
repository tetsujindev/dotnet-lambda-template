using Amazon.Lambda.Core;
using Infrastructure.Services;
using Infrastructure.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SubscriptionFilterLambda;

public class Function
{
    private readonly SimpleNotificationServiceService simpleNotificationServiceService;
    private readonly string topicArn;

    public Function()
    {
        simpleNotificationServiceService = new SimpleNotificationServiceService();
        topicArn = Environment.GetEnvironmentVariable("topic_arn") ?? throw new ArgumentNullException("topic_arn");
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
}
