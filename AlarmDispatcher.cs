using AlarmClock.Interfaces;
using AlarmClock.Models;
using System.Threading;
using Timer = System.Timers.Timer;
namespace AlarmClock
{
    internal class AlarmDispatcher
    {
        private static volatile AlarmDispatcher _Instance;
        public static AlarmDispatcher Instance {
            get{
                if (_Instance == null) throw new Exception("Not initialized");
                return _Instance; 
            } 
        }
        public static AlarmDispatcher Initialize(ICollection<Alarm> alarms, 
            params Action<CancellationToken>[] doAlarms) {
            if (_Instance != null) throw new Exception("Already initialized");
            _Instance = new AlarmDispatcher(alarms, doAlarms);
            return _Instance;
        }
        private Dictionary<int, Alarm> _MapIdToEntity;
        private Timer _TimerSnooze;
        private readonly object _LockObject = new object();
        private const int SNOOZE_INTERVAL = 15 * 60 * 1000;
        private Action<CancellationToken>[] _DoAlarms;
        private CancellationTokenSource? _CurrentCancellationTokenSource;
        private AlarmDispatcher(ICollection<Alarm> alarms, Action<CancellationToken>[] doAlarms) {
            _DoAlarms = doAlarms;
            _MapIdToEntity = alarms.ToDictionary(a => a.Id, a => a);
            TimeSource.Instance.TimeChanged += HandleTimeChanged;
            DalAlarms.Instance.OnSaved += HandleSavedAlarm;
            _TimerSnooze = new Timer();
            _TimerSnooze.Interval = SNOOZE_INTERVAL;
            _TimerSnooze.AutoReset = false;
        }
        private int _CurrentHoursDidAlarmsFor;
        private int _CurrentMinutesDidAlarmsFor;
        private void HandleTimeChanged(object? sender, TimeEventArgs e) {
            if (_CurrentHoursDidAlarmsFor == e.Hours && _CurrentMinutesDidAlarmsFor == e.Minutes) return;
            _CurrentHoursDidAlarmsFor = e.Hours;
            _CurrentMinutesDidAlarmsFor = e.Minutes;
            foreach(Alarm alarm in _MapIdToEntity.Values)
            {
                if ((!alarm.Enabled)||(e.Hours != alarm.Hour) || (e.Minutes != alarm.Minute)) {
                    continue;
                }
                TurnOnAlarm();
            }
        }
        private void HandleSavedAlarm(object? sender, Alarm e) {
            _MapIdToEntity[e.Id] = e;
        }
        private void TurnOnAlarm()
        {
            CancellationTokenSource cancellationTokenSource;
            lock (_LockObject)
            {
                if (_CurrentCancellationTokenSource != null) return;
                _CurrentCancellationTokenSource = (cancellationTokenSource  = new CancellationTokenSource());
            }
            foreach (var doAlarm in _DoAlarms)
            {
                try { 
                    doAlarm(cancellationTokenSource.Token);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }
        public void Snooze() {
            lock (_LockObject)
            {
                if (_CurrentCancellationTokenSource == null) return;
                _CurrentCancellationTokenSource.Cancel();
                _CurrentCancellationTokenSource = null;
            }
            _TimerSnooze.Stop();
            _TimerSnooze.Start();
        }
        public void TurnOffAlarm()
        {
            lock (_LockObject)
            {
                _CurrentCancellationTokenSource?.Cancel();
                _CurrentCancellationTokenSource = null;
            }
            _TimerSnooze.Stop();
        }
    }
}
