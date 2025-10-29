using Microsoft.Agents.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared.Extensions;

const string model = "gpt-5-codex";
var agent = AIResponseClientProvider
    .GetAIResponseClient(OpenAI_LLM_Providers.OpenAI, model)
    .CreateAIAgent(instructions: "You are a C# Developer");
List<AgentRunResponseUpdate> updates = [];
var question = "Show me an C# Example of a method adding two numbers";
await foreach (var update in agent.RunStreamingAsync(question))
{
    updates.Add(update);
    Console.Write(update);
}

var fullResponse = updates.ToAgentRunResponse();
fullResponse.Usage.OutputAsInformation();