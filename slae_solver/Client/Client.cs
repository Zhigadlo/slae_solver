using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client : IDisposable
    {
        private readonly TcpClient _client;
        private bool _isDisposed;
        private static NetworkStream _stream;

        public Client()
        {
            var settings = new ClientSettings();
            _client = new TcpClient(settings.HostName, settings.Port);
            _stream = _client.GetStream();
        }

        public ClientData GetDataFromServer()
        {
            byte[] buffer = new byte[256];

            do
            {
                int read = _stream.Read(buffer, 0, buffer.Length);
            } while (_stream.DataAvailable);
            string data = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<ClientData>(data);
        }
        public void SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Flush();
        }
        public float JacobiHandle(ClientData data)
        {
            float sum = 0f;

            for (int i = data.StartIter; i < data.EndIter; i++)
            {
                if (data.Iteration != i)
                {
                    sum += data.MatrixRow[i] * data.Previous[i];
                }
            }

            return sum;
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _stream.Close();
                _client.Close();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
