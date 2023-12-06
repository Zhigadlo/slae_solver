using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Client
{
    public class NodeServer : IDisposable
    {
        private readonly TcpClient _client;
        private bool _isDisposed;
        private NetworkStream _stream;

        public NodeServer()
        {
            var settings = new NodeServerSettings();
            _client = new TcpClient(settings.HostName, settings.Port);
            Console.WriteLine($"Connected to host {_client.Client.RemoteEndPoint}");
            _stream = _client.GetStream();
        }

        public ClientData GetDataFromServer()
        {
            string jsonData = DataManipulation.GetMessage(_stream);
            return JsonConvert.DeserializeObject<ClientData>(jsonData);
        }
        public void SendMessage(string message)
        {
            DataManipulation.SendMessage(_stream, message);
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
