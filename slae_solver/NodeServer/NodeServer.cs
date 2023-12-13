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
            _stream = _nodeServer.GetStream();

        }

        public bool Connect()
        {
            DataManipulation.SendMessage(_stream, "NodeServer");
            string message = DataManipulation.GetMessage(_stream);
            if (message == "OK")
            {
                Console.WriteLine($"Connected to host {_nodeServer.Client.RemoteEndPoint}");
                return true;
            }

            Console.WriteLine($"Connection with host {_nodeServer.Client.RemoteEndPoint} failed");
            return false;
        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    bool isSlaeSolved = false;
                    while (!isSlaeSolved)
                    {
                        var data = GetDataFromServer();
                        if (data.IsSlaeSolved)
                        {
                            isSlaeSolved = data.IsSlaeSolved;
                            continue;
                        }

                        var sum = JacobiHandle(data);
                        DataManipulation.SendMessage(_stream, sum.ToString());
                        Console.WriteLine($"Sent data to server: {sum}");
                    }

                    Console.WriteLine("SLAE solved:)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private NodeServerData GetDataFromServer()
        {
            string jsonData = DataManipulation.GetMessage(_stream);
            return JsonConvert.DeserializeObject<NodeServerData>(jsonData);
        }
        private float JacobiHandle(NodeServerData data)
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
