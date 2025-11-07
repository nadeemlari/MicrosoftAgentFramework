using WebCrawler;

var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey")??"your-api-key-here";
var cancellationTokenSource = new CancellationTokenSource();
var pipeline = new WebIndexingPipeline(apiKey);
await pipeline.CrawlAndIndexSiteAsync("https://www.solenis.com/", cancellationToken: cancellationTokenSource.Token);