using NAudio.CoreAudioApi;

namespace AlarmClock.Audio
{
    public static class SystemVolumeController
    {
        private static readonly MMDeviceEnumerator _enumerator = new();
        private static readonly MMDevice _device =
            _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        private static float? _PreviousVolumeFromNoHandlesTime;
        private static HashSet<TemporaryVolumeAdjustmentHandle> _CurrentHandles = new HashSet<TemporaryVolumeAdjustmentHandle>();

        public static TemporaryVolumeAdjustmentHandle TemporarilyAdjust()
        {
            lock (_CurrentHandles) {
                if (!_CurrentHandles.Any()) { 
                    _PreviousVolumeFromNoHandlesTime = GetVolume();
                }
                var handle = new TemporaryVolumeAdjustmentHandle(
                    SetVolume, GetVolume, RemoveHandle);
                _CurrentHandles.Add(handle);
                return handle;
            }
        }
        public static float GetVolume() =>
            _device.AudioEndpointVolume.MasterVolumeLevelScalar;
        private static void SetVolume(float value) {
            _device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
        }
        private static void RemoveHandle(TemporaryVolumeAdjustmentHandle handle) {
            lock (_CurrentHandles) {
                _CurrentHandles.Remove(handle);
                if (_CurrentHandles.Any())
                    return;
                if (_PreviousVolumeFromNoHandlesTime != null)
                {
                    SetVolume(_PreviousVolumeFromNoHandlesTime.Value);
                }
            }
        }
    }
}
