using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharedObjects;

public sealed class UnitVoiceDatabase {
    private static readonly Random rnd = new();
    private static Dictionary<string, UnitData> unitStubs = new();
    public static Dictionary<int, UnitData> InUse = new();
    private static HashSet<string> used = [];
    private static HashSet<string> infantryNicknames;
    private static HashSet<string> artilleryNicknames;
    private static HashSet<string> airNicknames;
    private static HashSet<string> playerBaseNicknames;
    private static HashSet<string> genericNicknames;
    public static List<UnitData> AdministrationVoices { get; set; }
    private static UnitVoiceDatabase instance = null;
    private static readonly object padlock = new object();

    public static UnitVoiceDatabase Instance {
        get {
            lock (padlock) {
                return instance ??= new UnitVoiceDatabase();
            }
        }
    }

    static UnitVoiceDatabase() {
        GenerateUnitVoiceMetadata();
    }


    private static void GenerateUnitVoiceMetadata() {
        infantryNicknames = new HashSet<string>(
            File.ReadAllLines("GameStatePresets/InfantryNicknames.txt")
                .OrderBy(_ => rnd.Next()).ToList()
        );
        artilleryNicknames = new HashSet<string>(
            File.ReadAllLines("GameStatePresets/ArtilleryNicknames.txt")
                .OrderBy(_ => rnd.Next()).ToList()
        );
        artilleryNicknames.ExceptWith(infantryNicknames);
        airNicknames = new HashSet<string>(
            File.ReadAllLines("GameStatePresets/AirNicknames.txt")
                .OrderBy(_ => rnd.Next()).ToList()
        );
        airNicknames.ExceptWith(infantryNicknames);
        airNicknames.ExceptWith(artilleryNicknames);

        playerBaseNicknames = new HashSet<string>(
            File.ReadAllLines("GameStatePresets/PlayerBaseNicknames.txt")
                .OrderBy(_ => rnd.Next()).ToList()
        );

        genericNicknames = new HashSet<string>(
            File.ReadAllLines("GameStatePresets/AirNicknames.txt")
                .OrderBy(_ => rnd.Next()).ToList()
        );
        genericNicknames.ExceptWith(infantryNicknames);
        genericNicknames.ExceptWith(artilleryNicknames);
        genericNicknames.ExceptWith(airNicknames);
        genericNicknames.ExceptWith(airNicknames);


        foreach (string nickname in infantryNicknames
                     .Union(artilleryNicknames)
                     .Union(airNicknames)
                     .Union(genericNicknames)) {
            unitStubs.Add(nickname, GetRandomUnitData(nickname, rnd));
        }

        AdministrationVoices = new List<UnitData>() {
            new(
                "administrator1", 0,
                0, 0,
                YandexTtSBackend.FemaleVoices[
                    rnd.Next(YandexTtSBackend.FemaleVoices.Count)
                ].name
            ),

            new(
                "administrator2", 0,
                0, 0,
                YandexTtSBackend.FemaleVoices[
                    rnd.Next(YandexTtSBackend.FemaleVoices.Count)
                ].name
            ),
        };
    }


    private static UnitData GetRandomUnitData(string nickname, Random rnd) {
        return new UnitData(
            nickname: nickname,
            voiceSpeedModificator: 1 + (float)rnd.NextDouble() / 3,
            pitchShift: rnd.NextInt64(-300, 200),
            radioInterferenceModificator: (float)rnd.NextDouble() * 5,
            yandexVoice: YandexTtSBackend.MaleVoices[
                rnd.Next(YandexTtSBackend.MaleVoices.Count)
            ].name
        );
    }

    public static UnitData reserveAndGetUnitData(int unitId, UnitType unitType) {
        HashSet<string> collection = genericNicknames;
        if (unitType == UnitType.InfantryUnit)
            collection = infantryNicknames;
        if (unitType == UnitType.ArtilleryUnit)
            collection = artilleryNicknames;
        if (unitType == UnitType.AirUnit)
            collection = airNicknames;
        string nickname = collection.First();
        collection.Remove(nickname);

        InUse[unitId] = unitStubs[nickname];
        return InUse[unitId];
    }
}

public struct UnitData {
    public string Nickname;
    public float VoiceSpeedModificator; // [1.0; 1.3]
    public float PitchShift; // [-1000;1000]
    public float RadioInterferenceModificator; // [0; 5];
    public string YandexVoice = "";


    public UnitData(
        string nickname, float voiceSpeedModificator, float pitchShift,
        float radioInterferenceModificator, string yandexVoice
    ) {
        Nickname = nickname;
        VoiceSpeedModificator = voiceSpeedModificator;
        PitchShift = pitchShift;
        RadioInterferenceModificator = radioInterferenceModificator;
        YandexVoice = yandexVoice;
    }
}
