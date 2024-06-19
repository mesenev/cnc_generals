using System.Threading;
using Grpc.Core;
using NAudio.Wave;
using Grpc.Net.Client;
using Lime;
using SharedObjects.Proto;

namespace Game.Stuff;

public class VoiceStreamingProcessor : IProcessor {
    private readonly WaveInEvent waveInEvent;
    private GrpcChannel channel;
    private SpeechToCommand.SpeechToCommandClient voiceStreamingClient;
    private AsyncClientStreamingCall<VoiceAudio, Response> callForSendVoice;

    public VoiceStreamingProcessor() {
        WaveFormat format = new WaveFormat(22050, 1);
        waveInEvent = new WaveInEvent();
        waveInEvent.WaveFormat = format;

        waveInEvent.DataAvailable += OnWaveInEventOnDataAvailable;
        // waveInEvent.StartRecording();
    }

    private void OnWaveInEventOnDataAvailable(object s, WaveInEventArgs a) {
        callForSendVoice.RequestStream.WriteAsync(
            new VoiceAudio { RecordVoice = Google.Protobuf.ByteString.CopyFrom(a.Buffer) }
        );
    }

    public void Update(float delta, GameObjects.Game game) {
        if (Window.Current.Input.WasKeyReleased(Key.Space)) {
            waveInEvent.StopRecording();
            Thread.Sleep(100);
            channel?.Dispose();
            callForSendVoice?.Dispose();
            channel = null;
            voiceStreamingClient = null;
            callForSendVoice = null;
        }

        if (Window.Current.Input.WasKeyPressed(Key.Space)) {
            channel ??= GrpcChannel.ForAddress("http://localhost:12344");
            voiceStreamingClient ??= new SpeechToCommand.SpeechToCommandClient(channel);
            callForSendVoice ??= voiceStreamingClient.AudioToText();
            waveInEvent.StartRecording();
        }
    }
}
