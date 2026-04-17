using Microsoft.Extensions.VectorData;
using Utilities;
using RagUtils;


namespace Crawler.Pdf;

public class PdfIngestor(VectorStoreCollection<ulong, PdfChunk> chunksCollection, VectorStoreCollection<ulong, IngestedPdfDocument> documentsCollection)
{
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
            
        }
    }
}
