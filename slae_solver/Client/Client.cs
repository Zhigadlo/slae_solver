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

        public Client()
        {
            var settings = new ClientSettings();
            _client = new TcpClient(settings.HostName, settings.Port);
        }

        public float[] SendRequestToSolve(SlaeData slaeData)
        {
            var json = JsonConvert.SerializeObject(slaeData);
            var json1 = SendRequest(json);
            return JsonConvert.DeserializeObject<float[]>(json1);
        }
        private string SendRequest(string json)
        {
            var stream = _client.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            byte[] buffer = new byte[256];
            List<byte> bytes1 = new List<byte>();

            do
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                bytes1.AddRange(buffer.Take(read));
            } while (stream.DataAvailable);

            stream.Close();
            Dispose();

            return Encoding.UTF8.GetString(bytes1.ToArray());
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _client.Close();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
