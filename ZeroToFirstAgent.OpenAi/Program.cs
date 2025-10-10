using Microsoft.Agents.AI;
using OpenAI;
using Shared;

const string apiKey = "....";
const string model = "gpt-4.1-mini";
var client = new OpenAIClient(apiKey);
var agent = client.GetChatClient(model).CreateAIAgent();
var response = await agent.RunAsync("What is the capital of Germany?");
DisplayUtil.Separator();
DisplayUtil.WriteLineYellow(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate message in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteSuccess(message.Text);
}