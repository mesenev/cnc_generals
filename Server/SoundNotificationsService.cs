using NetCoreAudio;

namespace Server {
    public class SoundNotificationsService {
        private delegate void playConnected();

        private static Player soundPlayer;

        public Task currentTask;

        public SoundNotificationsService() {
            soundPlayer = new Player();
            playConnected _ = PlayConnectedSound;
            // PlayConnectedSound();
        }

        public void PlayConnectedSound() {
            currentTask = soundPlayer.Play("Assets/peer-connected.wav" );
        }

        public void PlayDisconnectedSound() {
            currentTask = soundPlayer.Play("Assets/peer-disconnected.wav");
        }
    }
}
