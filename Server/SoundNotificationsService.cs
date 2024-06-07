using System.Media;
using System.Diagnostics;
using LiteNetLib;
using NAudio.Wave;
using NetCoreAudio;

namespace Server;

public class SoundNotificationsService {
    private static Player soundPlayer;

    public delegate void playConnected();

    public Task currentTask;

    public SoundNotificationsService() {
        soundPlayer = new();
        playConnected _ = new(PlayConnectedSound);
        // PlayConnectedSound();
    }

    public void PlayConnectedSound() {
        currentTask = soundPlayer.Play("Assets/peer-connected.wav");
    }
    
    public void PlayDisconnectedSound() {
        currentTask = soundPlayer.Play("Assets/peer-disconnected.wav");
    }
}
