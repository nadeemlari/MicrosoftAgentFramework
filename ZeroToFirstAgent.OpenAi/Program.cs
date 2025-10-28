using Microsoft.Agents.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

const string model = "gpt-4.1-mini";
var client = AIChatClient.GetOpenAI(OpenAI_LLM_Providers.OpenAI, model);
var agent = client.CreateAIAgent();
var response = await agent.RunAsync("What is the capital of Germany?");
DisplayUtil.Separator();
DisplayUtil.WriteLineYellow(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate message in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteSuccess(message.Text);
}