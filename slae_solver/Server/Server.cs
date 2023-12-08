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
                _servers.Add(nodeServer);
                var serverStream = nodeServer.GetStream();
                _serverStreams.Add(i, serverStream);
                Console.WriteLine($"Node server {nodeServer.Client.RemoteEndPoint} connected");
                i++;
            }

            Console.WriteLine("All node servers connected and ready to work:)");
            Console.WriteLine("Waiting for clients connections...");
            TcpClient client = _tcpListener.AcceptTcpClient();
            var clientStream = client.GetStream();
            DataManipulation.SendMessage(clientStream, "Server started handling your request");

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

            DataManipulation.SendMessage(clientStream, JsonConvert.SerializeObject(x));

            Console.WriteLine($"SLAE solved for client {client.Client.RemoteEndPoint}");
            long executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine($"Execution time: {executionTime} ms");
            DataManipulation.SendMessage(clientStream, executionTime.ToString());

            //Task.Run(ConnectionReceiving);
            //Task.Run(ClientHandling);
            
        }
        private void ConnectionReceiving()
        {
            Console.WriteLine("Waiting for client connections...");
            while (true)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();
                _clients.Enqueue(client);
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
            }
        }

        private void ClientHandling()
        {
            while (true)
            {
                if (_clients.TryDequeue(out var client))
                {
                    var clientStream = client.GetStream();
                    DataManipulation.SendMessage(clientStream, "Server started handling your request");

                    //получение СЛАУ от клиента
                    int matrixSize = JsonConvert.DeserializeObject<int>(DataManipulation.GetMessage(clientStream));
                    Console.WriteLine($"Matrix size: {matrixSize}");
                    
                    List<float[]> matrix = new List<float[]>(matrixSize);

                    for (int i = 0; i < matrixSize; i++)
                    {
                        var message = DataManipulation.GetMessage(clientStream);
                        Console.WriteLine(message);
                        float[] matrixRow = JsonConvert.DeserializeObject<float[]>(message);
                        matrix.Add(matrixRow);
                    }

                    float[] vector = JsonConvert.DeserializeObject<float[]>(DataManipulation.GetMessage(clientStream));

                    Console.WriteLine($"Starting to solve matrix {matrix.Count()}x{matrix.First().Length}");

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    float[] x = Solve(matrix, vector, _serversCount);

                    watch.Stop();

                    DataManipulation.SendMessage(clientStream, JsonConvert.SerializeObject(x));

                    Console.WriteLine($"SLAE solved for client {client.Client.RemoteEndPoint}");
                    long executionTime = watch.ElapsedMilliseconds;
                    Console.WriteLine($"Execution time: {executionTime} ms");
                    DataManipulation.SendMessage(clientStream, executionTime.ToString());
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
