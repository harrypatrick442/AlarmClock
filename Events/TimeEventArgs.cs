namespace AlarmClock.Interfaces
{
    public class TimeEventArgs : EventArgs
    {
        public int Hours { get; }
        public int Minutes { get; }
        public TimeEventArgs(int hours, int minutes) {
            Hours = hours;
            Minutes = minutes;
        }
    }
}
