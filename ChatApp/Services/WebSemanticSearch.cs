using Microsoft.Extensions.VectorData;
using RagUtils;

namespace ChatApp.Services;

public class WebSemanticSearch(VectorStoreCollection<ulong, WebPageChunk> vectorCollection)
{
    public async Task<IReadOnlyList<WebPageChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var nearest = vectorCollection.SearchAsync(text, maxResults );
        var results = new List<WebPageChunk>();
        await foreach (var r in nearest)
        {
            results.Add(r.Record);
        }
        return results.AsReadOnly();
    }
}