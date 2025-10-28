// ReSharper disable InconsistentNaming

using System.ClientModel;
using Anthropic.SDK;
using Azure.AI.OpenAI;
using Azure.Identity;
using GenerativeAI.Microsoft;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

namespace MicrosoftAgentFramework.Utilities;

public static class AIChatClient
{
    public static ChatClient GetOpenAI(OpenAI_LLM_Providers provider, string model)
    {
        switch (provider)
        {
            case OpenAI_LLM_Providers.AzureOpenAI:
            {
                const string endpoint = "https://nad-openai-azure.openai.azure.com/";
                var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());
                return client.GetChatClient(model);
            }
            case OpenAI_LLM_Providers.OpenAI:
            {
                var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ??
                             throw new InvalidOperationException("Please set the OpenAI__ApiKey environment variable.");
                var client = new OpenAIClient(new ApiKeyCredential(apiKey));

                return client.GetChatClient(model);
            }
            case OpenAI_LLM_Providers.OpenRouter:
            {
                var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                });
                return client.GetChatClient(model);
            }
            case OpenAI_LLM_Providers.A4F:
            {
                var apiKey = Environment.GetEnvironmentVariable("A4F_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://api.a4f.co/v1")
                });
                return client.GetChatClient(model);
            }
            default:
                throw new NotSupportedException($"The provider {provider} is not supported.");
        }
    }

    public static IChatClient GetNonOpenAI(NonOpenAiProviders provider, string model)
    {
        switch (provider)
        {
            case NonOpenAiProviders.Anthropic:
            {
                var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                var client = new AnthropicClient(apiKey).Messages.AsBuilder().Build();
                return client;
            }

            case NonOpenAiProviders.Gemini:
            {
                var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                             throw new InvalidOperationException(
                                 "Please set the OPENROUTER_API_KEY environment variable.");
                var client = new GenerativeAIChatClient(apiKey, model);
                return client;
            }
            default:
                throw new NotSupportedException($"The provider {provider} is not supported.");
        }
    }

    public static IEmbeddingGenerator<string, Embedding<float>> GetAzureOpenAiEmbeddingClient(string deploymentName)
    {
        const string endpoint = "https://nad-openai-azure.openai.azure.com/";
        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetEmbeddingClient(deploymentName)
            .AsIEmbeddingGenerator();
        return client;
    }
    
}

