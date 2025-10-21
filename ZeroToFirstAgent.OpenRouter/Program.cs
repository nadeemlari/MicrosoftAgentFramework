using Microsoft.Agents.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

var agent = AIChatClientProvider.GetOpenAIChatClient(LlmOpenAiProviders.OpenRoute, "gpt-4.1-mini").CreateAIAgent();
var response = await agent.RunAsync("What is the capital of Germany?");
DisplayUtil.Separator();
DisplayUtil.WriteLineYellow(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate message in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteSuccess(message.Text);
}