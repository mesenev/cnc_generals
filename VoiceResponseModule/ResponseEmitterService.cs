using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using NAudio.Wave;
using SharedObjects.GameObjects;
using SharedObjects.Proto;
using SharedObjects.TextToSpeech;
using VoiceResponseModule.Backends;

namespace VoiceResponseModule;

public class ResponseEmitterService {
    private YandexTtSBackend yandexBackend = new();
    private MsEmbeddedTtSBackend msBackend = new();
    public bool IsEmitting { get; set; }
    private UnitVoiceDatabase database;
    private readonly Queue<VoiceRequest> voiceRequests = new();
    public void AddVoiceRequest(VoiceRequest request) => voiceRequests.Enqueue(request);

    private readonly Dictionary<int, VoiceEmitterConnection> transmitters = new();

    public ResponseEmitterService(
        UnitVoiceDatabase database, GameState gameState, int peersAmount, int port
    ) {
        this.database = database;
        VoiceRequestToTextService.gameState = gameState;
        for (int i = 0; i < peersAmount; i++)
            AddPeer(new VoiceTransmitterData { PlayerId = i, Port = port + 1 + i });
    }

    public void AddPeer(VoiceTransmitterData transmitterData) {
        var emitter = new VoiceEmitterConnection();
        var server = new Server {
            Services = { StreamAudioFile.BindService(emitter) },
            Ports = {
                new ServerPort("localhost", transmitterData.Port, ServerCredentials.Insecure)
            }
        };
        server.Start();
        transmitters[transmitterData.PlayerId] = emitter;
    }

    private byte[] PostProduction(byte[] audio, object? args) {
        return audio;
    }

    private void TransmitResponse(VoiceRequest request, byte[] answer) {
        transmitters[request.PlayerId].addBuffer(answer);
    }

    public async void ProcessNext() {
        if (voiceRequests.Count == 0)
            return;
        await ProcessVoiceRequest(voiceRequests.Dequeue());
    }

    private async Task ProcessVoiceRequest(VoiceRequest request) {
        string text = VoiceRequestToTextService.RenderVoiceRequest(request);
        Debug.WriteLine(text);
        byte[] voiceAnswer = await msBackend.Synthesize(
            text, database.GetUnitVoiceByUnitId(request.UnitId)
        );
        // var provider = new BufferedWaveProvider(new WaveFormat(19000, 1));
        // provider.AddSamples(voiceAnswer, 0, voiceAnswer.Length);
        // var provider = new WaveFileReader("test.wav");
        // Debug.WriteLine(provider.WaveFormat);
        // WaveOutEvent waveOutEvent = new WaveOutEvent();
        // waveOutEvent.Init(provider);
        // waveOutEvent.Play();

        var fileStream = new Mp3FileReader("tears_in_rain.mp3");
        var buffer = new byte[fileStream.WaveFormat.AverageBytesPerSecond];

        for (long secWindow = 0; secWindow < 6; secWindow++) {
            fileStream.Read(buffer, 0, buffer.Length);
            TransmitResponse(request, buffer);
        }
    }
}

public struct VoiceTransmitterData {
    public int PlayerId;
    public int Port;
}
