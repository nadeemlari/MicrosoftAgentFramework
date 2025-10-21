using System.ClientModel;
using Microsoft.Agents.AI;
using OpenAI;
using Shared;

string apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? throw new InvalidOperationException("Please set the OPENROUTER_API_KEY environment variable.");
const string model = "gpt-4.1-mini";
var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
{
    Endpoint = new Uri("https://openrouter.ai/api/v1")
});

var agent = client.GetChatClient(model).CreateAIAgent();
var response = await agent.RunAsync("What is the capital of Germany?");
DisplayUtil.Separator();
DisplayUtil.WriteLineYellow(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate message in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteSuccess(message.Text);
}