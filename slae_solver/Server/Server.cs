using Domain;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;

namespace Server
{
    public class Server : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private bool _isDisposed = false;
        private ServerSettings _settings;

        private List<TcpClient> _servers;
        private List<NodeServerData> _serversData;
        private ConcurrentQueue<TcpClient> _clients;
        private Dictionary<int, NetworkStream> _serverStreams;

        private int _serversCount;
        public Server()
        {
            _settings = new ServerSettings();
            _tcpListener = new TcpListener(_settings.Ip, _settings.Port);
            _servers = new List<TcpClient>();
            _serverStreams = new Dictionary<int, NetworkStream>();
            _clients = new ConcurrentQueue<TcpClient>();

            Console.WriteLine("Enter number of node servers: ");
            _serversCount = int.Parse(Console.ReadLine());

            _serversData = new List<NodeServerData>(_serversCount);
            for (int i = 0; i < _serversCount; i++)
                _serversData.Add(new NodeServerData());
        }

        public void Start()
        {
            _tcpListener.Start();
            Console.WriteLine("Waiting for server connections...");
            int i = 0;
            while (i < _serversCount)
            {
                var nodeServer = _tcpListener.AcceptTcpClient();
                var serverStream = nodeServer.GetStream();
                string message = DataManipulation.GetMessage(serverStream);
                if (message == "NodeServer")
                {
                    _servers.Add(nodeServer);
                    _serverStreams.Add(i, serverStream);
                    DataManipulation.SendMessage(serverStream, "OK");
                    Console.WriteLine($"Node server {nodeServer.Client.RemoteEndPoint} connected");
                    i++;
                }
                else
                {
                    DataManipulation.SendMessage(serverStream, "Access denied");
                    Console.WriteLine($"Access denied to {nodeServer.Client.RemoteEndPoint}");
                }
            }

            Console.WriteLine("All node servers connected and ready to work:)");
            while (true)
            {
                Console.WriteLine("Waiting for clients connections...");
                TcpClient client = _tcpListener.AcceptTcpClient();
                var clientStream = client.GetStream();
                string message = DataManipulation.GetMessage(clientStream);
                if(message == "Client")
                {
                    DataManipulation.SendMessage(clientStream, "OK");
                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
                    //получение СЛАУ от клиента
                    int matrixSize = JsonConvert.DeserializeObject<int>(DataManipulation.GetMessage(clientStream));
                    Console.WriteLine($"Matrix size: {matrixSize}");

                    Console.WriteLine("Receiving matrix data...");

                    string matrixData = DataManipulation.GetMessage(clientStream);
                    //Console.WriteLine(matrixData);

                    List<float[]> matrix = JsonConvert.DeserializeObject<List<float[]>>(matrixData);

                    float[] vector = matrix.Last();
                    matrix.RemoveAt(matrix.Count - 1);

                    Console.WriteLine($"Starting to solve matrix {matrix.Count()}x{matrix.First().Length}");

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    float[] x = Solve(matrix, vector, _serversCount);

                    watch.Stop();

                    ResultData result = new ResultData
                    {
                        X = x,
                        ExecutionTime = watch.ElapsedMilliseconds
                    };

                    Console.WriteLine($"SLAE solved for client {client.Client.RemoteEndPoint}");
                    Console.WriteLine($"Execution time: {result.ExecutionTime} ms");
                    DataManipulation.SendMessage(clientStream, JsonConvert.SerializeObject(result));
                }
                else
                {
                    DataManipulation.SendMessage(clientStream, "Access denied");
                    Console.WriteLine($"Access denied to {client.Client.RemoteEndPoint}");
                }
            }
        }

        private float[] Solve(List<float[]> matrix, float[] vector, float eps = 0.00001f)
        {
            int size = vector.Length;
            float[] previous = new float[size];
            float[] current = new float[size];

            for (int i = 0; i < size; i++)
            {
                current[i] = vector[i] / matrix[i][i];
            }

            do
            {
                Array.Copy(current, previous, size);

                for (int i = 0; i < size; i++)
                {
                    float sum = 0f;

                    SetNodeServerData(i, size, matrix[i], previous);
                    Parallel.For(0, _serversCount, SendDataToClient);

                    Parallel.For(0, _serversCount, k =>
                    {
                        sum += GetSumFromClient(k);
                    });

                    current[i] = (vector[i] - sum) / matrix[i][i];
                };
            }
            while (!Converged(current, previous, eps));

            return current;
        }
        private bool Converged(float[] prevX, float[] currX, float eps)
        {
            float norm = 0f;
            for (int i = 0; i < prevX.Length; i++)
            {
                norm += (currX[i] - prevX[i]) * (currX[i] - prevX[i]);
            }
            return Math.Sqrt(norm) < eps;
        }
        private void SetNodeServerData(int iteration, int size, float[] matrixRow, float[] previous)
        {
            int step = size / _serversCount;
            int startIter;
            int endIter;
            for (int i = 0; i < _serversCount; i++)
            {
                startIter = step * i;
                endIter = startIter + step;
                var data = _serversData[i];
                data.MatrixRow = matrixRow;
                data.Previous = previous;
                data.Iteration = iteration;
                data.StartIter = startIter;
                data.EndIter = endIter;
            }
        }
        private void SendDataToClient(int key)
        {
            var stream = _serverStreams[key];
            var data = _serversData[key];
            DataManipulation.SendMessage(stream, JsonConvert.SerializeObject(data));
        }
        private float GetSumFromClient(int key)
        {
            var stream = _serverStreams[key];
            string message = DataManipulation.GetMessage(stream);
            return float.Parse(message);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _tcpListener.Stop();
                _isDisposed = true;
                for (int i = 0; i < _servers.Count(); i++)
                {
                    _serverStreams[i].Close();
                    _servers[i].Close();
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}
