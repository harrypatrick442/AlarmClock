using System;
using System.IO;
using System.Threading;
using Core.FileSystem;
using NAudio.Wave;

namespace AlarmClock.Audio
{
    public sealed class AlarmAudioPlayer
    {
        private static readonly Lazy<AlarmAudioPlayer> _instance = new(() => new AlarmAudioPlayer());
        public static AlarmAudioPlayer Instance => _instance.Value;
        private AlarmAudioPlayer() { 
            
        }

        private AudioPlayer _AudioPlayer = new AudioPlayer();
        private readonly string[] ALLOWED_EXTENSIONS = new string[] { ".mp3", ".wave" };
        public void Play(CancellationToken cancellationToken) {
            string[] audioFiles = Directory.GetFiles(AppContext.BaseDirectory).Where(f => ALLOWED_EXTENSIONS.Contains(Path.GetExtension(f))).ToArray();
            if (!audioFiles.Any()) return;
            _AudioPlayer.Play(audioFiles[0], cancellationToken);
            var handle = SystemVolumeController.TemporarilyAdjust();
            cancellationToken.Register(()=>handle.Dispose());
            handle.SlowlyCrankUp();
            if (cancellationToken.IsCancellationRequested) {
                handle.Dispose();
            }
        }
    }
}
