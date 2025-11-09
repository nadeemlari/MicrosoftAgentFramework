using Microsoft.Extensions.VectorData;

namespace Crawler.Web;

public class WebPageChunk
{
    private const int VectorDimensions = 1536; // 1536 is the default vector size for the OpenAI text-embedding-3-small model
   
    
    [VectorStoreKey]
    public required Guid Id { get; set; }
    
    [VectorStoreData]
    public required string Url { get; set; }
    
    [VectorStoreData]
    public required string Content { get; set; }
    
    [VectorStoreData]
    public required string Title { get; set; }
    
    [VectorStoreData]
    public DateTimeOffset CrawledAt { get; set; }
    
    [VectorStoreVector(VectorDimensions, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> ContentEmbedding { get; set; }
}