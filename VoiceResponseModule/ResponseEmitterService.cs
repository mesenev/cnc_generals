using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedObjects.GameObjects;
using SharedObjects.TextToSpeech;

namespace VoiceResponseModule;

public class ResponseEmitterService {
    private YandexTtSBackend yandexBackend = new();
    public bool IsEmitting { get; set; }
    private UnitVoiceDatabase database;
    private readonly Queue<VoiceRequest> voiceRequests = new();
    public void AddVoiceRequest(VoiceRequest request) => voiceRequests.Enqueue(request);

    private readonly Dictionary<int, object> transmitters = new();

    public ResponseEmitterService(VoiceTransmitterData[] data, UnitVoiceDatabase database) {
        this.database = database;
        foreach (var transmitterData in data) {
            object transmitter = new();
            transmitters[transmitterData.Player.PlayerId] = transmitter;
        }
    }

    private byte[] PostProduction(byte[] audio, object args) {
        throw new NotImplementedException();
    }

    private void TransmitResponse(VoiceRequest request, byte[] answer) {
        throw new NotImplementedException();
        // transmitters[request.PlayerId].Send(answer);
    }

    private async Task processVoiceRequest(VoiceRequest request) {
        string text = VoiceRequestToTextService.RenderVoiceRequest(request);
        byte[] voiceAnswer = await YandexTtSBackend.Synthesize(
            text, database.GetUnitVoiceByUnitId(request.UnitId)
        );
    }
}

public struct VoiceTransmitterData {
    public PlayerInfo Player;
    public int Port;
}
