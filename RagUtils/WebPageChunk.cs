using Microsoft.Extensions.VectorData;

namespace RagUtils;

public class WebPageChunk
{
    private const int VectorDimensions = 1536; 
   
    
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