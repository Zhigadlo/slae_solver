using System.Net.Sockets;

namespace Server
{
    public class TcpServer : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private bool _isDisposed = false;
        private Settings _settings;
        public TcpServer()
        {
            _settings = new Settings();
            _tcpListener = new TcpListener(_settings.IP, _settings.Port);
            SetUpThreadPool();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _tcpListener.Stop();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void SetUpThreadPool()
        {
            ThreadPool.SetMinThreads(_settings.MinThreadCount, _settings.MinThreadCount);
            ThreadPool.SetMaxThreads(_settings.MaxThreadCount, _settings.MaxThreadCount);
        }
    }
}
