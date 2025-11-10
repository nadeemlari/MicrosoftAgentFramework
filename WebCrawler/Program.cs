using Crawler.Pdf;
using Crawler.Web;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
#pragma warning disable CS0618 // Type or member is obsolete

var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey")??"your-api-key-here";

#region Web Crawling and Indexing
//var cancellationTokenSource = new CancellationTokenSource();
//var pipeline = new WebIndexingPipeline(apiKey);
//await pipeline.CrawlAndIndexSiteAsync("https://www.solenis.com/", cancellationToken: cancellationTokenSource.Token);
#endregion

#region PDF Ingestion

var vectorStore = new QdrantVectorStore(new QdrantClient("qdrant.pub.nadeemlari.in"), ownsClient: true);
var chunksCollection = vectorStore.GetCollection<ulong, PdfChunk>("solenis_pdf_chunk_content");
var documentsCollection = vectorStore.GetCollection<ulong, IngestedPdfDocument>("solenis_pdf_doc_content");

var embeddingService = new OpenAITextEmbeddingGenerationService(modelId: "text-embedding-3-small", apiKey: apiKey);

var pdfIngestor = new PdfIngestor(chunksCollection, documentsCollection);
await pdfIngestor.IngestDataAsync(new PdfDirectorySource(@"C:\Users\mnadeem\RiderProjects\MicrosoftAgentFramework\WebCrawler\Data\Pdfs", embeddingService));
#endregion
