using Microsoft.Extensions.VectorData;

namespace RagUtils;

public class PdfChunk
{
    private const int VectorDimensions = 1536; 
    
    [VectorStoreKey]
    public required Guid Key { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public required string DocumentId { get; set; }

    [VectorStoreData]
    public int PageNumber { get; set; }

    [VectorStoreData]
    public required string Text { get; set; }

    [VectorStoreVector(VectorDimensions, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> VectorData { get; set; }
}