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
            "t1.9euelZrIyYqdkpXKm8bOj5OblpqXj-3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_ceVTVN-e9Xekpn_d3z914DM03571d6Smf9zef1656VmpeVl8aTlYyPjMiZzY2YzIuV7_zF656VmpeVl8aTlYyPjMiZzY2YzIuV.iZag2k_Y9-G9X4ub9-ujIQ23Ftn_EFvYHEw6x5eQRtZ7nYktSJQGqeqr4lSuEhROoVRm0zBMDMElbgCAYV5oBA";
        _yandexSttClient = new YandexSttClient(iamToken, new Uri("https://stt.api.cloud.yandex.net:443"),
            "b1gv6ij7c6frub5g0dk2");
        _yandexSttClient.startStreamToApi();
    }

    public override async Task<Response> AudioToText(IAsyncStreamReader<VoiceAudio> requestStream,
        ServerCallContext context) {
        await foreach (VoiceAudio audio_voice in requestStream.ReadAllAsync()) {
            try {
                _yandexSttClient.writeDataToOpenStreamCall(audio_voice.RecordVoice).Wait();
            }
            //Если ytt deadline истек, пересоздает поток к ytt, и записывает в него последнее полученное сообщение
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
            Console.WriteLine("ITERATION");
            string recString = "";
            try {
                recString = _yandexSttClient.ReadFinalRefirementResponseFromOpenStream().Result;
            } catch (Exception error) {
                _yandexSttClient.disposeAll();
                _yandexSttClient.startStreamToApi();
                Console.WriteLine(error);
                Console.WriteLine(
                    "Exception during read final refirement from yandex stt. You should write something to request stream of CRM to recreate stream to YSTT");
            }

            //recString = Encoding.Default.GetString(Encoding.Default.GetBytes(recString));
            Console.WriteLine($"REC STRING: {recString}");
            if (recString.Split().Length >= 2) {
                try {
                    int offset = Int32.Parse(recString.Split()[1]);
                    await responseStream.WriteAsync(new DummyCommand {
                        Direction = recString.Split()[0], Offset = offset
                    });
                } catch (FormatException e) {
                    Console.WriteLine("");
                }
            }
            //else
            // {
            //     await responseStream.WriteAsync(new DummyCommand{ Direction = "EMPTY ANSWER", Offset = 0 });
            // }
        }
    }
}
