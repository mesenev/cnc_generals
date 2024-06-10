using System;
using CommandRecognitionModule;
using Grpc.Core;
using NAudio.Wave;
using Grpc.Net.Client;
namespace Game.Stuff {
    
    
    public class VoiceStreamingProcessor : IProcessor {

        private WaveInEvent waveInEvent;
        private GrpcChannel channel;
        private SpeechToCommand.SpeechToCommandClient voiceStreamingClient;
        private AsyncClientStreamingCall<VoiceAudio, Response> callForSendVoice;
        
        public VoiceStreamingProcessor() {
            
            channel = GrpcChannel.ForAddress("http://localhost:5175");
            voiceStreamingClient = new SpeechToCommand.SpeechToCommandClient(channel);
            callForSendVoice = voiceStreamingClient.AudioToText();
            
            WaveFormat format = new WaveFormat(44100, 2);
            waveInEvent = new WaveInEvent();
            waveInEvent.WaveFormat = format;

            waveInEvent.DataAvailable += (s, a) => {
                callForSendVoice.RequestStream.WriteAsync(
                    new VoiceAudio {RecordVoice = Google.Protobuf.ByteString.CopyFrom(a.Buffer)}
                );
            };
            waveInEvent.StartRecording();
        }
        public void Update(float delta, Game game) {
            // if (channel.State != ConnectivityState.Ready) {
            //     Console.WriteLine("RECREATE CHANNEL");
            //     channel.Dispose();
            //     channel = GrpcChannel.ForAddress("http://localhost:5175");
            //
            //     voiceStreamingClient = new SpeechToCommand.SpeechToCommandClient(channel);
            //
            //     callForSendVoice.Dispose();
            //     callForSendVoice = voiceStreamingClient.AudioToText();
            //     
            //     waveInEvent.StopRecording();
            //     waveInEvent.DataAvailable += (s, a) => {
            //         
            //         callForSendVoice.RequestStream.WriteAsync(
            //             new VoiceAudio {RecordVoice = Google.Protobuf.ByteString.CopyFrom(a.Buffer)}
            //         );
            //     };
            //     waveInEvent.StartRecording();
            // }
        }
    }
}
