using System.Text.Json;
using Anthropic.SDK.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using UseToonToSaveTokens;

const string model = AnthropicModels.Claude3Haiku;
var client = AIChatClientProvider.GetNonOpenAI(NonOpenAiProviders.Anthropic, model);
var chatClientAgentRunOptions = new ChatClientAgentRunOptions( new ChatOptions
{
    ModelId = model,
    MaxOutputTokens = 1000
});

var json = await File.ReadAllTextAsync("famous_people.json");
var list = JsonSerializer.Deserialize<List<FamousPerson>>(json)!;
const string instructions = "You answer questions about famous people. Always use tool 'get_famous_people' to get data";
const string question = "Tell me about Nadeem";

var agentWithJsonTool = client.CreateAIAgent(
    instructions:instructions,
    tools: [AIFunctionFactory.Create(GetFamousPeopleAsJson, name: "get_famous_people")]);

var agentWithToonTool = client.CreateAIAgent(
    instructions:instructions,
    tools: [AIFunctionFactory.Create(GetFamousPeopleAsToon, name: "get_famous_people")]);

DisplayUtil.WriteLineInformation("=== Ask using JSON Tool ===");
var res1 = await agentWithJsonTool.RunAsync(question, options: chatClientAgentRunOptions);
DisplayUtil.WriteLineSuccess(res1.Text);
res1.Usage.OutputAsInformation();
DisplayUtil.Separator();

DisplayUtil.WriteLineInformation("=== Ask using Toon Tool ===");
var res2 = await agentWithToonTool.RunAsync(question, options: chatClientAgentRunOptions);
DisplayUtil.WriteLineSuccess(res2.Text);
res2.Usage.OutputAsInformation();

return;

List<FamousPerson> GetFamousPeopleAsJson()
{
    return list;
}

string GetFamousPeopleAsToon()
{
    var toon = ToonNetSerializer.ToonNet.Encode(list);
    return toon;
}
    