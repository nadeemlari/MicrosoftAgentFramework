
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;
using Shared;

const string endpoint = "https://nad-openai-azure.openai.azure.com/";
const string model = "GPT-4.1-mini";
var client = new AzureOpenAIClient(new Uri(endpoint),new AzureCliCredential());
var agent= client.GetChatClient(model).CreateAIAgent();
var response = await agent.RunAsync("What is the Capital of France?");
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteYellow(update.Text);
}