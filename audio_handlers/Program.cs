using System;
using System.IO;
using Vosk;
using NAudio.Wave;
using System.Threading;
using audio_handlers;
using System.Numerics;
public class VoskDemo
{
    public static void DemoBytes(Model model)
    {
        // Demo byte buffer
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        rec.SetMaxAlternatives(0);
        rec.SetWords(true);
        using (Stream source = File.OpenRead("test.wav"))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (rec.AcceptWaveform(buffer, bytesRead))
                {
                    Console.WriteLine(rec.Result());
                }
                else
                {
                    Console.WriteLine(rec.PartialResult());
                }
            }
        }
        Console.WriteLine(rec.FinalResult());
    }

    public static void DemoFloats(Model model)
    {
        // Demo float array
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        using (Stream source = File.OpenRead("test.wav"))
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
                    Console.WriteLine(rec.Result());
                }
                else
                {
                    Console.WriteLine(rec.PartialResult());
                }
            }
        }
        Console.WriteLine(rec.FinalResult());
    }

    public static void DemoSpeaker(Model model)
    {
        // Output speakers
        SpkModel spkModel = new SpkModel("model-spk");
        VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        rec.SetSpkModel(spkModel);

        using (Stream source = File.OpenRead("test.wav"))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (rec.AcceptWaveform(buffer, bytesRead))
                {
                    Console.WriteLine(rec.Result());
                }
                else
                {
                    Console.WriteLine(rec.PartialResult());
                }
            }
        }
        Console.WriteLine(rec.FinalResult());
    }

    public static void ThreadProc()
    {
        while (true)
        {
            // TODO: Здесь выполняется то, что нужно
            Thread.Sleep(1000);
        }
    }
    public static void Main()
    {
        // You can set to -1 to disable logging messages
        //Vosk.Vosk.SetLogLevel(0);
        SpeechRecognizer s = new SpeechRecognizer("C:/Users/User/Desktop/test_app/vosk-model-small-ru-0.22/vosk-model-small-ru-0.22");
        Streamer streamer = new Streamer(s);
        streamer.startStreaming();
        Thread.Sleep(10000);
        //Console.WriteLine("WRAPPER" + s.recognizeSpeechFromWavFile("test.wav"));
        //List<string> known_commands = ["Назад", "Влево", "Вправо", "Вперед"];
        //Parser p = new Parser(known_commands);
        //p.parseStringToCommands("Вверх вниз влево вправо");
    }
}