namespace audio_handlers;
using Vosk;

public class SpeechRecognizer
{
    private Model model;
    private VoskRecognizer rec;

    public SpeechRecognizer(string path_to_module)
    {
        Vosk.SetLogLevel(0);
        model = new Model(path_to_module);
        rec = new VoskRecognizer(model, 16000.0f);
        rec.SetMaxAlternatives(0);
        rec.SetWords(true);
    }

    public string recognizeSpeechFromStream(byte[] data, int offset, int count)
    {
        if (rec.AcceptWaveform(data, count))
        {
            Console.WriteLine(rec.Result());
        }
        else
        {
            Console.WriteLine(rec.PartialResult());
        }
        //Console.WriteLine(rec.FinalResult());
        return "rec.FinalResult();";
    }

    public string recognizeSpeechFromWavFile(string path_to_file)
    {

        string result = "";
        using (Stream source = File.OpenRead(path_to_file))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                float[] fbuffer = new float[bytesRead / 2];
                for (int i = 0, n = 0; i < fbuffer.Length; i++, n += 2)
                {
                    fbuffer[i] = BitConverter.ToInt16(buffer, n);
                }
                if (rec.AcceptWaveform(fbuffer, fbuffer.Length))
                {
                    result += rec.Result();
                }
                else
                {
                    Console.WriteLine(rec.PartialResult());
                }
            }
        }
        return result + rec.FinalResult();
    }
}