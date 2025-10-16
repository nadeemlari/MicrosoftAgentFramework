using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Shared;
using StructuredOutput.Models;

var client = new AzureOpenAIClient(new Uri("https://nad-openai-azure.openai.azure.com/"), new AzureCliCredential());
var question = "What are the top 10 Movies according to IMDB?";

//Without Structured Output
var agent1 = client
    .GetChatClient("GPT-4.1-mini")
    .CreateAIAgent(instructions: "You are an expert in IMDB Lists");

var response1 = await agent1.RunAsync(question);
DisplayUtil.Separator();
DisplayUtil.WriteLineInformation(response1.Text);

//With Structured Output
var agent2 = client 
    .GetChatClient("GPT-4.1-mini")
    .CreateAIAgent(instructions: "You are an expert in IMDB Lists");

var response2 = await agent2.RunAsync<MovieResult>(question);
var movieResult2 = response2.Result;
DisplayUtil.Separator();
DisplayMovies(movieResult2);

//More cumbersome but sometimes needed way
var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
    Converters = { new JsonStringEnumConverter() }
};

var agent3 = client
    .GetChatClient("GPT-4.1-mini")
    .CreateAIAgent(instructions: "You are an expert in IMDB Lists");

var chatResponseFormatJson = ChatResponseFormat.ForJsonSchema<MovieResult>(jsonSerializerOptions);

var response3 = await agent3.RunAsync(question, options: new ChatClientAgentRunOptions()
{
    ChatOptions = new ChatOptions
    {
        ResponseFormat = chatResponseFormatJson
    }
});

var movieResult3 = response3.Deserialize<MovieResult>(jsonSerializerOptions);
DisplayUtil.Separator();
DisplayMovies(movieResult3);
return;


void DisplayMovies(MovieResult movieResult)
{
    var counter = 1;
    Console.WriteLine(movieResult!.MessageBack);
    foreach (var movie in movieResult.Top10Movies)
    {
        DisplayUtil.WriteLineSuccess($"{counter}: {movie.Title} ({movie.YearOfRelease}) - Genre: {movie.Genre} - Director: {movie.Director} - IMDB Score: {movie.ImdbScore}");
        counter++;
    }
}