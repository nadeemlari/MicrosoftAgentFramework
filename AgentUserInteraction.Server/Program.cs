using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using MicrosoftAgentFramework.Utilities;

Console.Clear();

const string model = "openai/gpt-4.1-mini";

// Initialize AI chat client with error handling
var chatClient = AIChatClientProvider.GetOpenAI(OpenAI_LLM_Providers.OpenRouter, model)
    ?? throw new InvalidOperationException("Failed to initialize AIChatClient.");

// Create agent with weather tool
var agent = chatClient.AsIChatClient().CreateAIAgent(
    tools: [AIFunctionFactory.Create(GetWeather, name: "get_weather")]
);

var builder = WebApplication.CreateBuilder(args);

// Register AGUI services
builder.Services.AddAGUI();

var app = builder.Build();

// Map AGUI endpoint
app.MapAGUI("/", agent);

await app.RunAsync();
return;


// Weather function for the agent
static string GetWeather(string city)
    => $"It is sunny and 19 degrees in {city}";