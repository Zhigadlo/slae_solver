using Domain;

namespace Client
{
    public class SlaeDataReader
    {
        private readonly string _matrixPath;
        private readonly string _vectorPath;
        public SlaeDataReader()
        {
            var settings = new ClientSettings();
            var path = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            _matrixPath = Path.Combine(path, settings.MatrixPath);
            _vectorPath = Path.Combine(path, settings.VectorPath);
        }
        public SlaeData ReadSlaeData()
        {
            var matrix = ReadMatrix(_matrixPath);
            Console.ResetColor();
            var vector = ReadVector(_vectorPath);
            var slaeData = new SlaeData(matrix, vector);
            Console.WriteLine("Slae data:");
            SlaeDataOutput(slaeData);
            return slaeData;
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


        private void SlaeDataOutput(SlaeData data)
        {
            for(int i = 0; i < data.Vector.Length; i++)
            {
                Console.WriteLine(string.Join(" ", data.Matrix[i].Select(p => p.ToString()).ToArray()) + " " + data.Vector[i]);
            }
        }
    }
}
