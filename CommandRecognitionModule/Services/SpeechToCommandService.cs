using System.Text;
using CommandRecognitionModule.ApiClient;
using Grpc.Core;

namespace CommandRecognitionModule.Services;

public class SpeechToCommandService : SpeechToCommand.SpeechToCommandBase {
    private readonly ILogger<SpeechToCommandService> _logger;
    private readonly YandexSttClient _yandexSttClient;
    private readonly string iamToken;

    public SpeechToCommandService(ILogger<SpeechToCommandService> logger) {
        _logger = logger;
        iamToken =
            "t1.9euelZrGkc7GiYvPl5TNl4qLyZCTmu3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_c9Q2VM-e85czoN_d3z931xYkz57zlzOg39zef1656VmpGPiZSJi82azpXJkJ2ejMmY7_zF656VmpGPiZSJi82azpXJkJ2ejMmY.p3a4Ct6iAJ7FZq2dI7mt5Wvre7Bw6ysPY9lj5pRrVgVtqgdLKGJZUMvZ2lEQHDIWj-NLjcSGMZraJ52rZdO9Bg";
        _yandexSttClient = new YandexSttClient(iamToken, new Uri("https://stt.api.cloud.yandex.net:443"),
            "b1gv6ij7c6frub5g0dk2");
    }

    public override async Task<Response> AudioToText(IAsyncStreamReader<VoiceAudio> requestStream,
        ServerCallContext context) {
        Console.WriteLine("START AUDIO TO TEXT!");
        _yandexSttClient.startStreamToApi();
        await foreach (VoiceAudio audio_voice in requestStream.ReadAllAsync()) {
            try {
                _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice).Wait();
            }
            catch (Exception error) {
                Console.WriteLine("Exception during write to yandex stt. Trying reconnect");
                _yandexSttClient.disposeAll();
                _yandexSttClient.startStreamToApi();
                _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice).Wait();
            }
        }

        return new Response { Content = "Working" };
    }

    public override async Task TextToCommand(
        InputText request,
        IServerStreamWriter<DummyCommand> responseStream,
        ServerCallContext context) {
        
        List<string> known_commands = new List<string>(["вверх", "вниз", "влево", "вправо"]);
        CommandParser parser = new CommandParser(known_commands);
        Console.WriteLine("START TEXT TO COMMAND SERVICE");
        while (true) {
            string recString = "";
            try {
                // в случае ошибки возвращается пустота
                recString = _yandexSttClient.TryReadFinalRefirementResponseFromOpenStream().Result;
            } catch (AggregateException error) {
                Console.WriteLine("Нечего считывать из потока  TextToCommand. Начните писать в AudioToText");
            }
            
            if (recString.Split().Length >= 2) {
                try {
                    int offset = Int32.Parse(recString.Split()[1]);
                    await responseStream.WriteAsync(new DummyCommand {
                        Direction = recString.Split()[0], Offset = offset
                    });
                    Console.WriteLine($"REC STRING: {recString}");
                } catch (FormatException e) {
                    //Console.WriteLine("");
                }
            }
            else
            {
                
                await responseStream.WriteAsync(new DummyCommand{ Direction = "EMPTY ANSWER", Offset = 0 });
            }
        }
    }
}
