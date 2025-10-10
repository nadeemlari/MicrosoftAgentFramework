using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Shared;

#pragma warning disable OPENAI001

const string endpoint = "https://nad-openai-azure.openai.azure.com/";
const string model = "GPT-4.1-mini";
var client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());
var noSettingAgent = client.GetChatClient(model).CreateAIAgent();
var agent = client.GetChatClient(model).CreateAIAgent(
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

var agentWithSettings = client.GetChatClient(model).CreateAIAgent(
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

var response = await agentWithSettings.RunAsync("What is the Capital of France?");
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);

public class MySpecialService
{
}