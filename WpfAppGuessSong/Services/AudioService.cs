using NAudio.Wave;

namespace WpfAppGuessSong.Services
{
    public class AudioService
    {
        private WaveOutEvent _outputDevice;
        private MediaFoundationReader _audioFile;

        public void Play(string url)
        {
            Stop();
            _outputDevice = new WaveOutEvent();
            _audioFile = new MediaFoundationReader(url);
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }

        public void Stop()
        {
            _outputDevice?.Stop();
            _outputDevice?.Dispose();
            _audioFile?.Dispose();
        }
    }
}