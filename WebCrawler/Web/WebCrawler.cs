using System.Collections.Concurrent;
using HtmlAgilityPack;
using Utilities;


namespace Crawler.Web;

public class WebCrawler
{
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, bool> _visitedUrls;
    private readonly ConcurrentBag<(string Url, string Title, string Content)> _crawledPages;
    private readonly string _baseUrl;
   
    public WebCrawler(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

        _visitedUrls = new ConcurrentDictionary<string, bool>();
        _crawledPages = new ConcurrentBag<(string, string, string)>();
        _baseUrl = NormalizeUrl(baseUrl);
      
    }

    public async Task<List<(string Url, string Title, string Content)>> CrawlSiteAsync()
    {
        await CrawlPageRecursiveAsync(_baseUrl, 0);
        return _crawledPages.ToList();
    }

    private async Task CrawlPageRecursiveAsync(string url, int depth)
    {
        
        // Check if already visited
        if (!_visitedUrls.TryAdd(url, true))
            return;

        try
        {
            DisplayUtil.WriteLineDarkGray($"Crawling (depth {depth}): {url}");

            // Crawl the page
            var html = await _httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Extract content
            var title = htmlDoc.DocumentNode
                .SelectSingleNode("//title")
                .InnerText.Trim();

            var content = ExtractContent(htmlDoc);

            // Store the crawled page
            _crawledPages.Add((url, title, content));
            
            // Extract and follow links
            var links = ExtractLinks(htmlDoc, url);

            foreach(var link in links.Where(link => ShouldCrawl(link)))
            {
                await CrawlPageRecursiveAsync(link, depth + 1);
                
            }

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crawling {url}: {ex.Message}");
        }
        
    }

    private static string ExtractContent(HtmlDocument htmlDoc)
    {
        // Remove script and style elements
        var nodesToRemove = htmlDoc.DocumentNode
            .SelectNodes("//script | //style | //nav | //header | //footer");

        foreach (var node in nodesToRemove)
        {
            node.Remove();
        }

        // Extract main content
        var contentNodes = htmlDoc.DocumentNode
            .SelectNodes("//main | //article | //div[@class='content'] | //p | //h1 | //h2 | //h3");

        var content = string.Join("\n",
            contentNodes.Select(n => n.InnerText.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t)));

        return content;
    }

    private static List<string> ExtractLinks(HtmlDocument htmlDoc, string currentUrl)
    {
        var links = new List<string>();
        var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

        foreach (var linkNode in linkNodes)
        {
            var href = linkNode.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href)) continue;

            // Convert relative URLs to absolute
            var absoluteUrl = ConvertToAbsoluteUrl(href, currentUrl);
            if (!string.IsNullOrEmpty(absoluteUrl))
            {
                links.Add(absoluteUrl);
            }
        }

        return links.Distinct().ToList();
    }

    private static string ConvertToAbsoluteUrl(string href, string currentUrl)
    {
        try
        {
            if (Uri.TryCreate(href, UriKind.Absolute, out var absoluteUri))
            {
                return absoluteUri.ToString();
            }

            if (Uri.TryCreate(new Uri(currentUrl), href, out var relativeUri))
            {
                return relativeUri.ToString();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return string.Empty;
    }

    private bool ShouldCrawl(string url)
    {
        // Only crawl URLs within the same domain
        if (!url.StartsWith(_baseUrl, StringComparison.OrdinalIgnoreCase))
            return false;

        // Skip common non-content URLs
        var skipExtensions = new[] { ".pdf", ".jpg", ".png", ".gif", ".css", ".js", ".xml", ".zip" };
        if (skipExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            return false;

        // Skip anchor links
        return !url.Contains('#');
    }

    private static string NormalizeUrl(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Scheme}://{uri.Host}";
    }
}