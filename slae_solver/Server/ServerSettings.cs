using Microsoft.Extensions.Configuration;
using System.Net;

namespace Server
{
    public class ServerSettings
    {
        private static IConfiguration? _config = null;
        private static IPAddress? _ip = null;
        private static int _port = -1;
        private static int _minThreadCount = -1;
        private static int _maxThreadCount = -1;
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

        public int MinThreadCount
        {
            get
            {
                if (_minThreadCount == -1)
                    _minThreadCount = GetMinThreadCount();
                return _minThreadCount;
            }
        }

        public int MaxThreadCount
        {
            get
            {
                if (_maxThreadCount == -1)
                    _maxThreadCount = GetMaxThreadCount();
                return _maxThreadCount;
            }
        }

        private IPAddress GetIp()
        {
            return IPAddress.Parse(Configuration.GetSection("NetworkSettings").GetSection("IP").Value);
        }

        private int GetPort()
        {
            return int.Parse(Configuration.GetSection("NetworkSettings").GetSection("port").Value);
        }

        private int GetMinThreadCount()
        {
            return int.Parse(Configuration.GetSection("NetworkSettings").GetSection("minThreadCount").Value);
        }

        private int GetMaxThreadCount()
        {
            return int.Parse(Configuration.GetSection("NetworkSettings").GetSection("maxThreadCount").Value);
        }

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
