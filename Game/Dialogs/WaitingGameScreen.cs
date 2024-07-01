using System;
using System.Threading;
using System.Threading.Tasks;
using Game.GameObjects;
using Game.Stuff;
using Lime;
using SharedObjects.Network;

namespace Game.Dialogs;

public class WaitingGameScreen : Dialog<Scenes.Data.GameScreen> {
    private Thread networkUpdate = null;
    private bool gameStateReceived = false;

    public WaitingGameScreen() {
        Console.WriteLine("waiting ...");
        NetworkClient.OnJoinEvent += NetworkJoinAcceptedPacketHandler;
        networkUpdate = new Thread(UpdateNetworkManagerState);
        networkUpdate.Start();

        return;
    }

    void NetworkJoinAcceptedPacketHandler(JoinAcceptPacket packet) {
        var InGameScreen = new Frame();
        var gameState = new ClientGameState(packet.state);
        Root.Nodes.Clear();
        The.Game = new GameObjects.Game(gameState, Root);

        The.Game.Canvas.Updated += The.Game.UpdatePlayers;
        The.Game.Canvas.Updated += The.Game.Update;
        gameStateReceived = true;
    }

    protected override void Shown() {
        Scene.RunAnimationInit();
    }

    protected override void Update(float delta) {
        if (Input.WasKeyPressed(Key.Escape)) {
            Lime.Application.Exit();
        }
    }

    private void UpdateNetworkManagerState() {
        The.NetworkClient.Connect(The.Username);
        Thread.Sleep(1000);
        while (!gameStateReceived) {
            The.NetworkClient.Update();
        }

        return;
    }
}
