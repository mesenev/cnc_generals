namespace audio_handlers;
using NAudio.Wave;
using Vosk;
public class Streamer
{

    private SpeechRecognizer destination;
    private WaveInEvent waveIn;
    Model model;
    VoskRecognizer rec;

    public Streamer(SpeechRecognizer destination_of_stream)
    {
        destination = destination_of_stream;
        waveIn = new WaveInEvent();
        
        Vosk.SetLogLevel(0);
        model = new Model("vosk-model-small-ru-0.22");
        rec = new VoskRecognizer(model, 16000.0f);
        rec.SetMaxAlternatives(0);
        rec.SetWords(true);
        
        waveIn.DataAvailable += (s, a) =>
        {
            // destination.recognizeSpeechFromStream(a.Buffer, 0, a.Buffer.Length);
            // Console.WriteLine(a.Buffer.Length);
            // // if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
            // // {
            // //         waveIn.StopRecording();
            // // }
            if (rec.AcceptWaveform(a.Buffer, a.BytesRecorded))
            {
                Console.WriteLine(rec.Result());
            }
            else
            {
                Console.WriteLine(rec.PartialResult());
            }
        };

        waveIn.RecordingStopped += (s, a) =>
        {
            waveIn.Dispose();
        };
    }

    // public Streamer(network destination);

    public void startStreaming()
    {
        waveIn.StartRecording();
    }
    public void stopStreaming()
    {
        waveIn.StopRecording();
    }
}
