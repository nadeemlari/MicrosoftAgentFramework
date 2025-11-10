using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using ChatApp.Components;
using ChatApp.Services;
using Qdrant.Client;
using RagUtils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var openAiClient = new OpenAIClient(
    new ApiKeyCredential(builder.Configuration["OpenAI:Key"] ?? throw new InvalidOperationException("Missing configuration: OpenAI:Key. See the README for details.")));

#pragma warning disable OPENAI001 // GetOpenAIResponseClient(string) is experimental and subject to change or removal in future updates.
var chatClient = openAiClient.GetOpenAIResponseClient("gpt-4o-mini").AsIChatClient();
#pragma warning restore OPENAI001

var embeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();
builder.Services.AddSingleton<QdrantClient>(_ => new QdrantClient("qdrant.pub.nadeemlari.in")); 
builder.Services.AddQdrantVectorStore();
builder.Services.AddQdrantCollection<ulong, IngestedPdfDocument>("solenis_pdf_doc_content");
builder.Services.AddQdrantCollection<ulong, PdfChunk>("solenis_pdf_chunk_content");
builder.Services.AddQdrantCollection<ulong, WebPageChunk>("solenis_web_content");

builder.Services.AddSingleton<PdfSemanticSearch>();
builder.Services.AddSingleton<WebSemanticSearch>();
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
