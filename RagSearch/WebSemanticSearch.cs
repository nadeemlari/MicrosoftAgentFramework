using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using RagUtils;

#pragma warning disable CS0618 // Type or member is obsolete

namespace RagSearch;

public class WebSemanticSearch(string openAiApiKey)
{
    private readonly ITextEmbeddingGenerationService _embeddingService = new OpenAITextEmbeddingGenerationService(
        modelId: "text-embedding-3-small",
        apiKey: openAiApiKey
    );

    private readonly VectorStore _vectorStore =
        new QdrantVectorStore(new QdrantClient("qdrant.pub.nadeemlari.in"), ownsClient: true);

    public async Task<List<(WebPageChunk Record, double? Score)>> SearchAsync(
        string query,
        int topK = 5)
    {
        var collection = _vectorStore.GetCollection<ulong, WebPageChunk>("solenis_web_content");

        // Generate embedding for the search query
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);

        // Perform vector search
        var searchResults = collection.SearchAsync(queryEmbedding, 
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