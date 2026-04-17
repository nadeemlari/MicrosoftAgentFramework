using Utilities;
using OpenAI.Images;
using System.Diagnostics;
#pragma warning disable OPENAI001

var client = ImageClientProvider.GetOpenAiClient("gpt-image-1");
var image = await client.GenerateImageAsync("A Tiger in a jungle with a party-hat", new ImageGenerationOptions{
    
    Background = GeneratedImageBackground.Auto,
    Quality = GeneratedImageQuality.Auto,
    Size = GeneratedImageSize.W1024xH1024,
    OutputFileFormat = GeneratedImageFileFormat.Png,
});
var bytes = image.Value.ImageBytes.ToArray();
var path = Path.Combine(Path.GetTempPath(), $"image-{Guid.NewGuid():N}.png");
File.WriteAllBytes(path, bytes);

await Task.Factory.StartNew(() =>
{
    Process.Start(new ProcessStartInfo
    {
        FileName = path,
        UseShellExecute = true
    });
});