using GenerativeAI;
using Microsoft.Agents.AI;
using MicrosoftAgentFramework.Utilities;
using Shared;


const string model = GoogleAIModels.Gemini2Flash;        
var client = AIChatClientProvider.GetNonOpenAIChatClient( LlmNonOpenAiProviders.Gemini, model);
var agent = new ChatClientAgent(client);
var response = await agent.RunAsync("What is the Capital of Australia?");
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteYellow(update.Text);
}