namespace test_app;
using NAudio.Wave;
public class Recorder
{
    private WaveInEvent waveIn;
    private WaveFileWriter writer;
    public Recorder(){
        waveIn = new WaveInEvent();
        writer = new WaveFileWriter("C:\\Users\\User\\Desktop\\test_app\\recorded_audio\\recor.wav", waveIn.WaveFormat); 
        waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                    {
                        waveIn.StopRecording();
                    }
        };
        waveIn.RecordingStopped += (s, a) =>
        {
                    writer?.Dispose(); 
                    writer = null; 
                    waveIn.Dispose();
        };
    }

    public void startRecord(){
        waveIn.StartRecording();
    }
    public void stopRecord(){
        waveIn.StopRecording();
    }

    
}
