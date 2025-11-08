using AlarmClock.Interfaces;
using AlarmClock.Models;

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
        public static AlarmDispatcher Initialize(ICollection<Alarm> alarms) {
            if (_Instance != null) throw new Exception("Already initialized");
            _Instance = new AlarmDispatcher(alarms);
            return _Instance;
        }
        private Dictionary<int, Alarm> _MapIdToEntity;
        private AlarmDispatcher(ICollection<Alarm> alarms) {
            _MapIdToEntity = alarms.ToDictionary(a => a.Id, a => a);
            TimeSource.Instance.TimeChanged += HandleTimeChanged;
            DalAlarms.Instance.OnSaved += HandleSavedAlarm;
        }
        private void HandleTimeChanged(object? sender, TimeEventArgs e) { 
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

        }
        public void TurnOffAlarm() { 

        }
    }
}
