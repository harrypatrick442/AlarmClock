using NAudio.CoreAudioApi;
using System.Timers;
using Timer = System.Timers.Timer;
namespace AlarmClock.Audio
{

    public class TemporaryVolumeAdjustmentHandle:IDisposable {
        private Action<float> _SetVolume;
        private Func<float> _GetVolume;
        private Action<TemporaryVolumeAdjustmentHandle> _Remove;
        private Timer _TimerCrankThat;
        private bool _Disposed = false;
        private readonly object _LockObjectDisposed = new object();
        private const float CRANK_UP_STEP = 0.1f;
        private const float CRANK_UP_INTERVAL = 15000;
        public TemporaryVolumeAdjustmentHandle(Action<float> setVolume, Func<float> getVolume, Action<TemporaryVolumeAdjustmentHandle> remove) {
            _SetVolume = setVolume;
            _GetVolume = getVolume;
            _Remove = remove;
            _TimerCrankThat = new Timer();
            _TimerCrankThat.Elapsed += CrankUpABit;
            _TimerCrankThat.Interval = CRANK_UP_INTERVAL;
            _TimerCrankThat.AutoReset = true;
        }
        public void SlowlyCrankUp()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _TimerCrankThat.Start();
            }
        }
        public void StopCrankUp()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _TimerCrankThat.Stop();
            }
        }
        public void Set(float volume)
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _SetVolume(volume);
            }
        }
        private void CrankUpABit(object? sender, ElapsedEventArgs e)
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                float volume = _GetVolume() + CRANK_UP_STEP;
                if (volume > 1f)
                {
                    volume = 1f;
                    _TimerCrankThat.Stop();
                }
                _SetVolume(volume);
            }
        }
        public void Dispose()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return; 
                _TimerCrankThat.Stop();
                _TimerCrankThat.Dispose();
                _Remove(this);
            }
            GC.SuppressFinalize(this);
        }
        ~TemporaryVolumeAdjustmentHandle() {
            Dispose();
        }
    }
}
