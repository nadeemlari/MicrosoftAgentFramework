using NAudio.Wave;
using OpenAI;
using OpenAI.Audio;

var apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey") ;
var client = new OpenAIClient(apiKey);
var audioClient = client.GetAudioClient("gpt-4o-mini-tts");
var voice = new GeneratedSpeechVoice("shimmer");
const string text = "Hi! Welcome to this video about OpenAI's AudioClient. I'm an AI speaking the words Rasmus entered in his program";
var result = audioClient.GenerateSpeech(text,voice,new SpeechGenerationOptions
{
  SpeedRatio   = 1,
  ResponseFormat = new GeneratedSpeechFormat("mp3")
});
var bytes =result.Value.ToArray();
File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "test.mp3"), bytes);
var waveStream = new Mp3FileReader(new MemoryStream(bytes));
var player = new WaveOutEvent();
player.Init(waveStream);
player.Play();
Console.WriteLine("Playing audio.Press Enter to exit...");
Console.ReadLine();
