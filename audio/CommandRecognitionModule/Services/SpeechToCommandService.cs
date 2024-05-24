using System.Text;
using ApiClient;
using Grpc.Core;
namespace CommandRecognitionModule.Services;

public class SpeechToCommandService : SpeechToCommand.SpeechToCommandBase
{
    private readonly ILogger<SpeechToCommandService> _logger;
    private readonly YandexSttClient _yandexSttClient;
    
    public SpeechToCommandService(ILogger<SpeechToCommandService> logger)
    {
        _logger = logger;
        _yandexSttClient = new YandexSttClient("t1.9euelZrJjM_Om53Ijc6PlZaWyMuNzO3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_d6VT5N-e9zQXhR_t3z9zoEPE3573NBeFH-zef1656Vms-UyJ2Si5yOy8_KlpaQjZ6b7_zF656Vms-UyJ2Si5yOy8_KlpaQjZ6b.Mas-C0E5ef1sjX3mv1V_a13mx6Asr21H90pEp_hmzFCAkQMAAzcOywvawRbHM7tg67C1y4bNOUztcFpuGG4nDw", new Uri("https://stt.api.cloud.yandex.net:443"), "b1gv6ij7c6frub5g0dk2");
        _yandexSttClient.startStreamToApi();
    }

    public override async Task<Response> AudioToText(IAsyncStreamReader<VoiceAudio> requestStream, ServerCallContext context)
    {   
        await foreach(VoiceAudio audio_voice in requestStream.ReadAllAsync())
        {
            _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice).Wait();
        }
        return new Response {Content = "Working"};
    }

    public override async Task TextToCommand(
        InputText request,
        IServerStreamWriter<DummyCommand> responseStream,
        ServerCallContext context)
    {
        List<string> known_commands = new List<string>(["вверх", "вниз", "влево", "вправо"]);
        CommandParser parser = new CommandParser(known_commands);

        while (true)
        {
            string recString = _yandexSttClient.ReadFinalRefirementResponseFromOpenStream().Result;
            //recString = Encoding.Default.GetString(Encoding.Default.GetBytes(recString));
            Console.WriteLine(recString);
            if (recString.Split().Length >= 2)
            {
                try
                {
                    int offset = Int32.Parse(recString.Split()[1]);
                    await responseStream.WriteAsync(new DummyCommand
                        { Direction = recString.Split()[0], Offset = offset });
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                await responseStream.WriteAsync(new DummyCommand
                    { Direction = "", Offset = 0 });
            }

        }
    }
}