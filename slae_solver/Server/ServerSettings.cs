using Microsoft.Extensions.Configuration;
using System.Net;

namespace Server
{
    public class ServerSettings
    {
        private static IConfiguration? _config = null;
        private static IPAddress? _ip = null;
        private static int _port = -1;
        private static string _dataPath = null;
        private static string _matrixFilename = null;
        private static string _vectorFilename = null;
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
        public string DataPath
        {
            get
            {
                if (_dataPath == null)
                    _dataPath = GetDataPath();

                return _dataPath;
            }
        }

        public string MatrixFilename
        {
            get
            {
                if( _matrixFilename == null)
                    _matrixFilename = GetMatrixFilename();

                return _matrixFilename;
            }
        }

        public string VectorFilename
        {
            get
            {
                if(_vectorFilename == null)
                    _vectorFilename = GetVectorFilename();

                return _vectorFilename;
            }
        }

        private string GetDataPath()
        {
            return Configuration.GetSection("dataPath").Value;
        }
        private string GetVectorFilename()
        {
            return Configuration.GetSection("files").GetSection("vector").Value;
        }

        private string GetMatrixFilename()
        {
            return Configuration.GetSection("files").GetSection("matrix").Value;
        }
        private IPAddress GetIp()
        {
            return IPAddress.Parse(Configuration.GetSection("NetworkSettings").GetSection("IP").Value);
        }

        private int GetPort()
        {
            return int.Parse(Configuration.GetSection("NetworkSettings").GetSection("port").Value);
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
