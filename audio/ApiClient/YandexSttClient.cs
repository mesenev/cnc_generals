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

    private Metadata makeMetadata()
    {
        Metadata metadata = new Metadata();
        metadata.Add("authorization", $"Bearer {iamToken}");
        metadata.Add("x-folder-id",  folderId);
        return metadata;
    }

    
    //DefaultEouClassifier max_pause_between_words_hint_ms
    private StreamingRequest makeRequestWithOptions()
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
    
    // ToDo альтернатив может быть больше чем 1, нужно это учесть в этом методе
    async public Task<string> readOneStreamingResponseFromActiveCall()
    {
        string text = "";
        if (call.ResponseStream.MoveNext().Result)
        {
            if (call.ResponseStream.Current.Partial != null)
            {
                if (call.ResponseStream.Current.Partial.Alternatives.Count != 0)
                {
                    text = call.ResponseStream.Current.Partial.Alternatives[0].Text;
                }
            }

            if (call.ResponseStream.Current.Final != null)
            {
                if (call.ResponseStream.Current.Final.Alternatives.Count != 0)
                {
                    text = call.ResponseStream.Current.Final.Alternatives[0].Text;
                }
            }
        }

        return text;
    }

    async public Task<string> readFirstFinalResponseInStream()
    {
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            if (response.Final != null)
            {
                if (response.Final.Alternatives.Count != 0)
                {
                    return response.Final.Alternatives[0].Text;
                }
            }
        }

        throw new Exception("No final recognize result has been return");
    }
    
    async public Task<List<string>> readAllDataFromResponseStream()
    {
            List<string> all_data = new List<string>();
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                //Console.WriteLine($"RESPONSE Uuid : {response.SessionUuid}");
                if (response.Partial != null)
                {
                    foreach (var alternative in response.Partial.Alternatives)
                    {
                        Console.WriteLine($"RESPONSE PARTIAL : {alternative.Text}");
                        all_data.Add(alternative.Text);
                    }
                }

                if (response.Final != null)
                {
                    foreach (var alternative in response.Final.Alternatives)
                    {
                        Console.WriteLine($"RESPONSE FINAL : {alternative.Text}");
                        all_data.Add(alternative.Text);
                    }
                }

                if (response.FinalRefinement != null)
                {
                    foreach (var alternative in response.FinalRefinement.NormalizedText.Alternatives)
                    {
                        Console.WriteLine($"FINAL REFIREMENT : {alternative.Text}");
                        all_data.Add(alternative.Text);
                    }
                }
            }
            return all_data;
    }

    public void disposeAll()
    {
        call.Dispose();
    }
}