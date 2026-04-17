// ReSharper disable InconsistentNaming

using System.ClientModel;
using OpenAI;
using OpenAI.Responses;

#pragma warning disable OPENAI001

namespace Utilities;

public static class AIResponseClientProvider
{
    public static OpenAIResponseClient GetAIResponseClient(OpenAI_LLM_Providers provider,  string model)
    {
        OpenAIClient client;
        switch (provider)
        {
                
            case OpenAI_LLM_Providers.OpenAI:
                var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ??
                             throw new InvalidOperationException("Please set the OpenAI__ApiKey environment variable.");
                client = new OpenAIClient(new ApiKeyCredential(apiKey));

                break;
            case OpenAI_LLM_Providers.OpenRouter:
                 apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                 client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                });
                break;

            case OpenAI_LLM_Providers.AzureOpenAI:
            case OpenAI_LLM_Providers.A4F:
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }

        return client.GetOpenAIResponseClient(model);
    }
}