using Microsoft.Extensions.VectorData;
using RagUtils;

namespace ChatApp.Services;

public class PdfSemanticSearch(
    VectorStoreCollection<ulong, PdfChunk> vectorCollection)
{
    public async Task<IReadOnlyList<PdfChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<PdfChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });
        var results = new List<PdfChunk>();
        await foreach (var r in nearest)
        {
            results.Add(r.Record);
        }
        return results.AsReadOnly();


    }
}
