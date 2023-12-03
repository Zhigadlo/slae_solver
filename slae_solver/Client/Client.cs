using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Client
{
    public class Client : IDisposable
    {
        private readonly TcpClient _client;
        private bool _isDisposed;
        private NetworkStream _stream;

        public Client()
        {
            var settings = new ClientSettings();
            _client = new TcpClient(settings.HostName, settings.Port);
            Console.WriteLine($"Connected to host {_client.Client.RemoteEndPoint}");
            _stream = _client.GetStream();
        }

        public ClientData GetDataFromServer()
        {

            float[] matrixRow = DataManipulation.GetArray(_stream);
            float[] previous = DataManipulation.GetArray(_stream);
            int iteration = int.Parse(DataManipulation.GetMessage(_stream));
            int startIter = int.Parse(DataManipulation.GetMessage(_stream));
            int endIter = int.Parse(DataManipulation.GetMessage(_stream));
            bool isSlaeSolved = bool.Parse(DataManipulation.GetMessage(_stream));
            return new ClientData
            {
                MatrixRow = matrixRow,
                Previous = previous,
                Iteration = iteration,
                StartIter = startIter,
                EndIter = endIter,
                IsSlaeSolved = isSlaeSolved
            };
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
