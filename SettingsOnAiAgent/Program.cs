using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using OpenAI.Chat;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Shared;

#pragma warning disable OPENAI001

const string model = "deepseek/deepseek-v3.2-exp"; 
var client = AIChatClientProvider.GetOpenAIChatClient(LlmOpenAiProviders.OpenRouter, model);
var noSettingAgent = client.CreateAIAgent();
var agent = client.CreateAIAgent(
    instructions: "You are a cool surfer dude",
    tools: []
);

var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton(new MySpecialService());
var serviceProvider = builder.Services.BuildServiceProvider();
var sourceName = Guid.NewGuid().ToString("N");
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(sourceName)
    .AddConsoleExporter()
    .Build();

var agentWithSettings = client.CreateAIAgent(
        instructions: "Speak like a Pirate",
        name: "My Agent",
        description: "This is a test agent",
        tools: [],
        clientFactory: chatClient =>
        {
            return new ConfigureOptionsChatClient(chatClient, options =>
            {
                options.RawRepresentationFactory = _ => new ChatCompletionOptions()
                {
                    // ReasoningEffortLevel = ChatReasoningEffortLevel.Low,
                };
            });
        },
        loggerFactory: LoggerFactory.Create(lb => lb.AddConsole().SetMinimumLevel(LogLevel.Debug)),
        services: serviceProvider
    )
    .AsBuilder()
    .UseOpenTelemetry(sourceName)
    .Build();

DisplayUtil.LoadingTask();
var response = await agentWithSettings.RunAsync("What is the Capital of France?");
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);
DisplayUtil.StopLoading();
public class MySpecialService
{
}