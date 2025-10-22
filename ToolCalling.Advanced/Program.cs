using System.Reflection;
using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;
using ToolCalling.Advanced.Tools;

#pragma warning disable MEAI001

// const string model = "openai/gpt-4.1-mini";
const string model = "deepseek/deepseek-v3.2-exp";
var client = AIChatClientProvider.GetOpenAIChatClient(LlmOpenAiProviders.OpenRouter, model);

var target = new FileSystemTools();
var tools = typeof(FileSystemTools).GetMethods(BindingFlags.Public | BindingFlags.Instance)
    .Where(m => m.DeclaringType == typeof(FileSystemTools))
    .Select(m => AIFunctionFactory.Create(m, target)).Cast<AITool>()
    .ToList();

tools.Add(new ApprovalRequiredAIFunction(AIFunctionFactory.Create(DangerousTools.SomethingDangerous)));

var agent = client.CreateAIAgent(
        instructions:
        "You are a File Expert. When working with files you need to provide the full path; not just the filename",
        tools: tools
    )
    .AsBuilder()
    .Use(FunctionCallMiddleware)
    .Build();

var thread = agent.GetNewThread();
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    var message = new ChatMessage(ChatRole.User, input);
    var response = await agent.RunAsync(message, thread);
    var userInputRequests = response.UserInputRequests.ToList();
    while (userInputRequests.Count > 0)
    {
        var userInputResponses = userInputRequests
            .OfType<FunctionApprovalRequestContent>()
            .Select(functionApprovalRequest =>
            {
                Console.WriteLine($"The agent would like to invoke the following function, please reply Y to approve: Name {functionApprovalRequest.FunctionCall.Name}");
                return new ChatMessage(ChatRole.User, [functionApprovalRequest.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);
            })
            .ToList();

        response = await agent.RunAsync(userInputResponses, thread);
        userInputRequests = response.UserInputRequests.ToList();
    }

    Console.WriteLine(response);

    DisplayUtil.Separator();
}
return;


async ValueTask<object?> FunctionCallMiddleware(AIAgent callingAgent, FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
{
    StringBuilder functionCallDetails = new();
    functionCallDetails.Append($"- Tool Call: '{context.Function.Name}'");
    if (context.Arguments.Count > 0)
    {
        functionCallDetails.Append(
            $" (Args: {string.Join(",", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
    }

    DisplayUtil.WriteLineInformation(functionCallDetails.ToString());

    return await next(context, cancellationToken);
}