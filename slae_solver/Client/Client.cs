using Domain;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Client
{
    public class Client : IDisposable
    {
        private bool _isDisposed = false;
        private TcpClient? _client = null;
        private NetworkStream? _stream = null;
        private ClientSettings _settings;

        public Client()
        {
            _settings = new ClientSettings();
        }

        public void StartSolving()
        {
            Console.WriteLine("Reading SLAE from file...");

            var matrixPath = Path.Combine(_settings.DataPath, _settings.MatrixFilename);
            var vectorPath = Path.Combine(_settings.DataPath, _settings.VectorFilename);
            var matrix = ReadMatrix(matrixPath);
            var vector = ReadVector(vectorPath);

            Console.WriteLine($"Read matrix {matrix.Count()}x{matrix.First().Length}");

            //отпаравка размера матрицы серверу
            SendMessage(JsonConvert.SerializeObject(matrix.Count()));

            matrix.Add(vector);
            string matrixData = JsonConvert.SerializeObject(matrix);
            Console.WriteLine("Sending matrix data to server...");
            DataManipulation.SendMessage(_stream, matrixData);

            Console.WriteLine("SLAE sent to server");
            Console.WriteLine("Waiting for SLAE solution...");

            var answer = DataManipulation.GetMessage(_stream);

            ResultData result = JsonConvert.DeserializeObject<ResultData>(answer);

            Console.WriteLine($"Slae was solved for {result.ExecutionTime} ms");
            WriteAnswer(result.X);

        }
        public bool Connect()
        {
            string ip = "192.168.0.130";
            int port = 8888;
            //Console.Write("Enter server ip: ");
            //string ip = Console.ReadLine();
            //Console.Write("Enter port: ");
            //int port = int.Parse(Console.ReadLine());

            _client = new TcpClient(ip, port);
            _stream = _client.GetStream();
            DataManipulation.SendMessage(_stream, "Client");
            string message = DataManipulation.GetMessage(_stream);
            if (message == "OK")
            {
                Console.WriteLine($"Connected to host {ip}:{port}");
                return true;
            }

            Console.WriteLine($"Connection with host {_client.Client.RemoteEndPoint} failed");

            return false;
        }

        private void SendMessage(string message) => DataManipulation.SendMessage(_stream, message);

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
    }
}
