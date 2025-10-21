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

public static class AIChatClientProvider
{
    public static ChatClient GetOpenAIChatClient(LlmOpenAiProviders provider, string model)
    {
        switch(provider)
        {
            case LlmOpenAiProviders.AzureOpenAI:
                {
                    const string endpoint = "https://nad-openai-azure.openai.azure.com/";
                    var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());
                    return client.GetChatClient(model);
                }
            case LlmOpenAiProviders.OpenAI:
                {
                    var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ?? throw new InvalidOperationException("Please set the OpenAI__ApiKey environment variable.");
                    var client = new OpenAIClient(new ApiKeyCredential(apiKey));
                    
                    return client.GetChatClient(model);
                }
            case LlmOpenAiProviders.OpenRouter:
            {
                var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? throw new InvalidOperationException("Please set the OPENROUTER_API_KEY environment variable.");
                var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                });
                return client.GetChatClient(model);
            }
            default:
                throw new NotSupportedException($"The provider {provider} is not supported.");
        }
    }
    public static IChatClient GetNonOpenAIChatClient(LlmNonOpenAiProviders provider, string model)
    {
        switch(provider)
        {
            case LlmNonOpenAiProviders.Anthropic:
            {
                var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? throw new InvalidOperationException("Please set the OPENROUTER_API_KEY environment variable.");
                var client = new AnthropicClient(apiKey).Messages.AsBuilder().Build();
                return client;
            }

            case LlmNonOpenAiProviders.Gemini:
            {
                var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? throw new InvalidOperationException("Please set the OPENROUTER_API_KEY environment variable.");
                var client = new GenerativeAIChatClient(apiKey, model);
                return client;
            }
            default:
                throw new NotSupportedException($"The provider {provider} is not supported.");
        }
    }
    
}