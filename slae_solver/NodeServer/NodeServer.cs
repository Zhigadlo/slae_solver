using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace NodeServer
{
    public class NodeServer : IDisposable
    {
        private readonly TcpClient _nodeServer;
        private bool _isDisposed;
        private NetworkStream _stream;

        public NodeServer()
        {
            var settings = new NodeServerSettings();
            _nodeServer = new TcpClient(settings.HostName, settings.Port);
            Console.WriteLine($"Connected to host {_nodeServer.Client.RemoteEndPoint}");
            _stream = _nodeServer.GetStream();
        }

        public NodeServerData GetDataFromServer()
        {
            string jsonData = DataManipulation.GetMessage(_stream);
            return JsonConvert.DeserializeObject<NodeServerData>(jsonData);
        }
        public void SendMessage(string message) => DataManipulation.SendMessage(_stream, message);

        public float JacobiHandle(NodeServerData data)
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
                _nodeServer.Close();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
