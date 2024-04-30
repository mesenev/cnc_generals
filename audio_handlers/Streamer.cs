namespace audio_handlers;
using NAudio.Wave;

public class Streamer
{

    private SpeechRecognizer destination;
    private WaveInEvent waveIn;

    public Streamer(SpeechRecognizer destination_of_stream)
    {
        destination = destination_of_stream;
        waveIn = new WaveInEvent();

        waveIn.DataAvailable += (s, a) =>
        {
            destination.recognizeSpeechFromStream(a.Buffer, 0, a.BytesRecorded);
            // if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
            // {
            //         waveIn.StopRecording();
            // }
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
