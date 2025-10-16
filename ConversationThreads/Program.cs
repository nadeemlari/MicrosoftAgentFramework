using Azure.AI.OpenAI;
using Azure.Identity;
using ConversationThreads;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.VisualBasic.CompilerServices;
using OpenAI;
using Shared;

var client = new AzureOpenAIClient(new Uri("https://nad-openai-azure.openai.azure.com/"), new AzureCliCredential());
var agent= client.GetChatClient("GPT-4.1-mini").CreateAIAgent("You are a Friendly AI Bot, answering questions");

var thread = agent.GetNewThread();
const bool optionToResume = true;
if (optionToResume)
{
    thread = await AgentThreadPersistence.ResumeChatIfRequestedAsync(agent);
}

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(input))
    {
        var message = new ChatMessage(ChatRole.User, input);
        await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(message, thread))
        {
            Console.Write(update);
        }
    }

    DisplayUtil.Separator();

    if (optionToResume)
    {
        await AgentThreadPersistence.StoreThreadAsync(thread);
    }
}