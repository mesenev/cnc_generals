using System.Collections.Generic;
using System.Linq;

namespace SharedObjects;

public class YandexTtSBackend {
    // Default: 22050 Hz, linear 16-bit signed little-endian PCM, with WAV header
    // https://yandex.cloud/ru/docs/speechkit/tts/voices
    
    public static readonly List<YandexVoice> Voices = new() {
        new YandexVoice { is_male = true, name = "filipp" },
        new YandexVoice { is_male = true, name = "ermil", mood = ["neutral", "good"] },
        new YandexVoice { is_male = true, name = "madirus" },
        new YandexVoice { is_male = false, name = "omazh", mood = ["neutral", "evil"] },
        new YandexVoice { is_male = false, name = "jane", mood = ["neutral", "good", "evil"] },
        new YandexVoice { is_male = false, name = "julia", mood = ["neutral", "strict"] },
        new YandexVoice { is_male = true, name = "alexander", mood = ["neutral", "good"] },
        new YandexVoice { is_male = true, name = "kirill", mood = ["neutral", "strict", "good"] },
        new YandexVoice { is_male = true, name = "anton", mood = ["neutral", "good"] },
    };
    public static List<YandexVoice> MaleVoices { get; set; }
    public static List<YandexVoice> FemaleVoices { get; set; }

    public YandexTtSBackend() {
        MaleVoices = Voices.Where(x => x.is_male).ToList();
        MaleVoices = Voices.Where(x => !x.is_male).ToList();
    }
}

//https://yandex.cloud/ru/docs/speechkit/tts/
public struct YandexVoice {
    public bool is_male;
    public string name;
    public string[]? mood;
}
