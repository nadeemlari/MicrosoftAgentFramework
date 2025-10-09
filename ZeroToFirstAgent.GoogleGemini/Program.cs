using GenerativeAI;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Shared;


const string apiKey = "....";
const string model = GoogleAIModels.Gemini2Flash;        
var client = new GenerativeAIChatClient(apiKey, model);
var agent = new ChatClientAgent(client);
var response = await agent.RunAsync("What is the Capital of Australia?");
Utils.Separator();
Utils.WriteLineSuccess(response.Text);
Utils.Separator();
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync("How to make soup?"))
{
    Utils.WriteYellow(update.Text);
}