using SharedObjects.TextToSpeech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharedObjects.GameObjects;
using SharedObjects.GameObjects.Orders;
using SharedObjects.GameObjects.Units;

namespace VoiceResponseModule;

internal static class VoiceRequestToTextService {
    private static readonly Dictionary<VoiceRequestType, string[]> templates = new();
    private static readonly Dictionary<OrderType, string[]> orderTemplates = new();
    private static readonly Random rnd = new();
    private static GameState gameState = null!;
    private static VoiceRequest currentRequest;
    private static BaseUnit unit = null!;


    private static readonly Dictionary<string, Func<string>> keywords = new() {
        { "!TO", RenderTo },
        { "!FROM", RenderFrom },
        { "!FROMSECTOR", RenderFromSector },
        { "!SECTOR", RenderSector },
        { "!TASK", RenderTask },
        { "!TASKNAME", RenderTaskName },
    };

    static VoiceRequestToTextService() {
        foreach (VoiceRequestType type in Enum.GetValues<VoiceRequestType>()) {
            string? typeName = Enum.GetName(type);
            templates[type] = File.ReadAllLines($"VoiceRequestTemplates/{typeName}.txt");
        }

        foreach (OrderType type in Enum.GetValues<OrderType>()) {
            string? typeName = Enum.GetName(type);
            orderTemplates[type] = File.ReadAllLines(
                $"VoiceRequestTemplates/Orders/{typeName}.txt"
            );
        }
    }

    public static string RenderVoiceRequest(VoiceRequest request) {
        currentRequest = request;
        unit = gameState.GetUnitById(request.UnitId);
        string template = templates[request.VoiceRequestType][
            rnd.Next(templates[request.VoiceRequestType].Length)
        ];
        string answer = template;
        var requiredData = keywords.Where(
            x => template.Contains(x.Key)
        );
        foreach (var data in requiredData) {
            answer = template.Replace(data.Key, data.Value());
        }

        return answer;
    }

    private static string RenderTo() {
        return gameState.Players.First(
            player => player.PlayerId == unit.OwnerId
        ).PlayerBaseName;
    }

    private static string RenderFrom() {
        return unit.Nickname;
    }

    private static string RenderFromSector() {
        return gameState.Grid.cells[unit.Y, unit.X].Name;
    }

    private static string RenderSector() {
        if (currentRequest.Sector == null)
            throw new Exception(
                "Sector parameter is not initialized in VoiceRequest but required!"
            );
        return gameState.Grid.cells[currentRequest.Sector[0], currentRequest.Sector[1]].Name;
    }

    private static string RenderTask() {
        if (currentRequest.Order == null)
            throw new Exception(
                "Order parameter is not initialized in VoiceRequest but required!"
            );
        string[] t = orderTemplates[currentRequest.Order.OrderType];
        return t[rnd.Next(t.Length)];
    }

    private static string RenderTaskName() {
        return RenderTask();
    }

    public static void SetGameState(GameState state) => gameState = state;
}