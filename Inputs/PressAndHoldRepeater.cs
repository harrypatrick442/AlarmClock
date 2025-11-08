using System;
using Timer = System.Timers.Timer;

namespace AlarmClock.Inputs
{
    /// <summary>
    /// Handles press-and-hold behaviour for UI buttons.
    /// - Call <see cref="MouseDown"/> when pressed.
    /// - Call <see cref="MouseUp"/> when released.
    /// After <paramref name="delayMilliseconds"/>, it repeatedly calls <paramref name="callback"/>
    /// every <paramref name="intervalMilliseconds"/> until released.
    /// Optionally notifies when repeating starts and stops.
    /// </summary>
    public class PressAndHoldRepeater : IDisposable
    {
        private readonly Action _callback;
        private readonly Action? _callbackStartingRepeat;
        private readonly Action? _callbackStoppingRepeat;
        private readonly int _delayMilliseconds;
        private readonly int _intervalMilliseconds;

        private readonly Timer _delayTimer;
        private readonly Timer _repeatTimer;

        private bool _isMouseDown;
        private bool _isRepeating;

        public PressAndHoldRepeater(
            Action callback,
            int delayMilliseconds = 500,
            int intervalMilliseconds = 100,
            Action? callbackStartingRepeat = null,
            Action? callbackStoppingRepeat = null)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _callbackStartingRepeat = callbackStartingRepeat;
            _callbackStoppingRepeat = callbackStoppingRepeat;
            _delayMilliseconds = delayMilliseconds;
            _intervalMilliseconds = intervalMilliseconds;

            _delayTimer = new Timer { AutoReset = false };
            _delayTimer.Elapsed += (_, __) =>
            {
                if (_isMouseDown)
                    StartRepeating();
            };

            _repeatTimer = new Timer { AutoReset = true };
            _repeatTimer.Elapsed += (_, __) =>
            {
                if (_isMouseDown)
                    _callback();
            };
        }

        /// <summary>
        /// Should be called on mouse (or pointer) press.
        /// Starts the initial delay timer.
        /// </summary>
        public void MouseDown()
        {
            if (_isMouseDown)
                return;

            _isMouseDown = true;
            _isRepeating = false;
            _delayTimer.Interval = _delayMilliseconds;
            _delayTimer.Start();
        }

        /// <summary>
        /// Should be called on mouse release.
        /// Stops any running timers immediately.
        /// </summary>
        public void MouseUp()
        {
            _isMouseDown = false;

            _delayTimer.Stop();
            _repeatTimer.Stop();

            if (_isRepeating)
            {
                _isRepeating = false;
                _callbackStoppingRepeat?.Invoke();
            }
        }

        private void StartRepeating()
        {
            _isRepeating = true;
            _callbackStartingRepeat?.Invoke();

            _repeatTimer.Interval = _intervalMilliseconds;
            _repeatTimer.Start();
        }

        public void Dispose()
        {
            _delayTimer.Dispose();
            _repeatTimer.Dispose();
        }
    }
}
