using RagUtils;

namespace Crawler.Pdf;

public interface IIngestionSource
{
    string SourceId { get; }

    Task<IEnumerable<IngestedPdfDocument>> GetNewOrModifiedDocumentsAsync(IReadOnlyList<IngestedPdfDocument> existingDocuments);

    Task<IEnumerable<IngestedPdfDocument>> GetDeletedDocumentsAsync(IReadOnlyList<IngestedPdfDocument> existingDocuments);

    Task<IEnumerable<PdfChunk>> CreateChunksForDocumentAsync(IngestedPdfDocument document);
}