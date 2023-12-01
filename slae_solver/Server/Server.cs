using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Server : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private bool _isDisposed = false;
        private ServerSettings _settings;
        private List<TcpClient> _clients;
        private List<ClientData> _clientData;
        private Dictionary<int, NetworkStream> _clientStreams;
        public Server()
        {
            _settings = new ServerSettings();
            _tcpListener = new TcpListener(_settings.Ip, _settings.Port);
            _clientData = new List<ClientData>();
            _clients = new List<TcpClient>();
            _clientStreams = new Dictionary<int, NetworkStream>();
        }

        public void Start()
        {
            Console.WriteLine("Enter number of clinets: ");
            int clientCount = int.Parse(Console.ReadLine());

            _tcpListener.Start();
            Console.WriteLine("Waiting for client connections...");
            int i = 0;
            while (i < clientCount)
            {
                var client = _tcpListener.AcceptTcpClient();
                _clients.Add(client);
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
                i++;
            }

            //открываем потоки для чтения и отправки данных клиентам
            for (i = 0; i < clientCount; i++)
                _clientStreams.Add(i, _clients[i].GetStream());

            var matrixPath = Path.Combine(_settings.DataPath, _settings.MatrixFilename);
            var vectorPath = Path.Combine(_settings.DataPath, _settings.VectorFilename);
            var matrix = ReadMatrix(matrixPath);
            var vector = ReadVector(vectorPath);

            float[] x = Solve(matrix, vector, clientCount);

            Console.WriteLine("SLAE solution:");
            for (i = 0; i < x.Length; i++)
                Console.WriteLine(x[i]);

            //отправка сообщений клиентам о решении СЛАУ
            var slaeSolvedData = new ClientData { IsSlaeSolved = true };
            foreach(var stream in _clientStreams.Values)
            {
                SendMessage(stream, JsonConvert.SerializeObject(slaeSolvedData));
            }
        }
        private static string GetRequestData(NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            List<byte> bytes = new List<byte>();

            do
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                bytes.AddRange(buffer.Take(read));
            } while (stream.DataAvailable);

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        private static void SendMessage(NetworkStream stream, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        private float[] Solve(List<float[]> matrix, float[] vector, int clientCount, int iterations = 1000)
        {
            int size = vector.Length;
            float[] previous = new float[size];
            float[] current = new float[size];

            for (int i = 0; i < size; i++)
            {
                current[i] = vector[i] / matrix[i][i];
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                Array.Copy(current, previous, size);

                Parallel.For(0, size, i =>
                {
                    float sum = 0f;
                    SetClientData(clientCount, iteration, size, matrix[i], previous);

                    Parallel.For(0, clientCount, k =>
                    {
                        sum += SendDataToClient(k, _clientData[k]);
                    });

                    current[i] = (vector[i] - sum) / matrix[i][i];
                });
            }

            return current;
        }

        private void SetClientData(int clientsCount, int iteration, int size, float[] matrixRow, float[] previous)
        {
            _clientData = new List<ClientData>(clientsCount);
            int step = size / clientsCount;
            int startIter;
            int endIter;
            for (int i = 0; i < clientsCount; i++)
            {
                startIter = step * i;
                endIter = startIter + step;
                _clientData.Add(new ClientData()
                {
                    MatrixRow = matrixRow,
                    Previous = previous,
                    Iteration = iteration,
                    StartIter = startIter,
                    EndIter = endIter
                });
            }
        }
        private float SendDataToClient(int key, ClientData data)
        {
            var stream = _clientStreams[key];
            var jsonData = JsonConvert.SerializeObject(data);
            SendMessage(stream, jsonData);
            string response = GetRequestData(stream);
            return float.Parse(response);
        }
        private List<float[]> ReadMatrix(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                var matrix = new List<float[]>();

                while (!reader.EndOfStream)
                {
                    var elements = reader.ReadLine().Split(' ').Select(float.Parse).ToArray();
                    matrix.Add(elements);
                }

                return matrix;
            }
        }
        private float[] ReadVector(string filename)
        {
            var elements = File.ReadAllLines(filename);
            var vector = new float[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                vector[i] = Convert.ToSingle(elements[i]);
            }

            return vector;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _tcpListener.Stop();
                _isDisposed = true;
                for (int i = 0; i < _clients.Count(); i++)
                {
                    _clientStreams[i].Close();
                    _clients[i].Close();
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}
