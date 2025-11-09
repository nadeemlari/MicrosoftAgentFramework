namespace Crawler.Web;

public static class TextChunker
{
    public static List<string> ChunkText(string text, int chunkSize = 1000, int overlap = 200)
    {
        var chunks = new List<string>();
        var start = 0;
        
        while (start < text.Length)
        {
            var end = Math.Min(start + chunkSize, text.Length);
            var chunk = text.Substring(start, end - start);
            
            chunks.Add(chunk.Trim());
            start += chunkSize - overlap;
        }
        
        return chunks;
    }
}