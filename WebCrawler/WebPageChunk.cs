using Microsoft.Extensions.VectorData;

namespace WebCrawler;

public class WebPageChunk
{
    private const int VectorDimensions = 1536; // 1536 is the default vector size for the OpenAI text-embedding-3-small model
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;
    
    [VectorStoreKey]
    public required Guid Id { get; set; }
    
    [VectorStoreData]
    public required string Url { get; set; }
    
    [VectorStoreData]
    public required string Content { get; set; }
    
    [VectorStoreData]
    public required string Title { get; set; }
    
    [VectorStoreData]
    public DateTime CrawledAt { get; set; }
    
    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    public ReadOnlyMemory<float> ContentEmbedding { get; set; }
}