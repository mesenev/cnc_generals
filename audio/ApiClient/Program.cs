// See https://aka.ms/new-console-template for more information

using ApiClient;
using Grpc.Core;
using Grpc.Net.Client;
using Speechkit.Stt.V3;
using NAudio.Wave;

RawAudio raw_audio = new RawAudio();
raw_audio.AudioEncoding = RawAudio.Types.AudioEncoding.Linear16Pcm;
raw_audio.SampleRateHertz = 44100;
raw_audio.AudioChannelCount = 2;


// ContainerAudio containerAudio = new ContainerAudio();
// containerAudio.ContainerAudioType = ContainerAudio.Types.ContainerAudioType.Unspecified;

AudioFormatOptions audio_format_options = new AudioFormatOptions();
audio_format_options.RawAudio = raw_audio;

TextNormalizationOptions text_normalization = new TextNormalizationOptions();
text_normalization.TextNormalization = TextNormalizationOptions.Types.TextNormalization.Enabled;
text_normalization.ProfanityFilter = true;
text_normalization.LiteratureText = false;

LanguageRestrictionOptions language_restriction = new LanguageRestrictionOptions();
language_restriction.RestrictionType = LanguageRestrictionOptions.Types.LanguageRestrictionType.Whitelist;
language_restriction.LanguageCode.Add("ru-RU");

RecognitionModelOptions recognition_model = new RecognitionModelOptions();
recognition_model.AudioFormat = audio_format_options;
recognition_model.TextNormalization = text_normalization;
recognition_model.LanguageRestriction = language_restriction;
recognition_model.AudioProcessingType = RecognitionModelOptions.Types.AudioProcessingType.RealTime;

StreamingOptions streaming_options =new StreamingOptions();
streaming_options.RecognitionModel = recognition_model;

StreamingRequest streamingRequestWithOptions = new StreamingRequest();
streamingRequestWithOptions.SessionOptions = streaming_options;


SslCredentials creds = new SslCredentials();

GrpcChannelOptions channelOptions = new GrpcChannelOptions();
channelOptions.Credentials = creds;


GrpcChannel channel = GrpcChannel.ForAddress("https://stt.api.cloud.yandex.net:443", channelOptions);
Recognizer.RecognizerClient recognizerClient = new Recognizer.RecognizerClient(channel);


Metadata metadata = new Metadata();
metadata.Add("authorization", "Bearer t1.9euelZqdjJvKmJPHkJyUkouVlZCVy-3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_c4Kg9O-e8sVVBx_t3z93hYDE757yxVUHH-zef1656Vmo6UmZbMzZGLz4vHyJPHkMia7_zF656Vmo6UmZbMzZGLz4vHyJPHkMia.JNpEl4CO48yuUR_cDVb0bIZ8hYwU_hJZxhHrkka8oV26p5sE9ZCDvy_QEH7UYj8sEsoj4SbycVIDR7o6fICXBw");
metadata.Add("x-folder-id",  "b1gv6ij7c6frub5g0dk2");

CallOptions callOptions = new CallOptions(metadata);

var call = recognizerClient.RecognizeStreaming(metadata, deadline: DateTime.UtcNow.AddMinutes(5));
call.RequestStream.WriteAsync(streamingRequestWithOptions).Wait();

// await foreach (var response in call.ResponseStream.ReadAllAsync())
// {
//     Console.WriteLine($"s2t chunk of {response.CalculateSize()} bytes recieved ");
//     // foreach (var chunk_rec in response)
//     // {
//     //     ChunkRecievedEventArgs evt = new ChunkRecievedEventArgs(chunk);
//     //     ChunkRecived?.Invoke(null, evt);
//     // }   
//     
// }

WaveFormat format = new WaveFormat(44100, 2);
WaveInEvent waveIn = new WaveInEvent();
waveIn.WaveFormat = format;

YandexSttClient apiClient = new YandexSttClient("t1.9euelZrKlpPLjcaRiZKKl4nLz46Rz-3rnpWamM2dy5eVz8aMx8-NncqKzZ3l8_dfAGRN-e80Umw0_t3z9x8vYU357zRSbDT-zef1656Vmo2UzcebzJaWnYyMmZmOzZOX7_zF656Vmo2UzcebzJaWnYyMmZmOzZOX.lHvCasZ6cCJC977tS0nSLYQQKn4zfqHMraTvitlSrpndipCM-vW_MUelDeuSczt3EjkbWPe5Kyx7VZ9YA0XmAw", new Uri("https://stt.api.cloud.yandex.net:443"), "b1gv6ij7c6frub5g0dk2");
apiClient.startStreamToApi();

waveIn.DataAvailable += (s, a) =>
{
    // StreamingRequest rR = new StreamingRequest();
    // AudioChunk audioChunk = new AudioChunk();
    // audioChunk.Data = Google.Protobuf.ByteString.CopyFrom(a.Buffer);
    // rR.Chunk = audioChunk;
    // call.RequestStream.WriteAsync(rR).Wait();
    // Console.WriteLine(a.Buffer.Length);
    
    apiClient.writeDataToOpenStreamCall(Google.Protobuf.ByteString.CopyFrom(a.Buffer));
    
};


waveIn.StartRecording();
Thread.Sleep(10000);
waveIn.StopRecording();

List<string> coms = new List<string>(["вверх", "вниз", "влево", "вправо"]);

CommandParser parser = new CommandParser(coms);

await apiClient.readAllDataFromResponseStream();
// using (FileStream fs = File.Open("test_file_ogg.opus", FileMode.Open))
// {
//     using (BufferedStream bs = new BufferedStream(fs, 32 * 1024))
//     {
//         int byteRead;
//         byte[] buffer = new byte[32 * 1024];
//
//         while ((byteRead = bs.Read(buffer, 0, 32 * 1024)) > 0)
//         {
//             StreamingRequest rR = new StreamingRequest();
//             AudioChunk audioChunk = new AudioChunk();
//             audioChunk.Data = Google.Protobuf.ByteString.CopyFrom(buffer);
//             rR.Chunk = audioChunk;
//             call.RequestStream.WriteAsync(rR).Wait();
//             Console.WriteLine(byteRead);
//         }
//     }
// }
// await foreach (var response in call.ResponseStream.ReadAllAsync())
// {
//     //Console.WriteLine($"RESPONSE Uuid : {response.SessionUuid}");
//     if (response.Partial != null)
//     {
//         Console.WriteLine($"RESPONSE PARTIAL : {response.Partial}");
//     }
//     if (response.Final != null)
//     {
//         Console.WriteLine($"RESPONSE FINAL : {response.Final}");
//     }
//     //Console.WriteLine($"RESPONSE FINAL : {response.Final}");
//     //Console.WriteLine($"RESPONSE audio cursor : {response.AudioCursors}");
// }
