using Core;
using Core.NativeExtensions;
using NAudio.CoreAudioApi;
using System.Timers;
using System.Windows.Input;
using TapoDevices;
using Timer = System.Timers.Timer;
namespace AlarmClock.Electronics
{

    public class TemporaryModifyTapoBulbHandle : IDisposable
    {
        private bool _Disposed = false;
        private readonly object _LockObjectDisposed = new object();
        private Timer _TimerCrankThat;
        private TapoBulb _TapoBulb;
        private int _InitialBrightness;
        private int _CurrentBrightness;
        private const int CRANK_UP_INTERVAL = 6000;
        private const int CRANK_UP_STEP = 10;
        public TemporaryModifyTapoBulbHandle(
            TapoBulb tapoBulb
        )
        {
            _TapoBulb = tapoBulb;
            _TimerCrankThat = new Timer();
            _TimerCrankThat.Elapsed += CrankUpABit;
            _TimerCrankThat.Interval = CRANK_UP_INTERVAL;
            _TimerCrankThat.AutoReset = true;
            }
            public void Initialize() {
                _InitialBrightness = Task.Run(async () =>
                {
                    await _TapoBulb.ConnectAsync();
                    var info = await _TapoBulb.GetInfoAsync();
                    return info.Brightness;
                }).GetAwaiter().GetResult();
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
        public void SetBrightness(int brightness)
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                Task.Run(async () =>
                {
                    try
                    {
                        await _TapoBulb.SetBrightnessAsync(brightness);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }).GetAwaiter().GetResult();
            }
        }
        private void CrankUpABit(object? sender, ElapsedEventArgs e)
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _CurrentBrightness += CRANK_UP_STEP;
                if (_CurrentBrightness > 100)
                {
                    _CurrentBrightness = 100;
                    _TimerCrankThat.Stop();
                }
                SetBrightness(_CurrentBrightness);
            }
        }
        public void Dispose()
        {
            lock (_LockObjectDisposed)
            {
                if (_Disposed) return;
                _TimerCrankThat.Stop();
                _TimerCrankThat.Dispose();
                SetBrightness(_InitialBrightness);
            }
            GC.SuppressFinalize(this);
        }
        ~TemporaryModifyTapoBulbHandle()
        {
            Dispose();
        }
    }
}
