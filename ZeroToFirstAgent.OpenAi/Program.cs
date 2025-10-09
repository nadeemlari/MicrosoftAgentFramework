using Microsoft.Agents.AI;
using OpenAI;
using Shared;

const string apiKey = "....";
const string model = "gpt-4.1-mini";
var client = new OpenAIClient(apiKey);
var agent = client.GetChatClient(model).CreateAIAgent();
var response = await agent.RunAsync("What is the capital of Germany?");
ConsoleUtils.Separator();
ConsoleUtils.WriteLineYellow(response.Text);
ConsoleUtils.Separator();
await foreach (AgentRunResponseUpdate message in agent.RunStreamingAsync("How to make soup?"))
{
    ConsoleUtils.WriteSuccess(message.Text);
}