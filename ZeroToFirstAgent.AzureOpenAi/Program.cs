using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

const string model = "GPT-4.1-mini";
var client = AIChatClientProvider.GetOpenAIChatClient(LlmOpenAiProviders.AzureOpenAI, model);
var agent= client.CreateAIAgent();
var response = await agent.RunAsync("What is the Capital of France?");
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);
DisplayUtil.Separator();
await foreach (var update in agent.RunStreamingAsync("How to make soup?"))
{
    DisplayUtil.WriteYellow(update.Text);
}