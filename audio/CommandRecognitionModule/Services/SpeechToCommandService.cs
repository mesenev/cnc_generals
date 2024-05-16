using ApiClient;
using Grpc.Core;
namespace CommandRecognitionModule.Services;

public class SpeechToCommandService : SpeechToCommand.SpeechToCommandBase
{
    private readonly ILogger<SpeechToCommandService> _logger;
    private readonly YandexSttClient _yandexSttClient;
    private string test_string;
    
    public SpeechToCommandService(ILogger<SpeechToCommandService> logger)
    {
        Console.WriteLine("KONSTRUCT CALLED");
        _logger = logger;
        _yandexSttClient = new YandexSttClient("t1.9euelZqYmpydz4yZjYyTlJKbnYmMy-3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_c1KW9N-e9-O1Eg_t3z93VXbE357347USD-zef1656VmpSQnJCRjcaYjJrIis-OismW7_zF656VmpSQnJCRjcaYjJrIis-OismW.ys2JSZaBzAc8HxoEvNDsxa6mcxOGCn4DnWnOXaggtgJdamIkjvVPaRL4auKwYBhof_jhDOXMZL7a41Xd4fbZDA", new Uri("https://stt.api.cloud.yandex.net:443"), "b1gv6ij7c6frub5g0dk2");
        _yandexSttClient.startStreamToApi();
    }

    public override async Task<Response> AudioToText(IAsyncStreamReader<VoiceAudio> requestStream, ServerCallContext context)
    {   
        await foreach(VoiceAudio audio_voice in requestStream.ReadAllAsync())
        {
            _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice).Wait();
        }

        this.test_string = "save_value_between_calls";
        return new Response {Content = "Working"};
    }

    public override async Task TextToCommand(
        InputText request,
        IServerStreamWriter<DummyCommand> responseStream,
        ServerCallContext context)
    {
        await responseStream.WriteAsync(new DummyCommand { Direction = "", Offset = 10});
    }
}