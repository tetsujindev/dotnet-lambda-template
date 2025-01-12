using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Infrastructure.Services;

public class SecretsManagerService
{
    private readonly IAmazonSecretsManager secretsManager;
    private readonly string region = "ap-northeast-1";

    public SecretsManagerService()
    {
        secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
    }

    public async Task<T> GetSecretAsync<T>(string secretName) where T : class
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        var response = await secretsManager.GetSecretValueAsync(request);

        return JsonSerializer.Deserialize<T>(response.SecretString) ?? throw new Exception("Failed to deserialize secret");
    }
}
