using System.ClientModel;
using Microsoft.Extensions.AI;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using OpenAI.Containers;
using OpenAI.Responses;
using Shared;
#pragma warning disable OPENAI001

// const string model = "openai/gpt-4.1";
const string model = "gpt-4.1";


// var client = AIResponseClientProvider.GetAIResponseClient(OpenAI_LLM_Providers.OpenRouter, model);
var client = OpenAIClientProvider.GetOpenAIClient(OpenAI_LLM_Providers.OpenAI);

var agent = client.GetOpenAIResponseClient(model)
    .CreateAIAgent(tools: [new HostedWebSearchTool()]
    );
var question = "Find Top 10 Countries in the world and make a Bar chart should each countries population in millions";
DisplayUtil.LoadingTask();
var response = await agent.RunAsync(question);
DisplayUtil.StopLoading();
foreach (var message in response.Messages)
{
    foreach (var content in message.Contents)
    {
        foreach (var annotation in content.Annotations ?? [])
        {
            if (annotation is CitationAnnotation)
            {
                Console.WriteLine("The intended way to get file, but not working at the moment due to a bug in the OpenAI SDK");
            }
        }
        if (content.RawRepresentation is CodeInterpreterCallResponseItem codeInterpreterCallResponse)
        {
            DisplayUtil.WriteLineGreen("The Code");
            DisplayUtil.WriteLineDarkGray(codeInterpreterCallResponse.Code);
            DisplayUtil.WriteLineGreen("The File");
            var containerClient = client.GetContainerClient();
            var containerId = codeInterpreterCallResponse.ContainerId;
            CollectionResult<ContainerFileResource> containerFileResources = containerClient.GetContainerFiles(containerId);
            foreach (var fileResource in containerFileResources)
            {
                ClientResult<BinaryData> fileContent = await containerClient.GetContainerFileContentAsync(containerId, fileResource.Id);
                var path = Path.Combine(Path.GetTempPath(), fileResource.Path.Replace("/", "_"));
                await File.WriteAllBytesAsync(path, fileContent.Value.ToArray());
                await Task.Factory.StartNew(() =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                });
            }
        }
    }
}
