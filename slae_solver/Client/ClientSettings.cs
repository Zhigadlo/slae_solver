using Microsoft.Extensions.Configuration;

namespace Client
{
    public class ClientSettings
    {
        private static string? _dataPath = null;
        private static string? _answerPath = null;
        private static string? _matrixFilename = null;
        private static string? _vectorFilename = null;
        private static IConfiguration? _config = null;
        public IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                    _config = GetConfiguration();

                return _config;
            }
        }

        public string AnswerPath
        {
            get
            {
                if (_answerPath == null)
                    _answerPath = GetAnswerPath();

                return _answerPath;
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
                if (_matrixFilename == null)
                    _matrixFilename = GetMatrixFilename();

                return _matrixFilename;
            }
        }
        public string VectorFilename
        {
            get
            {
                if (_vectorFilename == null)
                    _vectorFilename = GetVectorFilename();

                return _vectorFilename;
            }
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

        private string GetAnswerPath() => Configuration.GetSection("ClientSettings").GetSection("answerPath").Value;
        private string GetDataPath() => Configuration.GetSection("ClientSettings").GetSection("dataPath").Value;
        private string GetVectorFilename() => Configuration.GetSection("ClientSettings").GetSection("files").GetSection("vector").Value;
        private string GetMatrixFilename() => Configuration.GetSection("ClientSettings").GetSection("files").GetSection("matrix").Value;
    }
}
