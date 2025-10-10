using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Shared;

const string apiKey = "...";
const string model = AnthropicModels.Claude3Haiku;
var client = new AnthropicClient(apiKey).Messages.AsBuilder().Build();
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
