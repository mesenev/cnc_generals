using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using SharedObjects.TextToSpeech;

namespace VoiceResponseModule.Backends;

public class MsEmbeddedTtSBackend : ITtSBackend<UnitVoiceData> {
    private static SpeechSynthesizer synth = new();
    private static PromptBuilder builder = new(new CultureInfo("ru-RU"));
    private static PromptStyle style = new() { Emphasis = PromptEmphasis.Reduced };

    public async Task<byte[]> Synthesize(string text, UnitVoiceData args) {
        MemoryStream stream = new MemoryStream();
        synth.SetOutputToWaveStream(stream);
        builder.StartStyle(style);
        builder.AppendText(text);
        builder.EndStyle();
        var p = synth.SpeakAsync(builder);
        builder.ClearContent();
        byte[] speechBytes = stream.GetBuffer();
        return speechBytes;
    }
}
