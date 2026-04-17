using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Utilities;
using OpenAI.Embeddings;
using Qdrant.Client;
using RagUtils;

namespace RagSearch;

public class PdfSemanticSearch
{
    
    private  readonly EmbeddingClient _embeddingClient = OpenAIClientProvider.GetOpenAIClient(OpenAI_LLM_Providers.OpenAI).GetEmbeddingClient("text-embedding-3-small");
    
    private readonly VectorStore _vectorStore =
        new QdrantVectorStore(new QdrantClient("qdrant.pub.nadeemlari.in"), ownsClient: true);

    public async Task<List<(PdfChunk record, double? Score)>> SearchAsync(
        string query,
        int topK = 5)
    {
        var collection = _vectorStore.GetCollection<ulong, PdfChunk>("solenis_pdf_chunk_content");
        var x = await _embeddingClient.GenerateEmbeddingAsync(query);
        
        var searchResults = collection.SearchAsync(x.Value.ToFloats(), 
            topK
        );

        var results = new List<(PdfChunk, double?)>();
        await foreach (var result in searchResults)
        {
            results.Add((result.Record, result.Score));
        }

        return results;
    }
}