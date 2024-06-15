using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using yandex.tts;

namespace VoiceResponseModule;

public class YandexTtSBackend {
    private static readonly string iamToken = Environment.GetEnvironmentVariable("TTS_YANDEX_IAMTOKEN");
    private static readonly string folderId = Environment.GetEnvironmentVariable("TTS_YANDEX_FOLDER_ID");

    public async Task<byte[]> Synthesize(string text) {
        var request = new UtteranceSynthesisRequest {
            Text = text,
            OutputAudioSpec = new AudioFormatOptions {
                ContainerAudio = new ContainerAudio {
                    ContainerAudioType = ContainerAudio.Types.ContainerAudioType.Wav
                }
            },
            Hints = {
                new Hints { Voice = "alexander" },
                new Hints { Role = "good" },
                new Hints { Speed = 1.1 }
            },
            LoudnessNormalizationType =
                UtteranceSynthesisRequest.Types.LoudnessNormalizationType.Lufs
        };

        var channel = GrpcChannel.ForAddress("https://tts.api.cloud.yandex.net:443");
        var client = new Synthesizer.SynthesizerClient(channel);

        var headers = new Metadata {
            { "authorization", $"Bearer {iamToken}" },
            { "x-folder-id", $"{folderId}" },
        };

        var call = client.UtteranceSynthesis(request, headers);

        using var audio = new MemoryStream();
        await foreach (var response in call.ResponseStream.ReadAllAsync()) {
            audio.Write(
                response.AudioChunk.Data.ToByteArray(),
                0,
                response.AudioChunk.Data.Length
            );
        }

        audio.Seek(0, SeekOrigin.Begin);
        return audio.ToArray();
    }
}
