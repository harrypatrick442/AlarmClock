using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using AlarmClock.Interfaces;
using Microsoft.Win32;
using Timer = System.Timers.Timer;
using System.Timers;
using AlarmClock.Models;
using AlarmClock.Inputs;

namespace AlarmClock.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _colonOn = true;
        private bool _numbersOn = true;
        private int _currentMode = -1; // -1 = clock, 0–3 = alarm index
        private Timer _timerFlashColon;
        private Timer _timerFlashEntireTime;
        private Timer _timerTimeoutSetAlarmsDueToInactivity;
        private PressAndHoldRepeater _hourRepeater;
        private PressAndHoldRepeater _minutesRepeater;
        private ITimeSource _CurrentTimeSource;

        public ObservableCollection<AlarmViewModel> Alarms { get; } = new();

        public int Hours
        {
            get => _CurrentTimeSource.Hours;
        }

        public int Minutes
        {
            get => _CurrentTimeSource.Minutes;
        }

        public bool ColonOn
        {
            get => _colonOn;
            set
            {
                if (_colonOn == value) return;
                _colonOn = value; OnPropertyChanged(); 
            }
        }
        public bool NumbersOn
        {
            get => _numbersOn;
            set {
                if (_numbersOn == value) return;
                _numbersOn = value; OnPropertyChanged();
            }
        }

        public string ModeText => IsShowingActualTime ? "CLOCK" : $"ALARM {_currentMode+1}";

        // Commands
        public ICommand SnoozeCommand { get; }
        public ICommand CycleModeCommand { get; }
        public ICommand ClickedHoursCommand { get; }
        public ICommand ClickedMinutesCommand { get; }
        public ICommand ToggleAlarmCommand { get; }
        public ICommand AlarmOffCommand { get; }
        public ICommand MouseUpHoursCommand { get; }
        public ICommand MouseUpMinutesCommand { get; }
        public ICommand MouseDownHoursCommand { get; }
        public ICommand MouseDownMinutesCommand { get; }
        private bool IsShowingActualTime => _currentMode < 0;

        public MainViewModel(int alarmCount)
        {
            _timerFlashColon = new Timer();
            _timerFlashColon.Interval = 500;
            _timerFlashColon.Elapsed += HandleTimerFlashColonElapsed;
            _timerFlashEntireTime = new Timer();
            _timerFlashEntireTime.Interval = 500;
            _timerFlashEntireTime.Elapsed += HandlerTimerFlashEntireTimeElapsed;
            _timerTimeoutSetAlarmsDueToInactivity = new Timer();
            _timerTimeoutSetAlarmsDueToInactivity.Interval = 10000;
            _timerTimeoutSetAlarmsDueToInactivity.Elapsed += HandlerTimerTimeoutSetAlarmsDueToInactivityElapsed;
            _timerTimeoutSetAlarmsDueToInactivity.AutoReset = false;
            ReplaceTimeSource(TimeSource.Initialize());
            // Initialize alarms
            var alarms = DalAlarms.Instance
                    .GetAll();
            AlarmDispatcher.Initialize(alarms);
            var mapIdToAlarmModel = alarms
                    .ToDictionary(a => a.Id, a => a);
            for (int i = 0; i < alarmCount; i++) {
                mapIdToAlarmModel.TryGetValue(i, out Alarm? model);
                Alarms.Add(new AlarmViewModel(i, model));
            }

            // Commands 
            SnoozeCommand = new RelayCommand(_ => Snooze());
            AlarmOffCommand = new RelayCommand(_ => AlarmDispatcher.Instance.TurnOffAlarm());
            CycleModeCommand = new RelayCommand(_ => CycleMode());
            ClickedHoursCommand = new RelayCommand(_ => ClickedHours());
            ClickedMinutesCommand = new RelayCommand(_ => ClickedMinutes());
            ToggleAlarmCommand = new RelayCommand(_ => ToggleAlarm());
            _hourRepeater = new PressAndHoldRepeater(ClickedHours
            , delayMilliseconds: 500, intervalMilliseconds: 200,
            callbackStartingRepeat:StartingRepeat,
            callbackStoppingRepeat: StoppingRepeat);
            _minutesRepeater = new PressAndHoldRepeater(ClickedMinutes
            , delayMilliseconds: 500, intervalMilliseconds: 200,
            callbackStartingRepeat: StartingRepeat,
            callbackStoppingRepeat: StoppingRepeat);
            MouseDownHoursCommand = new RelayCommand(_ => _hourRepeater.MouseDown());
            MouseDownMinutesCommand = new RelayCommand(_ => _minutesRepeater.MouseDown());
            MouseUpHoursCommand = new RelayCommand(_ => _hourRepeater.MouseUp());
            MouseUpMinutesCommand = new RelayCommand(_ => _minutesRepeater.MouseUp());
        }



        private void HandleTimeChanged(object? sender, TimeEventArgs e)
        {
            OnPropertyChanged(nameof(Hours));
            OnPropertyChanged(nameof(Minutes));
            //ColonOn = !ColonOn;
        }
        private void HandleTimerFlashColonElapsed(object? sender, ElapsedEventArgs e) {
             ColonOn = !ColonOn;
        }
        private void HandlerTimerFlashEntireTimeElapsed(object? sender, ElapsedEventArgs e)
        {
            ColonOn = !ColonOn;
            NumbersOn = ColonOn;
        }
        private void HandlerTimerTimeoutSetAlarmsDueToInactivityElapsed(object? sender, ElapsedEventArgs e)
        {
            ToClockMode();
        }

        private void Snooze() =>
            System.Media.SystemSounds.Beep.Play();

        private void CycleMode()
        {
            _currentMode++;
            if (_currentMode >= Alarms.Count)
            {
                ToClockMode();
            }
            else
            {
                ToAlarmMode();
            }
            OnPropertyChanged(nameof(ModeText));
        }
        private void ToClockMode() {
            _currentMode = -1;
            ReplaceTimeSource(TimeSource.Instance);
            _timerTimeoutSetAlarmsDueToInactivity.Stop();
            foreach (var alarm in Alarms)
            {
                alarm.IsIlluminated = alarm.IsEnabled;
            }
        }
        private void ToAlarmMode()
        {
            RestartTimeoutSetAlarmsDueToInactivity();
            var alarmModifying = Alarms[_currentMode];
            foreach (var alarm in Alarms)
            {
                bool isActive = alarm.Equals(alarmModifying);
                alarm.IsIlluminated = isActive;
                if (isActive) {
                    ReplaceTimeSource(alarm);
                }
            }
        }

        private void ClickedHours()
        {
            AlarmViewModel? alarm = GetActiveAlarm();
            if (alarm == null) return;
            alarm.IncrementHours();
            RestartTimeoutSetAlarmsDueToInactivity();
            ColonOn = true;
            NumbersOn = true;
        }

        private void ClickedMinutes()
        {
            AlarmViewModel? alarm = GetActiveAlarm();
            if (alarm == null) return;
            alarm.IncrementMinutes();
            RestartTimeoutSetAlarmsDueToInactivity();
            ColonOn = true;
            NumbersOn = true;
        }
        private AlarmViewModel? GetActiveAlarm() {
            if (_currentMode < 0) return null;
            return Alarms[_currentMode];
        }
        private void ReplaceTimeSource(ITimeSource newTimeSource) {
            if (_CurrentTimeSource==newTimeSource) return;
            if (_CurrentTimeSource != null) { 
                _CurrentTimeSource.TimeChanged-= HandleTimeChanged;
                _timerFlashEntireTime.Stop();
                _timerFlashColon.Stop();
                ColonOn = true;
                NumbersOn = true;

            }
            _CurrentTimeSource = newTimeSource;
            _CurrentTimeSource.TimeChanged += HandleTimeChanged;
            if (IsShowingActualTime)
            {
                _timerFlashColon.Start();
            }
            else {
                _timerFlashEntireTime.Start();
            }
            OnPropertyChanged(nameof(Hours));
            OnPropertyChanged(nameof(Minutes));
        }
        private void RestartTimeoutSetAlarmsDueToInactivity() {
            if (IsShowingActualTime) return;
            _timerTimeoutSetAlarmsDueToInactivity.Stop();
            _timerTimeoutSetAlarmsDueToInactivity.Start();
        }
        private void ToggleAlarm()
        {
            if (_currentMode >= 0 && _currentMode < Alarms.Count)
                Alarms[_currentMode].Toggle();
        }
        private void StartingRepeat() {
            _timerFlashEntireTime.Stop();
        }
        private void StoppingRepeat() {
            if (!IsShowingActualTime) {
                _timerFlashEntireTime.Start();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
