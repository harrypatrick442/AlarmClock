namespace AlarmClock.Interfaces
{
    internal interface ITimeSource
    {
        public event EventHandler<TimeEventArgs> TimeChanged;
        int Hours { get; }
        int Minutes { get; }
    }
}
