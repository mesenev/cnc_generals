using System.Text.Json;
using NAudio.Wave;

namespace ApiClient;

using Grpc.Core;
using Grpc.Net.Client;
using Speechkit.Stt.V3;

public class YandexSttClient
{
    private string iamToken;
    private Uri apiUri;
    private string folderId;
    private Recognizer.RecognizerClient recognizerClient;
    private AsyncDuplexStreamingCall<StreamingRequest, StreamingResponse> call;

    public YandexSttClient(string iam_token, Uri api_uri, string folder_id)
    {
        iamToken = iam_token;
        apiUri = api_uri;
        folderId = folder_id;
    }

    protected Metadata makeMetadata()
    {
        Metadata metadata = new Metadata();
        metadata.Add("authorization", $"Bearer {iamToken}");
        metadata.Add("x-folder-id",  folderId);
        return metadata;
    }

    protected StreamingRequest makeRequestWithOptions()
    {
        RawAudio raw_audio = new RawAudio();
        raw_audio.AudioEncoding = RawAudio.Types.AudioEncoding.Linear16Pcm;
        raw_audio.SampleRateHertz = 44100;
        raw_audio.AudioChannelCount = 2;
        
        AudioFormatOptions audio_format_options = new AudioFormatOptions();
        audio_format_options.RawAudio = raw_audio;
        
        RecognitionModelOptions recognition_model = new RecognitionModelOptions();
        recognition_model.AudioFormat = audio_format_options;
        recognition_model.AudioProcessingType = RecognitionModelOptions.Types.AudioProcessingType.RealTime;
        StreamingOptions streaming_options =new StreamingOptions();
        streaming_options.RecognitionModel = recognition_model;
        
        StreamingRequest streamingRequestWithOptions = new StreamingRequest();
        streamingRequestWithOptions.SessionOptions = streaming_options;

        return streamingRequestWithOptions;
    }

    public void startStreamToApi()
    {
        SslCredentials creds = new SslCredentials();

        GrpcChannelOptions channelOptions = new GrpcChannelOptions();
        channelOptions.Credentials = creds;

        GrpcChannel channel = GrpcChannel.ForAddress(apiUri, channelOptions);
        recognizerClient = new Recognizer.RecognizerClient(channel);
        call = recognizerClient.RecognizeStreaming(makeMetadata(), deadline: DateTime.UtcNow.AddMinutes(5));
        call.RequestStream.WriteAsync(makeRequestWithOptions()).Wait();
    }

    public void writeDataToOpenStreamCall(byte[] bytes)
    {
        Console.WriteLine("Method called");
        StreamingRequest rR = new StreamingRequest();
        AudioChunk audioChunk = new AudioChunk();
        audioChunk.Data = Google.Protobuf.ByteString.CopyFrom(bytes);
        rR.Chunk = audioChunk;
        call.RequestStream.WriteAsync(rR).Wait();
        Console.WriteLine($"{bytes.Length} байт было отправлено в Yandex Speech Kit Recognition");
    }

    async public void readAllDataFromResponseStream()
    {
        
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            //Console.WriteLine($"RESPONSE Uuid : {response.SessionUuid}");
            if (response.Partial != null)
            {
                Console.WriteLine($"RESPONSE PARTIAL : {response.Partial}");
            }
            if (response.Final != null)
            {
                Console.WriteLine($"RESPONSE FINAL : {response.Final}");
            }
        }
    }

    public void disposeAll()
    {
        call.Dispose();
    }
}