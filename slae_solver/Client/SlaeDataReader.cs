using Domain;

namespace Client
{
    public class SlaeDataReader
    {
        public SlaeData ReadSlaeData()
        {
            Console.Write("Путь к файлу с матрицей: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            var matrixPath = Console.ReadLine();
            Console.ResetColor();
            var matrix = ReadMatrix(matrixPath);
            Console.Write("Путь к файлу с вектором свободных членов: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            var vectorPath = Console.ReadLine();
            Console.ResetColor();
            var vector = ReadVector(vectorPath);
            var slaeData = new SlaeData(matrix, vector);
            return slaeData;
        }

        private IReadOnlyList<float[]> ReadMatrix(string filename)
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
            var elements = File.ReadAllLines(filename);
            var vector = new float[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                vector[i] = Convert.ToSingle(elements[i]);
            }

            return vector;
        }
    }
}
