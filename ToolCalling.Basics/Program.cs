using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;
using ToolCalling.Basics;

// const string model = "openai/gpt-4.1-mini";
const string model = "deepseek/deepseek-v3.2-exp";
var client = AIChatClient.GetOpenAI(OpenAI_LLM_Providers.OpenRouter,model);
var agent = client.CreateAIAgent(
    instructions: "YYou are a Time Expert",
    tools: [AIFunctionFactory.Create(Tools.CurrentDataAndTime), AIFunctionFactory.Create(Tools.CurrentTimezone)]
);
var thread = agent.GetNewThread();
while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
        break;

    var message = new ChatMessage(ChatRole.User, userInput);
    DisplayUtil.LoadingTask();
    var response = await agent.RunAsync(message, thread);
    DisplayUtil.StopLoading();
    DisplayUtil.Separator();
    DisplayUtil.WriteLineSuccess($"Agent: {response.Text}");
}