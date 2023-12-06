using Microsoft.Extensions.Configuration;

namespace Client
{
    public class NodeServerSettings
    {
        private static IConfiguration? _config = null;
        private static string? _hostname = null;
        private static int _port = -1;

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

        private string GetHostName() => Configuration.GetSection("NetworkSettings").GetSection("hostname").Value;
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
