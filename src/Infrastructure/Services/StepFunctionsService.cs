using System.Text.Json;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;

namespace Infrastructure.Services;

public class StepFunctionsService
{
    private readonly IAmazonStepFunctions stepFunctionsClient;

    public StepFunctionsService()
    {
        stepFunctionsClient = new AmazonStepFunctionsClient();
    }

    public async Task StartExecutionAsync(string stateMachineArn, object inputObject)
    {
        var request = new StartExecutionRequest
        {
            StateMachineArn = stateMachineArn,
            Input = JsonSerializer.Serialize(inputObject),
        };

        await stepFunctionsClient.StartExecutionAsync(request);
    }
}
