using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared.Extensions;

const string model = "openai/gpt-4.1";
// const string model = "gpt-4.1";
var resClient = AIResponseClientProvider.GetAIResponseClient(OpenAI_LLM_Providers.OpenRouter, model);

var agent = resClient.CreateAIAgent(
    instructions: "You are a Space News AI Reporter",
    tools: [new HostedWebSearchTool()]
);

var question = "What is today's news in Space Exploration (List today's date at the top)";
List<AgentRunResponseUpdate> updates = [];
await foreach (var update in agent.RunStreamingAsync(question))
{
    updates.Add(update);
    Console.Write(update);
}
var fullResponse = updates.ToAgentRunResponse();
fullResponse.Usage.OutputAsInformation();