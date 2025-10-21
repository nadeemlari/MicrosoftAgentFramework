using Anthropic.SDK.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using Shared;

const string model = AnthropicModels.Claude3Haiku;
var client = AIChatClientProvider.GetNonOpenAIChatClient(LlmNonOpenAiProviders.Anthropic, model);
var chatClientAgentRunOptions = new ChatClientAgentRunOptions( new ChatOptions
{
    ModelId = model,
    MaxOutputTokens = 1000
});
var agent = new ChatClientAgent(client);
var response = await agent.RunAsync("What is the Capital of Australia?", options:chatClientAgentRunOptions);
DisplayUtil.WriteLineSuccess(response.Text);
DisplayUtil.Separator();
await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync("How to make soup?", options: chatClientAgentRunOptions))
{
    DisplayUtil.WriteYellow(update.Text);
}
