namespace SharedObjects.TextToSpeech;

public struct VoiceRequest {
    public int unitId;
    public VoiceRequestType VoiceRequestType;

    public VoiceRequest(int unitId, VoiceRequestType voiceRequestType) {
        this.unitId = unitId;
        VoiceRequestType = voiceRequestType;
    }
}

public enum VoiceRequestType {
    Confirmation,
    Report,
    EnemySpotted,
    EnemyEngaged,
    TaskFinished,
    RandomEvent
}
