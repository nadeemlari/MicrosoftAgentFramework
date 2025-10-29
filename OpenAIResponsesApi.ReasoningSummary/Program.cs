using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using OpenAI.Responses;
using Shared;

#pragma warning disable OPENAI001

// const string model = "openai/gpt-5.1-mini";
const string model = "gpt-5.1-mini";
var client = AIChatClient.GetOpenAI(OpenAI_LLM_Providers.AzureOpenAI, model);
var agent = client.CreateAIAgent(new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions
    {
        RawRepresentationFactory = _ => new ResponseCreationOptions
        {
            ReasoningOptions = new ResponseReasoningOptions
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.Medium,
                ReasoningSummaryVerbosity = ResponseReasoningSummaryVerbosity.Detailed
            }
        }
    }
});
DisplayUtil.LoadingTask();
var response = await agent.RunAsync("What is the capital of france and how many live there?");
DisplayUtil.StopLoading();
foreach (var message in response.Messages)
{
    foreach (var content in message.Contents)
    {
        if (content is TextReasoningContent textReasoningContent)
        {
            DisplayUtil.WriteLineGreen("The Reasoning");
            DisplayUtil.WriteLineDarkGray(textReasoningContent.Text);
        }
    }
}
DisplayUtil.WriteLineGreen("The Answer");
Console.WriteLine(response.Text);