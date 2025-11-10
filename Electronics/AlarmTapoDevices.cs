using AlarmClock.Audio;
using AlarmClock.Electronics.Configurations;
using Core.NativeExtensions;
using System.IO;
using TapoDevices;

namespace AlarmClock.Electronics
{
    public class AlarmTapoDevices
    {
        private static readonly Lazy<AlarmTapoDevices> _instance = new(() => new AlarmTapoDevices());
        public static AlarmTapoDevices Instance => _instance.Value;
        private TapoConfiguration? _Configuration;
        public AlarmTapoDevices() {
            string filePath = Path.Combine(AppContext.BaseDirectory, "tapo_config.json");
            _Configuration = TapoConfiguration.Load(filePath);
            if (_Configuration == null)
            {
                _Configuration = TapoConfiguration.CreateTemplate();
                _Configuration.Save(filePath);
                return;
            }
            TapoDevicesScanner.Initialize(_Configuration.Credentials);
        }
        public void DoAlarm(CancellationToken cancellationToken) {
            new Thread(() =>
            {
                try
                {
                    _DoAlarm(cancellationToken);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }).Start();
        }
        private void _DoAlarm(CancellationToken cancellationToken)
        {
            if (_Configuration == null) return;
            List<IDisposable> toCleanupAfter = new List<IDisposable>();
            cancellationToken.Register(() => {
                lock (toCleanupAfter)
                {
                    foreach (var t in toCleanupAfter)
                    {
                        t.Dispose();
                    }
                }
            });
            if (_Configuration.BulbNames != null)
            {
                foreach (var bulbName in _Configuration.BulbNames)
                {
                    try
                    {
                        TapoBulb? tapoBulb = TapoDevicesScanner.Instance
                            .FindBulb(bulbName)
                            .WaitResult();
                        if (tapoBulb == null) continue;
                        var handle = new TemporaryModifyTapoBulbHandle(tapoBulb);
                         handle.Initialize();
                        lock (toCleanupAfter)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                handle.Dispose();
                                continue;
                            }
                            toCleanupAfter.Add(handle);
                        }
                        handle.SlowlyCrankUp();
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
            }
        }
    }
}
