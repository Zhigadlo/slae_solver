﻿using Domain;
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
        public Server()
        {
            _settings = new ServerSettings();
            _tcpListener = new TcpListener(_settings.Ip, _settings.Port);
            SetUpThreadPool();
        }

        public void Start()
        {
            _tcpListener.Start();
            Console.WriteLine("Ожидание подключения клиентов...");

            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ClientHandling, client);
            }
        }

        private void ClientHandling(object obj)
        {
            var client = (TcpClient)obj;
            var stream = client.GetStream();

            try
            {
                var clientData = GetRequestData(stream);
                var slaeData = JsonConvert.DeserializeObject<SlaeData>(clientData);
                Console.WriteLine($"Решение СЛАУ клиента {client.Client.RemoteEndPoint}...");
                var x = GaussSolver.Solve(slaeData.Matrix, slaeData.Vector);
                foreach (var element in x)
                    Console.WriteLine(element);
                Console.WriteLine($"СЛАУ решено для клиента {client.Client.RemoteEndPoint}. Идёт отправка данных...");
                var xJson = JsonConvert.SerializeObject(x);
                SendResponse(stream, xJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                stream.Close();
                client.Close();
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

        private static void SendResponse(NetworkStream stream, string json)
        {
            var buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _tcpListener.Stop();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void SetUpThreadPool()
        {
            ThreadPool.SetMinThreads(_settings.MinThreadCount, _settings.MinThreadCount);
            ThreadPool.SetMaxThreads(_settings.MaxThreadCount, _settings.MaxThreadCount);
        }
    }
}