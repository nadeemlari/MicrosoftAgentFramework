using System.ClientModel;
using NAudio.Utils;
using NAudio.Wave;
using OpenAI;
using OpenAI.Audio;
using Shared;

var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ;
var client = new OpenAIClient(apiKey);
var audioClient = client.GetAudioClient("gpt-4o-mini-tts");
var agent = client
    .GetChatClient("gpt-4.1-mini")
    .CreateAIAgent(instructions: "You are a Friendly AI Bot, answering questions");

var thread =  agent.GetNewThread();

while (true)
{
    Console.WriteLine("Press any key to start recording...");
    Console.ReadKey();
    using var audioStream = RecordAudio();
    ClientResult<AudioTranscription> result = await audioClient.TranscribeAudioAsync(audioStream, "audio.wav");
    var questionFromAudio = result.Value.Text;
    Console.WriteLine($"> {questionFromAudio}");
    var response = await agent.RunAsync(questionFromAudio, thread);
    Console.WriteLine(response);
    
    DisplayUtil.Separator();
}

MemoryStream RecordAudio()
{
    MemoryStream stream = new();
    using WaveInEvent waveIn = new();
    waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
    WaveFileWriter writer = new(new IgnoreDisposeStream(stream), waveIn.WaveFormat);

    waveIn.DataAvailable += (_, args) => { writer.Write(args.Buffer, 0, args.BytesRecorded); };
    waveIn.StartRecording();

    Console.WriteLine("Recording... Press any key to stop");
    Console.ReadKey();

    waveIn.StopRecording();
    stream.Position = 0;
    return stream;
}