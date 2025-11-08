using AlarmClock.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AlarmClock
{
    public partial class MainWindow : Window
    {
        public MainViewModel VM { get; }

        public MainWindow()
        {
            InitializeComponent();
            VM = new MainViewModel(alarmCount: 5); // ← set any number of alarms
            DataContext = VM;
        }
        //private readonly DispatcherTimer _timer = new();
        //private bool _colonVisible = true;


        /*
        _timer.Interval = TimeSpan.FromSeconds(0.5);
        _timer.Tick += (_, __) =>
        {
            _colonVisible = !_colonVisible;
            ClockDisplay.On = _colonVisible;

            var now = DateTime.Now;
            ClockDisplay.Hours = now.Hour;
            ClockDisplay.Minutes = now.Minute;
        };
        _timer.Start();*/
    }
}
