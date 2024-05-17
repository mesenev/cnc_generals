using ApiClient;
using Grpc.Core;
namespace CommandRecognitionModule.Services;

public class SpeechToCommandService : SpeechToCommand.SpeechToCommandBase
{
    private readonly ILogger<SpeechToCommandService> _logger;
    private readonly YandexSttClient _yandexSttClient;
    private string test_string = "";
    
    public SpeechToCommandService(ILogger<SpeechToCommandService> logger)
    {
        Console.WriteLine("KONSTRUCT CALLED");
        _logger = logger;
        _yandexSttClient = new YandexSttClient("t1.9euelZrKlpPLjcaRiZKKl4nLz46Rz-3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_dfAGRN-e80Umw0_t3z9x8vYU357zRSbDT-zef1656Vmo2UzcebzJaWnYyMmZmOzZOX7_zF656Vmo2UzcebzJaWnYyMmZmOzZOX.lHvCasZ6cCJC977tS0nSLYQQKn4zfqHMraTvitlSrpndipCM-vW_MUelDeuSczt3EjkbWPe5Kyx7VZ9YA0XmAw", new Uri("https://stt.api.cloud.yandex.net:443"), "b1gv6ij7c6frub5g0dk2");
        _yandexSttClient.startStreamToApi();
    }

    public override async Task<Response> AudioToText(IAsyncStreamReader<VoiceAudio> requestStream, ServerCallContext context)
    {   
        await foreach(VoiceAudio audio_voice in requestStream.ReadAllAsync())
        {
            await _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice);
        }
        return new Response {Content = "Working"};
    }

    public override async Task TextToCommand(
        InputText request,
        IServerStreamWriter<DummyCommand> responseStream,
        ServerCallContext context)
    {
        await _yandexSttClient.readAllDataFromResponseStream();
        await responseStream.WriteAsync(new DummyCommand { Direction = test_string, Offset = 10});
    }
}