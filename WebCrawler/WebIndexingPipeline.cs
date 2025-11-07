using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Shared;


#pragma warning disable SKEXP0050
#pragma warning disable CS0618 // Type or member is obsolete

namespace WebCrawler;

public class WebIndexingPipeline(string openAiApiKey)
{
    
    private readonly ITextEmbeddingGenerationService _embeddingService = new OpenAITextEmbeddingGenerationService(
        modelId: "text-embedding-3-small",
        apiKey: openAiApiKey
    );
    
    private readonly VectorStore _vectorStore = new InMemoryVectorStore();


    public async Task CrawlAndIndexSiteAsync(string siteUrl, CancellationToken cancellationToken = default)
    {
        DisplayUtil.WriteInformation($"Starting site crawl: {siteUrl}");
        DisplayUtil.Separator();
       
        // Initialize crawler for this site
        var crawler = new WebCrawler(siteUrl);
        
        // 1. Crawl the entire site
        var crawledPages = await crawler.CrawlSiteAsync();
        Console.WriteLine($"Crawled {crawledPages.Count} pages");
        
        // 2. Get or create collection
        var collection = _vectorStore.GetCollection<string, WebPageChunk>("web_content");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        
        // 3. Process pages in batches
        var allRecords = new List<WebPageChunk>();
        var pageCount = 0;
        DisplayUtil.Separator();
        foreach (var (url, title, content) in crawledPages)
        {
            pageCount++;
            Console.WriteLine($"Processing page {pageCount}/{crawledPages.Count}: {url}");
            
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("  Skipped: No content");
                continue;
            }
            
            // Chunk the content
            var chunks = TextChunker.ChunkText(content);
            DisplayUtil.WriteLineGreen($"Created {chunks.Count} chunks");
            
            // Generate embeddings for chunks
            for (var i = 0; i < chunks.Count; i++)
            {
                try
                {
                    //var embedding = await _embeddingService.GenerateEmbeddingAsync(
                      //  chunks[i], 
                        //cancellationToken: cancellationToken);
                    
                    var record = new WebPageChunk
                    {
                        Id = $"{Guid.NewGuid()}",
                        Url = url,
                        Content = chunks[i],
                        Title = title,
                        CrawledAt = DateTime.UtcNow,
                        //ContentEmbedding = embedding
                    };
                    
                    allRecords.Add(record);
                    
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error processing chunk {i}: {ex.Message}");
                }
            }
        }
        
        // Upsert remaining records
        if (allRecords.Count != 0)
        {
            DisplayUtil.Separator();
            allRecords.ForEach(async void (r) =>
            {
                await collection.UpsertAsync(r, cancellationToken);
                DisplayUtil.WriteLineSuccess($"Indexed chunks {r.Title} from {r.Url}");
            }); 
            
            
            
        }
        DisplayUtil.Separator();
        DisplayUtil.WriteLineInformation($"\nIndexing complete!");
        DisplayUtil.WriteLineInformation($"Total pages crawled: {crawledPages.Count}");
        DisplayUtil.WriteLineInformation($"Total chunks indexed: {allRecords.Count}"); 
    }
}