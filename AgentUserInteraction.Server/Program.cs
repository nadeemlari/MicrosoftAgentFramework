using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using MicrosoftAgentFramework.Utilities;

Console.Clear();
const string model = "openai/gpt-4.1-mini";
var chatClient = AIChatClient.GetOpenAI(OpenAI_LLM_Providers.OpenRouter, model);
var agent = chatClient.AsIChatClient().CreateAIAgent(tools: [AIFunctionFactory.Create(GetWeather, name: "get_weather")]);
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAGUI();
var app = builder.Build();
app.MapAGUI("/", agent);
await app.RunAsync();
return;
static string GetWeather(string city)
{
    return $"It is sunny and 19 degrees in {city}";
}