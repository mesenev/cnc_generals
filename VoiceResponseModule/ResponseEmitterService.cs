using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DeepMorphy;
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

    public ResponseEmitterService(UnitVoiceDatabase database, GameState gameState) {
        this.database = database;
        VoiceRequestToTextService.gameState = gameState;
    }

    public void AddPeer(VoiceTransmitterData transmitterData) {
        object transmitter = new();
        transmitters[transmitterData.Player.PlayerId] = transmitter;
    }

    private byte[] PostProduction(byte[] audio, object args) {
        throw new NotImplementedException();
    }

    private void TransmitResponse(VoiceRequest request, byte[] answer) {
        throw new NotImplementedException();
        // transmitters[request.PlayerId].Send(answer);
    }

    public async void ProcessNext() {
        if (voiceRequests.Count == 0)
            return;
        await ProcessVoiceRequest(voiceRequests.Dequeue());

    }

    private async Task ProcessVoiceRequest(VoiceRequest request) {
        string text = VoiceRequestToTextService.RenderVoiceRequest(request);
        Debug.WriteLine(text);
        return;
        byte[] voiceAnswer = await YandexTtSBackend.Synthesize(
            text, database.GetUnitVoiceByUnitId(request.UnitId)
        );
    }
}

public struct VoiceTransmitterData {
    public PlayerInfo Player;
    public int Port;
}
