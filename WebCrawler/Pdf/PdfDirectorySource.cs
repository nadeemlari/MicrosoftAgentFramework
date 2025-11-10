using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace Crawler.Pdf;

public class PdfDirectorySource(string sourceDirectory, OpenAITextEmbeddingGenerationService embeddingService) : IIngestionSource
{
    public static string SourceFileId(string path) => Path.GetFileName(path);
    public static string SourceFileVersion(string path) => File.GetLastWriteTimeUtc(path).ToString("o");

    public string SourceId => $"{nameof(PdfDirectorySource)}:{sourceDirectory}";

    public Task<IEnumerable<IngestedPdfDocument>> GetNewOrModifiedDocumentsAsync(IReadOnlyList<IngestedPdfDocument> existingDocuments)
    {
        var results = new List<IngestedPdfDocument>();
        var sourceFiles = Directory.GetFiles(sourceDirectory, "*.pdf");
        var existingDocumentsById = existingDocuments.ToDictionary(d => d.DocumentId);

        foreach (var sourceFile in sourceFiles)
        {
            var sourceFileId = SourceFileId(sourceFile);
            var sourceFileVersion = SourceFileVersion(sourceFile);
            var existingDocumentVersion = existingDocumentsById.TryGetValue(sourceFileId, out var existingDocument) ? existingDocument.DocumentVersion : null;
            if (existingDocumentVersion != sourceFileVersion)
            {
                results.Add(new() { Key = Guid.NewGuid(), SourceId = SourceId, DocumentId = sourceFileId, DocumentVersion = sourceFileVersion });
            }
        }

        return Task.FromResult((IEnumerable<IngestedPdfDocument>)results);
    }

    public Task<IEnumerable<IngestedPdfDocument>> GetDeletedDocumentsAsync(IReadOnlyList<IngestedPdfDocument> existingDocuments)
    {
        var currentFiles = Directory.GetFiles(sourceDirectory, "*.pdf");
        var currentFileIds = currentFiles.ToLookup(SourceFileId);
        var deletedDocuments = existingDocuments.Where(d => !currentFileIds.Contains(d.DocumentId));
        return Task.FromResult(deletedDocuments);
    }

    public Task<IEnumerable<PdfChunk>> CreateChunksForDocumentAsync(IngestedPdfDocument document)
    {
        using var pdf = PdfDocument.Open(Path.Combine(sourceDirectory, document.DocumentId));
        var paragraphs = pdf.GetPages().SelectMany(GetPageParagraphs).ToList();

        return Task.FromResult(paragraphs.Select(p => new PdfChunk
        {
            Key = Guid.NewGuid(),
            DocumentId = document.DocumentId,
            PageNumber = p.PageNumber,
            Text = p.Text,
            VectorData = embeddingService.GenerateEmbeddingAsync(p.Text).GetAwaiter().GetResult()
        }));
    }

    private static IEnumerable<(int PageNumber, int IndexOnPage, string Text)> GetPageParagraphs(Page pdfPage)
    {
        var letters = pdfPage.Letters;
        var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);
        var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
        var pageText = string.Join(Environment.NewLine + Environment.NewLine,
            textBlocks.Select(t => t.Text.ReplaceLineEndings(" ")));

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
        return TextChunker.SplitPlainTextParagraphs([pageText], 200)
            .Select((text, index) => (pdfPage.Number, index, text));
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
    }
}
