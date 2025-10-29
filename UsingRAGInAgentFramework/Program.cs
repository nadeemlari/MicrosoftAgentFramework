using System.Text.Json;
using Microsoft.SemanticKernel.Connectors.InMemory;
using MicrosoftAgentFramework.Utilities;
using Shared;
using UsingRAGInAgentFramework.Models;

var jsonWithMovies = await File.ReadAllTextAsync("made_up_movies.json");
var movies = JsonSerializer.Deserialize<List<Movie>>(jsonWithMovies);
DisplayUtil.LoadingTask();
var client = AIChatClient.GetAzureOpenAiEmbeddingClient("text-embedding-ada-002");
var store = new InMemoryVectorStore(new InMemoryVectorStoreOptions
{
  EmbeddingGenerator = client
});

var collection = store.GetCollection<Guid, MovieVectorStoreRecord>("movies");
await collection.EnsureCollectionExistsAsync();
var counter = 0;
if (movies != null)
  foreach (var movie in movies)
  {
    counter++;
    Console.Write($"\rEmbedding Movies: {counter}/{movies.Count}");
    await collection.UpsertAsync(new MovieVectorStoreRecord
    {
      Id = Guid.NewGuid(),
      Title = movie.Title,
      Plot = movie.Plot,
      Rating = movie.Rating
    });
  }

DisplayUtil.StopLoading();
