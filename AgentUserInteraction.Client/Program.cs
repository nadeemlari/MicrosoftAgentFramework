using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

Console.Clear();
var httpClient = new HttpClient();
const string serverUrl = "http://localhost:5000";
var textColor = ConsoleColor.White;
var chatClient = new AGUIChatClient(httpClient, serverUrl);
var agent = chatClient.CreateAIAgent(tools: [AIFunctionFactory.Create(ChangeColor, name: "change_color")]);
List<ChatMessage> messages = [new ChatMessage(ChatRole.System, "You are a nice AI Agent")];
while (true)
{
    Console.Write("User > ");
    var message = Console.ReadLine() ?? string.Empty;
    if (message == string.Empty)
    {
        continue;
    }
    messages.Add(new ChatMessage(ChatRole.User, message));
    List<AgentRunResponseUpdate> updates = [];
    await foreach (var update in agent.RunStreamingAsync(messages))
    {
        updates.Add(update);
        foreach (var content in update.Contents)
        {
            switch (content)
            {
                case TextContent textContent:
                    Console.ForegroundColor = textColor;
                    Console.Write(textContent.Text);
                    break;
                case FunctionCallContent functionCallContent:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    StringBuilder toolCallDetails = new();
                    toolCallDetails.Append($"[Tool Call: {functionCallContent.Name}");
                    if (functionCallContent.Arguments.Any())
                    {
                        toolCallDetails.Append($" (Args: {string.Join(",", functionCallContent.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
                    }

                    toolCallDetails.Append("]");
                    Console.WriteLine(toolCallDetails);
                    Console.ForegroundColor = textColor;
                    break;
                case FunctionResultContent functionResultContent:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    var isError = functionResultContent.Exception != null;
                    Console.WriteLine(isError ? $"[Tool Error: {functionResultContent.Exception}]" : $"[Tool Result: {functionResultContent.Result}]");

                    Console.ForegroundColor = textColor;
                    break;
                case ErrorContent errorContent:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[Error: {errorContent.Message}]");
                    Console.ForegroundColor = textColor;
                    break;
            }
        }
        
    }
}

return;
void ChangeColor(ConsoleColor color)
{
    textColor = color;
    Console.ForegroundColor = textColor;
}