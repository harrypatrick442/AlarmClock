using AlarmClock.Interfaces;
using System.Windows.Threading;

namespace AlarmClock
{
    internal class TimeSource : ITimeSource
    {
        private static volatile TimeSource _Instance;
        public static TimeSource Instance {
            get{
                if (_Instance == null) throw new Exception("Not initialized");
                return _Instance; 
            } 
        }
        public event EventHandler<TimeEventArgs> TimeChanged;
        private readonly DispatcherTimer _timer;
        public static TimeSource Initialize() {
            if (_Instance != null) throw new Exception("Already initialized");
            _Instance = new TimeSource();
            return _Instance;
        }
        private TimeSource() {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => Tick();
            _timer.Start();
            var now = DateTime.Now;
            _hours = now.Hour;
            _minutes = now.Minute;
        }
        private int _hours, _minutes;
        public int Hours
        {
            get => _hours;
            protected set { _hours = value; }
        }
        public int Minutes
        {
            get => _minutes;
            protected set { _minutes = value % 60;}
        }
        private void Tick()
        {
            var now = DateTime.Now;
            Hours = now.Hour % 24;
            Minutes = now.Minute % 60;
            TimeChanged?.Invoke(this, new TimeEventArgs(Hours, Minutes));
        }
    }
}
