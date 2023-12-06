using Microsoft.Extensions.Configuration;
using System.Net;

namespace Server
{
    public class ServerSettings
    {
        private static IConfiguration? _config = null;
        private static IPAddress? _ip = null;
        private static int _port = -1;

        public IPAddress Ip
        {
            get
            {
                if (_ip == null)
                    _ip = GetIp();

                return _ip;
            }
        }
        public int Port
        {
            get
            {
                if (_port == -1)
                    _port = GetPort();

                return _port;
            }
        }
        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                    _config = GetConfiguration();

                return _config;
            }
        }

        private IPAddress GetIp() => IPAddress.Parse(Configuration.GetSection("NetworkSettings").GetSection("IP").Value);
        private int GetPort() => int.Parse(Configuration.GetSection("NetworkSettings").GetSection("port").Value);
        private IConfiguration GetConfiguration()
        {
            string configFileName = "config.json";
            string path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile(configFileName, optional: false);

            return builder.Build();
        }
    }
}
