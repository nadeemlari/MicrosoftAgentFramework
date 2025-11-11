using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using MicrosoftAgentFramework.Utilities;

Console.Clear();
var chatClient = AIChatClient.GetNonOpenAI(NonOpenAiProviders.Anthropic, "claude-haiku-4-5-20251001");
var agent = chatClient.CreateAIAgent(tools: [AIFunctionFactory.Create(GetWeather, name: "get_weather")]);
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAGUI();
var app = builder.Build();
app.MapAGUI("/", agent);
await app.RunAsync();
return;
static string GetWeather(string city)
{
    return "It is sunny and 19 degrees";
}