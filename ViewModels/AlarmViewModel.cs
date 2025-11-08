using AlarmClock.Interfaces;
using AlarmClock.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlarmClock.ViewModels
{
    public class AlarmViewModel : INotifyPropertyChanged, ITimeSource
    {
        public event EventHandler<TimeEventArgs>? TimeChanged;
        private bool _isIlluminated;
        private bool _isEnabled;
        private int _Id;
        public int Number => _Id + 1;
        /// <summary>
        /// Whether the bell/number are currently lit on the display.
        /// </summary>
        public bool IsIlluminated
        {
            get => _isIlluminated;
            set
            {
                if (_isIlluminated != value)
                {
                    _isIlluminated = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether the alarm is actually active (enabled).  
        /// When false, a cross should be drawn over the bell.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Hours { get; protected set; }

        public int Minutes { get; protected set; }

        public AlarmViewModel(int id, Alarm? model)
        {
            _Id = id;
            _isIlluminated = false;
            _isEnabled = false;
            if (model!=null) {
                Hours = model.Hour;
                Minutes = model.Minute;
                _isEnabled = model.Enabled;
                _isIlluminated = _isEnabled;
            }
        }

        public void IncrementHours()
        {
            Hours = (Hours + 1) % 24;
            DispatchTimeChanged();
            Save();
        }

        public void IncrementMinutes()
        {
            Minutes = (Minutes + 1) % 60;
            DispatchTimeChanged();
            Save();
        }

        public void Toggle()
        {
             IsEnabled = !IsEnabled;
            Save();
        }
        private void DispatchTimeChanged() {

            TimeChanged?.Invoke(this, new TimeEventArgs(Hours, Minutes));
        }
        private void Save() {
            DalAlarms.Instance.Set(new Alarm
            {
                Id = _Id,
                Hour = Hours,
                Minute = Minutes,
                Enabled = IsEnabled
            });
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

