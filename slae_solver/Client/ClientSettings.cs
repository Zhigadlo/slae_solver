using Microsoft.Extensions.Configuration;

namespace Client
{
    public class ClientSettings
    {
        private static IConfiguration? _config = null;
        private static string? _hostname = null;
        private static int _port = -1;
        private static string? _matrixPath;
        private static string? _vectorPath;

        public string MatrixPath
        {
            get
            {
                if (_matrixPath == null)
                    _matrixPath = GetMatrixPath();

                return _matrixPath;
            }
        }

        public string VectorPath
        {
            get
            {
                if(_vectorPath == null)
                    _vectorPath = GetVectorPath();

                return _vectorPath;
            }
        }
        public string HostName
        {
            get
            {
                if (_hostname == null)
                    _hostname = GetHostName();

                return _hostname;
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
        private string GetHostName()
        {
            return Configuration.GetSection("NetworkSettings").GetSection("hostname").Value;
        }

        private int GetPort()
        {
            return int.Parse(Configuration.GetSection("NetworkSettings").GetSection("port").Value);
        }

        private string GetMatrixPath()
        {
            return Configuration.GetSection("NetworkSettings").GetSection("matrixfile").Value;
        }

        private string GetVectorPath()
        {
            return Configuration.GetSection("NetworkSettings").GetSection("vectorfile").Value;
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
