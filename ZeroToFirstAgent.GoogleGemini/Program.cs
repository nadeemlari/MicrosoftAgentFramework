using GenerativeAI;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Shared;


const string apiKey = "....";
const string model = GoogleAIModels.Gemini2Flash;        
var client = new GenerativeAIChatClient(apiKey, model);
var agent = new ChatClientAgent(client);
var response = await agent.RunAsync("What is the Capital of Australia?");
ConsoleUtils.Separator();
ConsoleUtils.WriteLineSuccess(response.Text);
ConsoleUtils.Separator();
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync("How to make soup?"))
{
    ConsoleUtils.WriteYellow(update.Text);
}