using System.ComponentModel;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

// const string model = "openai/gpt-4.1-mini";
const string model = "deepseek/deepseek-v3.2-exp";

var client = AIChatClient.GetOpenAI(OpenAI_LLM_Providers.OpenRouter, model);
var writerAgent  = client.CreateAIAgent(
        instructions: "Write stories that are engaging and creative.",
        name: "Writer Agent",
        description: "An agent that writes stories.",
        tools: [AIFunctionFactory.Create(GetAuthor), AIFunctionFactory.Create(FormatStory)]
        
    );

var editorAgent  = client.CreateAIAgent(
    instructions: "Make the story more engaging , fix grammar , and enhance the plot.",
    name: "Editor Agent",
    description: "An agent that edits  stories."
);

var workflowAgent = await AgentWorkflowBuilder
    .BuildSequential(writerAgent, editorAgent)
    .AsAgentAsync();

DisplayUtil.LoadingTask();

var response = await workflowAgent.RunAsync("Write a short story about a hunted house in the mountains.");
DisplayUtil.StopLoading();
DisplayUtil.Separator();
DisplayUtil.WriteLineSuccess(response.Text);
return;

[Description("Get the author of story")]
string GetAuthor() => "Mohammad Nadeem";

[Description("Format the story for Display")]
string FormatStory(string title, string author, string content) => $"Title: {title}\nAuthor: {author}\n\n{content}";



