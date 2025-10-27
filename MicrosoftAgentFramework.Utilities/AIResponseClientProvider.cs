// ReSharper disable InconsistentNaming

using System.ClientModel;
using OpenAI;
using OpenAI.Responses;

#pragma warning disable OPENAI001

namespace MicrosoftAgentFramework.Utilities;

public static class AIResponseClientProvider
{
    public static OpenAIResponseClient GetAIResponseClient(LlmOpenAiProviders provider,  string model)
    {
        OpenAIClient client;
        switch (provider)
        {
                
            case LlmOpenAiProviders.OpenAI:
                var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ??
                             throw new InvalidOperationException("Please set the OpenAI__ApiKey environment variable.");
                client = new OpenAIClient(new ApiKeyCredential(apiKey));

                break;
            case LlmOpenAiProviders.OpenRouter:
                 apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                 client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                });
                break;

            case LlmOpenAiProviders.AzureOpenAI:
            case LlmOpenAiProviders.A4F:
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }

        return client.GetOpenAIResponseClient(model);
    }
}