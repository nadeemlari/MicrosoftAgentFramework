using Utilities;
using RagSearch;


// DisplayUtil.WriteLineInformation("Web  Search - Web Semantic Search Example - Write Your Query");
// var query = Console.ReadLine();
// var webSearch = new WebSemanticSearch();
// var results = await webSearch.SearchAsync(query ?? string.Empty, topK: 5);
//
// DisplayUtil.Separator();
// foreach (var (record, score) in results)
// {
//     DisplayUtil.WriteLineInformation($"Title: {record.Title}");
//     DisplayUtil.WriteLineInformation($"URL: {record.Url}");
//     DisplayUtil.WriteLineInformation($"Score: {score}");
//     DisplayUtil.WriteLineInformation($"Content Snippet: {record.Content[..Math.Min(200, record.Content.Length)]}...");
//     DisplayUtil.Separator();
// }

DisplayUtil.WriteLineInformation("Pdf  Search - Pdf Semantic Search Example - Write Your Query");
var pdfQuery = Console.ReadLine();
var pdfSearch = new PdfSemanticSearch();
var pdfResults = await pdfSearch.SearchAsync(pdfQuery ?? string.Empty, topK: 5);

DisplayUtil.Separator();
foreach (var (record, score) in pdfResults)
{
    DisplayUtil.WriteLineInformation($"Title: {record.DocumentId}");
    DisplayUtil.WriteLineInformation($"Page#: {record.PageNumber}");
    DisplayUtil.WriteLineInformation($"Score: {score}");
    DisplayUtil.WriteLineInformation($"Content Snippet: {record.Text[..Math.Min(200, record.Text.Length)]}...");
    DisplayUtil.Separator();
}