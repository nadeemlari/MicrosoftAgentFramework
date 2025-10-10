using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Shared;
using Shared.Extensions;

const string apiKey = "...";
var azureOpenAiClient =
    new AzureOpenAIClient(new Uri("https://nad-openai-azure.openai.azure.com/"), new AzureCliCredential());
var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
AIAgent azureOpenAiAgent = azureOpenAiClient.GetChatClient("GPT-4.1-mini").CreateAIAgent();
AIAgent openAiAgent = openAiClient.GetChatClient("gpt-4.1-mini").CreateAIAgent();

var scenario = Scenario.Pdf;
switch (scenario)
{
    case Scenario.Text:
        var response =
            await azureOpenAiAgent.RunAsync(new ChatMessage(ChatRole.User, "What is the capital of France?"));
        ShowResponse(response);
        break;
    case Scenario.Pdf:
        var path = Path.Combine("SampleData", "catan_rules.pdf");
        var base64Pdf = Convert.ToBase64String(File.ReadAllBytes(path));
        var dataUri = $"data:application/pdf;base64,{base64Pdf}";
        response = await openAiAgent.RunAsync(new ChatMessage(ChatRole.User,
        [
            new TextContent("What is the winning condition in attached PDF"),
            new DataContent(dataUri, "application/pdf")
        ]));
        ShowResponse(response);
        ReadOnlyMemory<byte> data1 = File.ReadAllBytes(path).AsMemory();
        response = await openAiAgent.RunAsync(new ChatMessage(ChatRole.User,
        [
            new TextContent("What is the winning condition in attached PDF"),
            new DataContent(data1, "application/pdf"),
        ]));
        ShowResponse(response);
        break;
    case Scenario.Image:
        response = await azureOpenAiAgent.RunAsync(new ChatMessage(ChatRole.User,
        [
            new TextContent("What is in this image?"),
            new UriContent("https://upload.wikimedia.org/wikipedia/commons/7/70/A_game_of_Settlers_of_Catan.jpg",
                "image/jpeg")
        ]));
        ShowResponse(response);
        path = Path.Combine("SampleData", "image.jpg");

        //Image via Base64
        base64Pdf = Convert.ToBase64String(File.ReadAllBytes(path));
        dataUri = $"data:image/jpeg;base64,{base64Pdf}";
        response = await azureOpenAiAgent.RunAsync(new ChatMessage(ChatRole.User,
        [
            new TextContent("What is in this image?"),
            new DataContent(dataUri, "image/jpeg")
        ]));
        ShowResponse(response);
        ReadOnlyMemory<byte> data = File.ReadAllBytes(path).AsMemory();
        response = await azureOpenAiAgent.RunAsync(new ChatMessage(ChatRole.User,
        [
            new TextContent("What is in this image?"),
            new DataContent(data, "image/jpeg")
        ]));
        ShowResponse(response);


        break;
    default:
        throw new ArgumentOutOfRangeException();
}


return;


void ShowResponse(AgentRunResponse agentRunResponse)
{
    DisplayUtil.Separator();
    DisplayUtil.WriteLineYellow(agentRunResponse.Text);
    DisplayUtil.Separator();
    agentRunResponse.Usage.OutputAsInformation();
    DisplayUtil.Separator();
}

internal enum Scenario
{
    Text,
    Pdf,
    Image,
}