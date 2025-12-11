using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;
using OpenAI.Images;

namespace MicrosoftAgentFramework.Utilities;

public class ImageClientProvider
{
    private const string AzureOpenAiEndpoint = "https://nad-openai-azure.openai.azure.com/";

    public static ImageClient GetOpenAiClient(string model)
    {
        var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ??
                     throw new InvalidOperationException("Please set the OpenAI__ApiKey environment variable.");
        var client = new OpenAIClient(new ApiKeyCredential(apiKey));
        var imageClient = client.GetImageClient(model);
        return imageClient;
    }

    public static ImageClient GetAzureOpenAiClient(string model)
    {
        var client = new AzureOpenAIClient(new Uri(AzureOpenAiEndpoint), new AzureCliCredential());
        var imageClient = client.GetImageClient(model);
        return imageClient;
    }
}