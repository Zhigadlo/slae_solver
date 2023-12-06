using Domain;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    public class Server : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private bool _isDisposed = false;
        private ServerSettings _settings;
        private List<TcpClient> _servers;
        private List<ClientData> _serversData;
        private Dictionary<int, NetworkStream> _serverStreams;
        private int _serversCount;
        public Server()
        {
            _settings = new ServerSettings();
            _tcpListener = new TcpListener(_settings.Ip, _settings.Port);
            _servers = new List<TcpClient>();
            _serverStreams = new Dictionary<int, NetworkStream>();

            Console.WriteLine("Enter number of node servers: ");
            _serversCount = int.Parse(Console.ReadLine());

            _serversData = new List<ClientData>(_serversCount);
            for (int i = 0; i < _serversCount; i++)
                _serversData.Add(new ClientData());
        }

        public void Start()
        {
            _tcpListener.Start();
            Console.WriteLine("Waiting for client connections...");
            int i = 0;
            while (i < _serversCount)
            {
                var client = _tcpListener.AcceptTcpClient();
                _servers.Add(client);
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
                i++;
            }

            //открываем потоки для чтения и отправки данных клиентам
            for (i = 0; i < _serversCount; i++)
            {
                var stream = _servers[i].GetStream();
                _serverStreams.Add(i, stream);
            }

            Console.WriteLine("Reading SLAE from file...");

            var matrixPath = Path.Combine(_settings.DataPath, _settings.MatrixFilename);
            var vectorPath = Path.Combine(_settings.DataPath, _settings.VectorFilename);
            var matrix = ReadMatrix(matrixPath);
            var vector = ReadVector(vectorPath);

            Console.WriteLine($"Starting to solve matrix {matrix.Count()}x{matrix.First().Length}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            float[] x = Solve(matrix, vector, _serversCount);

            watch.Stop();

            Console.WriteLine("SLAE solution:");
            for (i = 0; i < x.Length; i++)
                Console.WriteLine(x[i]);
            Console.WriteLine($"Execution time: {watch.ElapsedMilliseconds} ms");
            WriteAnswer(x);
            //отправка сообщений клиентам о решении СЛАУ
            var slaeSolvedData = new ClientData { IsSlaeSolved = true };
            Parallel.For(0, _serversCount, i =>
            {
                _serversData[i] = slaeSolvedData;
                SendDataToClient(i);
            });
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

                    SetClientData(i, size, matrix[i], previous);
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
        private void SetClientData(int iteration, int size, float[] matrixRow, float[] previous)
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

        private void WriteAnswer(float[] answer)
        {
            try
            {
                // Если файл существует, он будет перезаписан
                using (StreamWriter sw = new StreamWriter(_settings.AnswerPath))
                {
                    foreach (float x in answer)
                    {
                        sw.WriteLine(x);
                    }
                }

                Console.WriteLine($"Answer was written to {_settings.AnswerPath} successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private List<float[]> ReadMatrix(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                var matrix = new List<float[]>();

                while (!reader.EndOfStream)
                {
                    var elements = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    matrix.Add(new float[elements.Length]);

                    for (int i = 0; i < elements.Length; i++)
                    {
                        matrix[matrix.Count - 1][i] = Convert.ToSingle(elements[i]);
                    }
                }

                return matrix;
            }
        }
        private float[] ReadVector(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                var elements = reader.ReadToEnd().Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vector = new float[elements.Length];

                for (int i = 0; i < elements.Length; i++)
                    vector[i] = Convert.ToSingle(elements[i]);

                return vector;
            }
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
