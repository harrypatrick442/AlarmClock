using System;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace AlarmClock.Audio
{
    public class AudioPlayer : IDisposable
    {
        private IWavePlayer? _output;
        private AudioFileReader? _reader;

        public float Volume
        {
            get => _reader?.Volume ?? 1f;
            set { if (_reader != null) _reader.Volume = value; }
        }

        public void Play(string filePath, CancellationToken? cancellationToken)
        {
            Stop();
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            _reader = new AudioFileReader(filePath);
            _output = new WaveOutEvent();
            _output.Init(_reader);
            _output.Play();
            cancellationToken?.Register(Stop);
        }

        public void Stop()
        {
            _output?.Stop();
            _reader?.Dispose();
            _output?.Dispose();
            _reader = null;
            _output = null;
        }

        public void Dispose() => Stop();
    }
}
