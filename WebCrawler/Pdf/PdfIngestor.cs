using Microsoft.Extensions.VectorData;
using Shared;

namespace Crawler.Pdf;

public class PdfIngestor(VectorStoreCollection<ulong, PdfChunk> chunksCollection, VectorStoreCollection<ulong, IngestedPdfDocument> documentsCollection)
{
    // public static async Task IngestDataAsync(IServiceProvider services, IIngestionSource source)
    // {
    //     using var scope = services.CreateScope();
    //     var ingestor = scope.ServiceProvider.GetRequiredService<DataIngestor>();
    //     await ingestor.IngestDataAsync(source);
    // }

    public async Task IngestDataAsync(IIngestionSource source)
    {
        await chunksCollection.EnsureCollectionExistsAsync();
        await documentsCollection.EnsureCollectionExistsAsync();

        var sourceId = source.SourceId;
        var documentsForSource = await documentsCollection.GetAsync(doc => doc.SourceId == sourceId, top: int.MaxValue).ToListAsync();

        var deletedDocuments = await source.GetDeletedDocumentsAsync(documentsForSource);
        foreach (var deletedDocument in deletedDocuments)
        {
            DisplayUtil.WriteLineInformation($"Removing ingested data for {deletedDocument.DocumentId}");
            await DeleteChunksForDocumentAsync(deletedDocument);
            //await documentsCollection.DeleteAsync(deletedDocument.Key);
        }

        var modifiedDocuments = await source.GetNewOrModifiedDocumentsAsync(documentsForSource);
        foreach (var modifiedDocument in modifiedDocuments)
        {
            DisplayUtil.WriteLineInformation($"Processing {modifiedDocument.DocumentId}");
            await DeleteChunksForDocumentAsync(modifiedDocument);

            await documentsCollection.UpsertAsync(modifiedDocument);

            var newRecords = await source.CreateChunksForDocumentAsync(modifiedDocument);
            await chunksCollection.UpsertAsync(newRecords);
        }

        DisplayUtil.WriteLineInformation("Ingestion is up-to-date");

        async Task DeleteChunksForDocumentAsync(IngestedPdfDocument document)
        {
            var documentId = document.DocumentId;
            var chunksToDelete = await chunksCollection.GetAsync(record => record.DocumentId == documentId, int.MaxValue).ToListAsync();
            if (chunksToDelete.Count != 0)
            {
                //await chunksCollection.DeleteAsync(chunksToDelete.Select(r => r.Key));
            }
        }
    }
}
