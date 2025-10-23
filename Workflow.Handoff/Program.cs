using Microsoft.Agents.AI.Workflows;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

// const string model = "provider-3/gpt-4o-mini";
const string model = "openai/gpt-4o-mini";
// const string model = "provider-3/gpt-4o-mini";
var client = AIChatClientProvider.GetOpenAIChatClient(LlmOpenAiProviders.OpenRouter, model);

var intentAgent = client.CreateAIAgent(
    name: "IntentAgent",
    instructions: "Determine what type of question was asked. Never answer yourself"
);
var movieNerd = client.CreateAIAgent(
    name: "MovieNerd",
    instructions: "You are a Movie Nerd"
);
var musicNerd = client.CreateAIAgent(
    name: "MusicNerd",
    instructions: "You are a Music Nerd"
);
var workflowAgent = await AgentWorkflowBuilder.CreateHandoffBuilderWith(intentAgent)
    .WithHandoffs(intentAgent, [movieNerd, musicNerd])
    .WithHandoffs([movieNerd, musicNerd], intentAgent)
    .Build().AsAgentAsync();
    
Console.Write("> ");
var userInput = Console.ReadLine();
DisplayUtil.LoadingTask();
var response = await workflowAgent.RunAsync(userInput ?? string.Empty);
DisplayUtil.StopLoading();
DisplayUtil.Separator();
DisplayUtil.WriteLineInformation("Final Response:");
DisplayUtil.WriteLineSuccess(response.Text);
foreach (var m in response.Messages)
{
    DisplayUtil.Separator();
    DisplayUtil.WriteLineInformation($"{m.AuthorName} Response:");
    DisplayUtil.WriteLineYellow(m.Text);
}