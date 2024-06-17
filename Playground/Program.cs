using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using NAudio.Wave;

namespace Playground;

internal static class Program {
    private static void Main(string[] args) {
        // Basic TTS

        MemoryStream stream = new MemoryStream();
        var custom = WaveFormat.CreateCustomFormat(
            WaveFormatEncoding.Pcm,
            22050,
            1,
            44100,
            2,
            16
        );

        byte[] speechBytes;
        using (SpeechSynthesizer synth = new SpeechSynthesizer()) {
            synth.SetOutputToWaveStream(stream);
            synth.Rate = 5;
            synth.Speak("Феникс Севастополю. Задача на перемещение выполнена.");
            speechBytes = stream.GetBuffer();
        }

        var buffered = new BufferedWaveProvider(custom);
        buffered.BufferLength = custom.AverageBytesPerSecond * 30;
        
        var player = new WaveOut();
        player.Init(buffered);
        player.Play();
        Thread.Sleep(500);
        buffered.AddSamples(speechBytes, 0, speechBytes.Length);
        buffered.AddSamples(speechBytes, 0, speechBytes.Length);
        Thread.Sleep(15000);
        return;
        // var file = new WaveFileReader("test.wav");
        // Console.WriteLine(file.WaveFormat.Encoding);
        // Console.WriteLine(file.WaveFormat.SampleRate);
        // Console.WriteLine(file.WaveFormat.Channels);
        // Console.WriteLine(file.WaveFormat.AverageBytesPerSecond);
        // Console.WriteLine(file.WaveFormat.BlockAlign);
        // Console.WriteLine(file.WaveFormat.BitsPerSample);
        // WaveFormat targetFormat = WaveFormat.CreateCustomFormat(
        // 19000, //SampleRate
        // waveIn.WaveFormat.Channels,     //Channels
        // 320000,    //Average Bytes per Second
        // waveIn.WaveFormat.BlockAlign,     //Block Align
        // waveIn.WaveFormat.BitsPerSample);    //Bits per Sample   
    }
}
