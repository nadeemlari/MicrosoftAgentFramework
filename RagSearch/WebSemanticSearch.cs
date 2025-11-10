using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using MicrosoftAgentFramework.Utilities;
using OpenAI.Embeddings;
using Qdrant.Client;
using RagUtils;

namespace RagSearch;

public class WebSemanticSearch
{
    
    private  readonly EmbeddingClient _embeddingClient = OpenAIClientProvider.GetOpenAIClient(OpenAI_LLM_Providers.OpenAI).GetEmbeddingClient("text-embedding-3-small");
    
    private readonly VectorStore _vectorStore =
        new QdrantVectorStore(new QdrantClient("qdrant.pub.nadeemlari.in"), ownsClient: true);

    public async Task<List<(WebPageChunk Record, double? Score)>> SearchAsync(
        string query,
        int topK = 5)
    {
        var collection = _vectorStore.GetCollection<ulong, WebPageChunk>("solenis_web_content");
        var x = await _embeddingClient.GenerateEmbeddingAsync(query);
        
        var searchResults = collection.SearchAsync(x.Value.ToFloats(), 
            topK
           );

        var results = new List<(WebPageChunk, double?)>();
        await foreach (var result in searchResults)
        {
            results.Add((result.Record, result.Score));
        }

        return results;
    }
}