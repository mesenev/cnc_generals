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
        _yandexSttClient = new YandexSttClient("t1.9euelZqJzp6WlsqSzZSOyMjMkJ2eyO3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_cfaU5N-e9bexxN_d3z918XTE3571t7HE39zef1656VmpmYlp3OlpbNiZSei5aVi8jI7_zF656VmpmYlp3OlpbNiZSei5aVi8jI.nzWyaVHgEqg6O96jUB80tPwrHIWoY8jTzpdoFG2M2TvkaHVSFwTNc78NYDw4yyLPK9x9dN7xle2qDCNZ7ZN-Cg", new Uri("https://stt.api.cloud.yandex.net:443"), "b1gv6ij7c6frub5g0dk2");
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