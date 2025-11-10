using RagSearch;
using Shared;


DisplayUtil.WriteLineInformation("Web  Search - Web Semantic Search Example - Write Your Query");
var query = Console.ReadLine();
var webSearch = new WebSemanticSearch();
var results = await webSearch.SearchAsync(query ?? string.Empty, topK: 5);

DisplayUtil.Separator();
foreach (var (record, score) in results)
{
    DisplayUtil.WriteLineInformation($"Title: {record.Title}");
    DisplayUtil.WriteLineInformation($"URL: {record.Url}");
    DisplayUtil.WriteLineInformation($"Score: {score}");
    DisplayUtil.WriteLineInformation($"Content Snippet: {record.Content[..Math.Min(200, record.Content.Length)]}...");
    DisplayUtil.Separator();
}